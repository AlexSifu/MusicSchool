import { apiFetch } from "/js/auth.js";
import { API_BASE_URL } from "/js/config.js";

document.addEventListener("DOMContentLoaded", async () => {
    await loadSchools();
    const form = document.getElementById("createForm");

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const firstName = document.getElementById("firstName").value;
        const lastName = document.getElementById("lastName").value;
        const identificationNumber = document.getElementById("identificationNumber").value;
        const schoolId = document.getElementById("schoolId").value;

        const res = await apiFetch(API_BASE_URL+"/teachers", {
            method: "POST",
            body: JSON.stringify({ firstName, lastName, identificationNumber, schoolId })
        });

        if (res && res.ok) {
            window.location.href = "/Teachers";
        } else {
            alert("Error al crear profesor");
        }
    });
});

async function loadSchools() {
    try {
        const res = await apiFetch(API_BASE_URL + "/Teachers/Schools", {
            method: "GET",
        });

        const data = await res.json();

        // Obtener el elemento select
        const selectElement = document.getElementById('schoolId');

        // Limpiar opciones existentes (excepto la primera si es un placeholder)
        selectElement.innerHTML = '<option value="">Selecciona una escuela</option>';

        // Verificar si hay datos
        if (data && data.length > 0) {
            // Agregar cada escuela como una opción
            data.forEach(school => {
                const option = document.createElement('option');
                option.value = school.id; // Asume que tu objeto school tiene una propiedad 'id'
                option.textContent = school.name; // Asume que tu objeto school tiene una propiedad 'name'
                selectElement.appendChild(option);
            });
        } else {
            // Si no hay escuelas, mostrar mensaje
            const option = document.createElement('option');
            option.value = '';
            option.textContent = 'No hay escuelas disponibles';
            option.disabled = true;
            selectElement.appendChild(option);
        }

    } catch (error) {
        console.error('Error al cargar escuelas:', error);

        // Mostrar error en el select
        const selectElement = document.getElementById('schoolId');
        selectElement.innerHTML = '<option value="">Error al cargar escuelas</option>';
    }
}
