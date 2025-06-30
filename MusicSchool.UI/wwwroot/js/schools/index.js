import { apiFetch } from "/js/auth.js";
import { API_BASE_URL } from "/js/config.js";
import { convertirFechaYEdad } from "/js/utils.js";

document.addEventListener("DOMContentLoaded", async () => {
    const tableBody = document.querySelector("#generalTable tbody");

    async function loadStudents() {
        const res = await apiFetch(API_BASE_URL+"/Schools");
        if (!res) return;

        const schools = await res.json();
        tableBody.innerHTML = "";

        schools.forEach(s => {
            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${s.code}</td>
                <td>${s.name}</td>
                <td>${s.description}</td>
                <td>
                    <a class="btn btn-sm btn-primary" href="/Schools/Edit?id=${s.id}">Editar</a>
                    <button class="btn btn-sm btn-danger" data-id="${s.id}">Eliminar</button>
                </td>
            `;
            tableBody.appendChild(row);
        });

        document.querySelectorAll("button[data-id]").forEach(btn => {
            btn.addEventListener("click", async () => {
                const id = btn.getAttribute("data-id");
                if (!confirm("¿Deseas eliminar esta escuela?")) return;

                const res = await apiFetch(API_BASE_URL +`/Schools/${id}`, { method: "DELETE" });
                if (res && res.ok) {
                    loadStudents();
                } else {
                    alert("Error al eliminar");
                }
            });
        });
    }

    loadStudents();
});
