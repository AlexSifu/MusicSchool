import { apiFetch } from "/js/auth.js";
import { API_BASE_URL } from "/js/config.js";
import { formatearFechaISO } from "/js/utils.js";

document.addEventListener("DOMContentLoaded", async () => {
    await loadSchools();
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get("id");

    if (!id) {
        alert("ID de profesor no especificado.");
        window.location.href = "/Teachers";
        return;
    }

    //const nameInput = document.getElementById("name");
    //const dniInput = document.getElementById("dni");
    const firstNameInput = document.getElementById("firstName");
    const lastNameInput = document.getElementById("lastName");
    const identificationNumberInput = document.getElementById("identificationNumber");
    const schoolIdInput = document.getElementById("schoolId");

    // Cargar datos actuales del profesor
    const res = await apiFetch(API_BASE_URL+`/teachers/${id}`);
    if (!res || !res.ok) {
        alert("No se pudo cargar el profesor.");
        window.location.href = "/Teachers";
        return;
    }

    const student = await res.json();
    firstNameInput.value = student.firstName;
    lastNameInput.value = student.lastName;
    identificationNumberInput.value = student.identificationNumber;
    schoolIdInput.value = student.schoolId;

    // Manejo del envío del formulario
    document.getElementById("editForm").addEventListener("submit", async (e) => {
        e.preventDefault();

        const updatedStudent = {
            id: parseInt(id),
            firstName: firstNameInput.value,
            lastName: lastNameInput.value,
            identificationNumber: identificationNumberInput.value,
            schoolId: schoolIdInput.value
        };

        const updateRes = await apiFetch(API_BASE_URL+`/teachers/${id}`, {
            method: "PUT",
            body: JSON.stringify(updatedStudent)
        });

        if (updateRes && updateRes.ok) {
            window.location.href = "/Teachers";
        } else {
            alert("No se pudo actualizar el profesor.");
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