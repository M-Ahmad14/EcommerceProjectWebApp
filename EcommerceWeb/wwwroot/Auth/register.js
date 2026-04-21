$(document).ready(function () {
    // Remove error while typing
    $("#name").on("keyup", function () {
        if ($(this).val().trim() !== "") {
            $("#nameError").text("");
        }
    });

    $("#email").on("keyup", function () {
        if ($(this).val().trim() !== "") {
            $("#emailError").text("");
        }
    });

    $("#password").on("keyup", function () {
        if ($(this).val().trim() !== "") {
            $("#passwordError").text("");
        }

        checkPasswordMatch();
    });

    $("#confirmPassword").on("keyup", function () {
        if ($(this).val().trim() !== "") {
            $("#confirmPasswordError").text("");
        }

        checkPasswordMatch();
    });
});

function validateRegisterForm() {
    let isValid = true;

    $(".text-danger").text("");

    const name = $("#name").val().trim();
    const email = $("#email").val().trim();
    const password = $("#password").val().trim();
    const confirmPassword = $("#confirmPassword").val().trim();
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (name === "") {
        $("#nameError").text("Full name is required");
        isValid = false;
    }

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

    if (confirmPassword === "") {
        $("#confirmPasswordError").text("Confirm password is required");
        isValid = false;
    } else if (password !== confirmPassword) {
        $("#confirmPasswordError").text("Passwords do not match");
        isValid = false;
    }

    return isValid;
}

function checkPasswordMatch() {
    const password = $("#password").val().trim();
    const confirmPassword = $("#confirmPassword").val().trim();

    if (confirmPassword !== "" && password === confirmPassword) {
        $("#confirmPasswordError").text("");
    }
}

function togglePassword(inputId, btn) {
    const input = $("#" + inputId);

    if (input.attr("type") === "password") {
        input.attr("type", "text");
        $(btn).text("Hide");
    } else {
        input.attr("type", "password");
        $(btn).text("Show");
    }
}

async function registerUser() {
    if (!validateRegisterForm()) {
        return;
    }

    const requestBody = {
        name: $("#name").val().trim(),
        email: $("#email").val().trim(),
        password: $("#password").val().trim()
    };

    try {
        const response = await apiRequest(
            "POST",
            "https://localhost:7096/api/Auth/register",
            requestBody
        );

        console.log("API Success Response:", response);
        window.location.href = "/Auth/Login";
    } catch (error) {
        console.error("API Error Response:", error);
        $("#emailError").text(error.message || "Registration failed");
    }
}