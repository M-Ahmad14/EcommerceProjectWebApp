(function () {
    const TOKEN_STORAGE_KEY = "authToken";
    const loaderElement = document.getElementById("globalLoader");
    let pendingLoaderCount = 0;

    function getAuthToken() {
        return localStorage.getItem(TOKEN_STORAGE_KEY) || "";
    }

    function setAuthToken(token) {
        if (!token) {
            return;
        }
        localStorage.setItem(TOKEN_STORAGE_KEY, token);
    }

    function clearAuthToken() {
        localStorage.removeItem(TOKEN_STORAGE_KEY);
    }

    function showLoader() {
        if (!loaderElement) {
            return;
        }

        pendingLoaderCount += 1;
        loaderElement.classList.add("is-active");
        loaderElement.setAttribute("aria-hidden", "false");
    }

    function hideLoader() {
        if (!loaderElement) {
            return;
        }

        pendingLoaderCount = Math.max(0, pendingLoaderCount - 1);
        if (pendingLoaderCount === 0) {
            loaderElement.classList.remove("is-active");
            loaderElement.setAttribute("aria-hidden", "true");
        }
    }

    async function apiRequest(method, url, data = null, options = {}) {
        const requestMethod = String(method || "GET").toUpperCase();
        const shouldUseLoader = options.useLoader !== false;

        const headers = {
            "Content-Type": "application/json",
            ...(options.headers || {})
        };
        const token = getAuthToken();
        if (token && !headers.Authorization) {
            headers.Authorization = "Bearer " + token;
        }

        const requestOptions = {
            method: requestMethod,
            headers,
            credentials: options.credentials || "same-origin"
        };

        if (data != null && requestMethod !== "GET" && requestMethod !== "DELETE") {
            requestOptions.body = JSON.stringify(data);
        }

        if (shouldUseLoader) {
            showLoader();
        }

        try {
            const response = await fetch(url, requestOptions);

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || `Request failed with status ${response.status}`);
            }

            if (response.status === 204) {
                return null;
            }

            const contentType = response.headers.get("content-type") || "";
            if (contentType.includes("application/json")) {
                return await response.json();
            }

            return await response.text();
        } finally {
            if (shouldUseLoader) {
                hideLoader();
            }
        }
    }

    function onNavigationStart(event) {
        const target = event.target;
        if (!(target instanceof HTMLAnchorElement)) {
            return;
        }

        if (!target.href || target.target === "_blank" || target.hasAttribute("download")) {
            return;
        }

        const targetUrl = new URL(target.href, window.location.origin);
        if (targetUrl.origin !== window.location.origin) {
            return;
        }

        showLoader();
    }

    function onFormSubmit() {
        showLoader();
    }

    window.addEventListener("pageshow", function () {
        pendingLoaderCount = 0;
        if (loaderElement) {
            loaderElement.classList.remove("is-active");
            loaderElement.setAttribute("aria-hidden", "true");
        }
    });

    document.addEventListener("click", onNavigationStart);
    document.addEventListener("submit", onFormSubmit);

    window.ApiClient = {
        request: apiRequest,
        get: function (url, options = {}) {
            return apiRequest("GET", url, null, options);
        },
        post: function (url, data, options = {}) {
            return apiRequest("POST", url, data, options);
        },
        put: function (url, data, options = {}) {
            return apiRequest("PUT", url, data, options);
        },
        delete: function (url, options = {}) {
            return apiRequest("DELETE", url, null, options);
        },
        getToken: getAuthToken,
        setToken: setAuthToken,
        clearToken: clearAuthToken
    };

    window.apiRequest = apiRequest;
    window.showGlobalLoader = showLoader;
    window.hideGlobalLoader = hideLoader;
})();
