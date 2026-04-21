$(document).ready(function () {
    $("#email").on("keyup", function () {
        if ($(this).val().trim() !== "") {
            $("#emailError").text("");
        }
    });

    $("#password").on("keyup", function () {
        if ($(this).val().trim() !== "") {
            $("#passwordError").text("");
        }
    });

    $("#loginForm").on("submit", function (event) {
        event.preventDefault();
        logIn();
    });
});

function validateLoginForm() {
    let isValid = true;

    $("#emailError").text("");
    $("#passwordError").text("");

    const email = $("#email").val().trim();
    const password = $("#password").val().trim();
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (email === "") {
        $("#emailError").text("Email is required");
        isValid = false;
    } else if (!emailPattern.test(email)) {
        $("#emailError").text("Invalid email address");
        isValid = false;
    }

    if (password === "") {
        $("#passwordError").text("Password is required");
        isValid = false;
    }

    return isValid;
}

async function logIn() {
    if (!validateLoginForm()) {
        return;
    }

    const email = $("#email").val().trim();
    const password = $("#password").val().trim();

    console.log("Login Email:", email);
    console.log("Login Password:", password);

    try {
        const response = await apiRequest("POST", "https://localhost:7096/api/Auth/login", { email, password });
        const token = extractToken(response);

        console.log("Login API Response:", response);

        if (!token) {
            throw new Error("Login succeeded but token was not found. Check API response format.");
        }

        ApiClient.setToken(token);
        window.location.assign("/Product/AllProducts");
    } catch (error) {
        console.error("Login error:", error);
        $("#passwordError").text(error.message || "Login failed");
    }
}

function extractToken(response) {
    if (!response) {
        return "";
    }

    if (typeof response === "string") {
        return response;
    }

    const directToken =
        response.token ||
        response.Token ||
        response.accessToken ||
        response.AccessToken ||
        response.jwtToken ||
        response.JwtToken;

    if (directToken) {
        return directToken;
    }

    if (response.data && typeof response.data === "object") {
        return (
            response.data.token ||
            response.data.Token ||
            response.data.accessToken ||
            response.data.AccessToken ||
            ""
        );
    }

    return "";
}
