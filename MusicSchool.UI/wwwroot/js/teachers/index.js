import { apiFetch } from "/js/auth.js";
import { API_BASE_URL } from "/js/config.js";
import { convertirFechaYEdad } from "/js/utils.js";

document.addEventListener("DOMContentLoaded", async () => {
    const tableBody = document.querySelector("#generalTable tbody");

    async function loadStudents() {
        const res = await apiFetch(API_BASE_URL+"/teachers");
        if (!res) return;

        const teachers = await res.json();
        tableBody.innerHTML = "";

        teachers.forEach(s => {

            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${s.identificationNumber}</td>
                <td>${s.fullName}</td>
                <td>${s.schoolName}</td>
                <td>
                    <a class="btn btn-sm btn-primary" href="/Teachers/Edit?id=${s.id}">Editar</a>
                    <button class="btn btn-sm btn-danger" data-id="${s.id}">Eliminar</button>
                </td>
            `;
            tableBody.appendChild(row);
        });

        document.querySelectorAll("button[data-id]").forEach(btn => {
            btn.addEventListener("click", async () => {
                const id = btn.getAttribute("data-id");
                if (!confirm("¿Deseas eliminar este profesor?")) return;

                const res = await apiFetch(API_BASE_URL +`/teachers/${id}`, { method: "DELETE" });
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
