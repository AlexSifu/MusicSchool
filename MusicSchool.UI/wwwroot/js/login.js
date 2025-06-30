document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("loginForm");
    const errorDiv = document.getElementById("error");

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const username = document.getElementById("username").value;
        const password = document.getElementById("password").value;

        const response = await fetch("https://localhost:7164/api/auth/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ username, password })
        });

        const data = await response.json();

        if (!response.ok) {
            errorDiv.textContent = data.message || "Error al iniciar sesión";
        } else {
            localStorage.setItem("token", data.token);
            window.location.href = "/";
        }
    });
});
