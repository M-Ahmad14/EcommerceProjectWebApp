document.addEventListener("DOMContentLoaded", function () {
    loadProducts();
});

const PRODUCTS_API_URL = "https://localhost:7096/api/Product";
const REVIEWS_API_BASE_URL = "https://localhost:7096/api/reviews";
const IMAGE_BASE_URL = "https://localhost:7096";

async function loadProducts() {
    const container = document.getElementById("productsContainer");
    const messageBox = document.getElementById("productsMessage");

    if (!container || !messageBox) {
        return;
    }

    container.innerHTML = "";
    hideMessage(messageBox);

    try {
        const products = await getAllProducts();
        const productsWithReviews = await attachReviewsToProducts(products);
        renderProducts(container, messageBox, productsWithReviews);
    } catch (error) {
        showMessage(messageBox, error.message || "Failed to load products", "danger");
    }
}

async function getAllProducts() {
    const products = await ApiClient.get(PRODUCTS_API_URL);
    return Array.isArray(products) ? products : [];
}

async function attachReviewsToProducts(products) {
    const result = await Promise.all(products.map(async function (product) {
        let reviews = [];

        try {
            const response = await apiRequest(
                "GET",
                REVIEWS_API_BASE_URL + "/" + product.id,
                null,
                { useLoader: false }
            );
            reviews = Array.isArray(response) ? response : [];
        } catch (error) {
            console.warn("Reviews could not be loaded for product " + product.id, error);
        }

        return {
            ...product,
            reviews: reviews
        };
    }));

    return result;
}

function renderProducts(container, messageBox, products) {
    if (!Array.isArray(products) || products.length === 0) {
        showMessage(messageBox, "No products found.", "warning");
        return;
    }

    const cardsHtml = products.map(function (product) {
        return createProductCard(product);
    });

    container.innerHTML = cardsHtml.join("");
}

function createProductCard(product) {
    const carouselId = "productCarousel_" + product.id;
    const carouselHtml = createCarouselHtml(carouselId, product.images || []);
    const ratingInfo = buildRatingInfo(product.reviews || []);
    const commentsHtml = createCommentsHtml(product.reviews || []);

    return `
        <div class="col-12 col-md-6 col-xl-4">
            <article class="card product-card h-100">
                ${carouselHtml}
                <div class="card-body d-flex flex-column">
                    <h2 class="h5 card-title mb-2">${escapeHtml(product.name || "Unnamed Product")}</h2>
                    <p class="product-price mb-2">PKR ${formatPrice(product.price)}</p>
                    <p class="text-muted mb-3">${escapeHtml(product.description || "No description available.")}</p>
                    <div class="product-rating mb-2">
                        <span class="me-2">${ratingInfo.stars}</span>
                        <small class="text-muted">${ratingInfo.label}</small>
                    </div>
                    <div class="product-comments mt-auto">
                        ${commentsHtml}
                    </div>
                </div>
            </article>
        </div>
    `;
}

function createCarouselHtml(carouselId, images) {
    if (!Array.isArray(images) || images.length === 0) {
        return `
            <div class="product-no-image d-flex align-items-center justify-content-center">
                <span>No image available</span>
            </div>
        `;
    }

    const indicators = images.map(function (_, index) {
        const activeClass = index === 0 ? "active" : "";
        const current = index === 0 ? 'aria-current="true"' : "";
        return `
            <button type="button" data-bs-target="#${carouselId}" data-bs-slide-to="${index}" class="${activeClass}" ${current} aria-label="Slide ${index + 1}"></button>
        `;
    }).join("");

    const slides = images.map(function (image, index) {
        const activeClass = index === 0 ? "active" : "";
        return `
            <div class="carousel-item ${activeClass}">
                <img src="${resolveImageUrl(image)}" class="d-block w-100 product-image" alt="Product image ${index + 1}">
            </div>
        `;
    }).join("");

    return `
        <div id="${carouselId}" class="carousel slide" data-bs-ride="carousel">
            <div class="carousel-indicators">${indicators}</div>
            <div class="carousel-inner">${slides}</div>
            <button class="carousel-control-prev" type="button" data-bs-target="#${carouselId}" data-bs-slide="prev">
                <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                <span class="visually-hidden">Previous</span>
            </button>
            <button class="carousel-control-next" type="button" data-bs-target="#${carouselId}" data-bs-slide="next">
                <span class="carousel-control-next-icon" aria-hidden="true"></span>
                <span class="visually-hidden">Next</span>
            </button>
        </div>
    `;
}

function createCommentsHtml(reviews) {
    if (!reviews.length) {
        return '<p class="mb-0 text-muted small">No comments yet.</p>';
    }

    const latestThree = reviews.slice(0, 3);
    return latestThree.map(function (review) {
        return `
            <div class="review-item">
                <p class="mb-1 small">"${escapeHtml(review.comment || "")}"</p>
                <small class="text-muted">- ${escapeHtml(review.userName || "Anonymous")}</small>
            </div>
        `;
    }).join("");
}

function buildRatingInfo(reviews) {
    if (!reviews.length) {
        return {
            stars: "☆☆☆☆☆",
            label: "No ratings"
        };
    }

    const total = reviews.reduce(function (sum, review) {
        return sum + (review.rating || 0);
    }, 0);
    const average = total / reviews.length;
    const rounded = Math.round(average);
    const stars = "★".repeat(rounded) + "☆".repeat(5 - rounded);

    return {
        stars: stars,
        label: average.toFixed(1) + " / 5 (" + reviews.length + " reviews)"
    };
}

function resolveImageUrl(imagePath) {
    if (!imagePath) {
        return "";
    }

    if (imagePath.startsWith("http://") || imagePath.startsWith("https://")) {
        return imagePath;
    }

    return IMAGE_BASE_URL + imagePath;
}

function formatPrice(price) {
    if (price == null || Number.isNaN(Number(price))) {
        return "0";
    }

    return Number(price).toLocaleString();
}

function showMessage(messageBox, text, type) {
    messageBox.className = "alert alert-" + type;
    messageBox.textContent = text;
    messageBox.classList.remove("d-none");
}

function hideMessage(messageBox) {
    messageBox.classList.add("d-none");
}

function escapeHtml(value) {
    return String(value)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll("\"", "&quot;")
        .replaceAll("'", "&#39;");
}
