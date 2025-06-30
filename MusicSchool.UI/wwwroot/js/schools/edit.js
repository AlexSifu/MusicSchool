import { apiFetch } from "/js/auth.js";
import { API_BASE_URL } from "/js/config.js";
import { formatearFechaISO } from "/js/utils.js";

document.addEventListener("DOMContentLoaded", async () => {
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get("id");

    if (!id) {
        alert("ID de escuela no especificado.");
        window.location.href = "/Schools";
        return;
    }

    //const nameInput = document.getElementById("name");
    //const dniInput = document.getElementById("dni");
    const codeInput = document.getElementById("code");
    const nameInput = document.getElementById("name");
    const descriptionInput = document.getElementById("description");

    // Cargar datos actuales del escuela
    const res = await apiFetch(API_BASE_URL+`/Schools/${id}`);
    if (!res || !res.ok) {
        alert("No se pudo cargar la escuela.");
        window.location.href = "/Schools";
        return;
    }

    const school = await res.json();
    codeInput.value = school.code;
    nameInput.value = school.name
    descriptionInput.value = school.description;

    // Manejo del envío del formulario
    document.getElementById("editForm").addEventListener("submit", async (e) => {
        e.preventDefault();

        const updatedStudent = {
            id: parseInt(id),
            code: codeInput.value,
            name: nameInput.value,
            description: descriptionInput.value,
        };

        const updateRes = await apiFetch(API_BASE_URL+`/Schools/${id}`, {
            method: "PUT",
            body: JSON.stringify(updatedStudent)
        });

        if (updateRes && updateRes.ok) {
            window.location.href = "/Schools";
        } else {
            alert("No se pudo actualizar la escuela.");
        }
    });
});
