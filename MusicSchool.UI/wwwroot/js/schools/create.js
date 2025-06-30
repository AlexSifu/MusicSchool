import { apiFetch } from "/js/auth.js";
import { API_BASE_URL } from "/js/config.js";

document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("createForm");

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const code = document.getElementById("code").value;
        const name = document.getElementById("name").value;
        const description = document.getElementById("description").value;

        const res = await apiFetch(API_BASE_URL+"/Schools", {
            method: "POST",
            body: JSON.stringify({ code, name, description })
        });

        if (res && res.ok) {
            window.location.href = "/Schools";
        } else {
            alert("Error al crear estudiante");
        }
    });
});
