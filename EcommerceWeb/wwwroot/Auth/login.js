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

function logIn() {
    if (!validateLoginForm()) {
        return;
    }

    const email = $("#email").val().trim();
    const password = $("#password").val().trim();

    console.log("Login Email:", email);
    console.log("Login Password:", password);
}
