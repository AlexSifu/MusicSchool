import { apiFetch } from "/js/auth.js";
import { API_BASE_URL } from "/js/config.js";

document.addEventListener("DOMContentLoaded", async () => {
    await loadSchools();
    setupSchoolsSelect();
    setupTeachersSelect();
    setupButtonAssign();
    setupButtonRemove();
});

async function loadSchools() {
    try {
        const res = await apiFetch(API_BASE_URL + "/Inscriptions/Schools", {
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

function setupSchoolsSelect() {
    const schoolId = document.getElementById('schoolId');
    schoolId.addEventListener('change', async () => {
        console.log(schoolId.value);
        const studentsContainer = document.getElementById('studentsContainer');
        studentsContainer.classList.add('d-none');

        // Obtener el select de profesores
        const teacherSelect = document.getElementById('teacherId');

        // Si no se seleccionó una escuela, limpiar el select de profesores
        if (!schoolId.value) {
            teacherSelect.innerHTML = '<option value="">Selecciona un profesor</option>';
            return;
        }

        try {
            // Mostrar loading mientras se cargan los datos
            teacherSelect.innerHTML = '<option value="">Cargando profesores...</option>';

            // Hacer la petición al endpoint
            const res = await apiFetch(API_BASE_URL + `/inscriptions/teachers-by-school/${schoolId.value}`, {
                method: "GET",
            });

            const data = await res.json();
            // Limpiar el select
            teacherSelect.innerHTML = '<option value="">Selecciona un profesor</option>';

            // Verificar si hay profesores
            if (data && data.length > 0) {
                // Agregar cada profesor como una opción
                data.forEach(teacher => {
                    const option = document.createElement('option');
                    option.value = teacher.id; // Ajusta según la propiedad real
                    option.textContent = teacher.fullName; // Ajusta según la propiedad real
                    teacherSelect.appendChild(option);
                });
            } else {
                // Si no hay profesores
                const option = document.createElement('option');
                option.value = '';
                option.textContent = 'No hay profesores disponibles en esta escuela';
                option.disabled = true;
                teacherSelect.appendChild(option);
            }

        } catch (error) {
            console.error('Error al cargar profesores:', error);

            // Mostrar error en el select
            teacherSelect.innerHTML = '<option value="">Error al cargar profesores</option>';
        }
    });
}

function setupTeachersSelect() {
    const teacherId = document.getElementById('teacherId');
    const schoolId = document.getElementById('schoolId');
    teacherId.addEventListener('change', async () => {
        console.log(teacherId.value);
        const studentsContainer = document.getElementById('studentsContainer');
        studentsContainer.classList.remove('d-none');
        await getStudentsBySchool(schoolId.value);
        await getStudentsByTeacher(teacherId.value);
    });
}

async function getStudentsBySchool(schoolId) {
    try {
        // Obtener la tabla
        const table = document.getElementById('schoolStudentsTable');
        // Mostrar loading mientras se cargan los datos
        table.innerHTML = `
            <thead>
                <tr>
                    <th>Seleccionar</th>
                    <th>DNI</th>
                    <th>Nombre Completo</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td colspan="4" style="text-align: center;">Cargando estudiantes...</td>
                </tr>
            </tbody>
        `;
        // Hacer la petición al endpoint
        const res = await apiFetch(API_BASE_URL + `/inscriptions/students-by-school/${schoolId}`, {
            method: "GET",
        });
        const data = await res.json();
        // Crear el encabezado de la tabla
        let tableHTML = `
            <thead>
                <tr>
                    <th>Seleccionar</th>
                    <th>DNI</th>
                    <th>Nombre Completo</th>
                </tr>
            </thead>
            <tbody>
        `;
        // Verificar si hay estudiantes
        if (data && data.length > 0) {
            // Agregar cada estudiante como una fila
            data.forEach(student => {
                tableHTML += `
                    <tr>
                        <td class="text-center">
                            <input type="checkbox" class="student-checkbox" data-id="${student.id}" />
                        </td>
                        <td>${student.identificationNumber}</td>
                        <td>${student.fullName}</td>
                    </tr>
                `;
            });
        } else {
            // Si no hay estudiantes
            tableHTML += `
                <tr>
                    <td colspan="4" style="text-align: center;">No hay estudiantes en esta escuela</td>
                </tr>
            `;
        }
        tableHTML += '</tbody>';
        // Actualizar la tabla
        table.innerHTML = tableHTML;

        // Agregar event listeners a los checkboxes
        const checkboxes = table.querySelectorAll('.student-checkbox');
        checkboxes.forEach(checkbox => {
            checkbox.addEventListener('change', (e) => {
                const studentId = e.target.getAttribute('data-id');
                const isChecked = e.target.checked;

                console.log(`Estudiante ID: ${studentId}, Seleccionado: ${isChecked}`);

                // Aquí puedes agregar la lógica cuando se selecciona/deselecciona
                handleStudentSelection(studentId, isChecked);
            });
        });

    } catch (error) {
        console.error('Error al cargar estudiantes:', error);
        // Mostrar error en la tabla
        const table = document.getElementById('schoolStudentsTable');
        table.innerHTML = `
            <thead>
                <tr>
                    <th>Seleccionar</th>
                    <th>DNI</th>
                    <th>Nombre Completo</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td colspan="4" style="text-align: center; color: red;">Error al cargar estudiantes</td>
                </tr>
            </tbody>
        `;
    }
}

// Función auxiliar para manejar la selección de estudiantes
function handleStudentSelection(studentId, isChecked) {
    if (isChecked) {
        console.log(`Estudiante ${studentId} seleccionado`);
        // Lógica cuando se selecciona
    } else {
        console.log(`Estudiante ${studentId} deseleccionado`);
        // Lógica cuando se deselecciona
    }
}

// Función auxiliar para obtener todos los estudiantes seleccionados
function getSelectedStudents() {
    const selectedCheckboxes = document.querySelectorAll('.student-checkbox:checked');
    const selectedIds = Array.from(selectedCheckboxes).map(checkbox => checkbox.getAttribute('data-id'));
    return selectedIds;
}

async function getStudentsByTeacher(teacherId) {
    try {
        // Obtener la tabla
        const table = document.getElementById('teacherStudentsTable');
        // Mostrar loading mientras se cargan los datos
        table.innerHTML = `
            <thead>
                <tr>
                    <th>Seleccionar</th>
                    <th>DNI</th>
                    <th>Nombre Completo</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td colspan="4" style="text-align: center;">Cargando estudiantes...</td>
                </tr>
            </tbody>
        `;
        // Hacer la petición al endpoint
        const res = await apiFetch(API_BASE_URL + `/inscriptions/students-by-teacher/${teacherId}`, {
            method: "GET",
        });
        const data = await res.json();
        // Crear el encabezado de la tabla
        let tableHTML = `
            <thead>
                <tr>
                    <th>Seleccionar</th>
                    <th>DNI</th>
                    <th>Nombre Completo</th>
                </tr>
            </thead>
            <tbody>
        `;
        // Verificar si hay estudiantes
        if (data && data.length > 0) {
            // Agregar cada estudiante como una fila
            data.forEach(student => {
                tableHTML += `
                    <tr>
                        <td class="text-center">
                            <input type="checkbox" class="teacher-student-checkbox" data-id="${student.id}" />
                        </td>
                        <td>${student.identificationNumber}</td>
                        <td>${student.fullName}</td>
                    </tr>
                `;
            });
        } else {
            // Si no hay estudiantes
            tableHTML += `
                <tr>
                    <td colspan="4" style="text-align: center;">No hay estudiantes asignados a este profesor</td>
                </tr>
            `;
        }
        tableHTML += '</tbody>';
        // Actualizar la tabla
        table.innerHTML = tableHTML;

        // Agregar event listeners a los checkboxes
        const checkboxes = table.querySelectorAll('.teacher-student-checkbox');
        checkboxes.forEach(checkbox => {
            checkbox.addEventListener('change', (e) => {
                const studentId = e.target.getAttribute('data-id');
                const isChecked = e.target.checked;

                console.log(`Estudiante del profesor ID: ${studentId}, Seleccionado: ${isChecked}`);

                // Aquí puedes agregar la lógica cuando se selecciona/deselecciona
                handleTeacherStudentSelection(studentId, isChecked);
            });
        });

    } catch (error) {
        console.error('Error al cargar estudiantes del profesor:', error);
        // Mostrar error en la tabla
        const table = document.getElementById('teacherStudentsTable');
        table.innerHTML = `
            <thead>
                <tr>
                    <th>Seleccionar</th>
                    <th>DNI</th>
                    <th>Nombre Completo</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td colspan="4" style="text-align: center; color: red;">Error al cargar estudiantes del profesor</td>
                </tr>
            </tbody>
        `;
    }
}

// Función auxiliar para manejar la selección de estudiantes del profesor
function handleTeacherStudentSelection(studentId, isChecked) {
    if (isChecked) {
        console.log(`Estudiante del profesor ${studentId} seleccionado`);
        // Lógica cuando se selecciona
    } else {
        console.log(`Estudiante del profesor ${studentId} deseleccionado`);
        // Lógica cuando se deselecciona
    }
}

// Función auxiliar para obtener todos los estudiantes del profesor seleccionados
function getSelectedTeacherStudents() {
    const selectedCheckboxes = document.querySelectorAll('.teacher-student-checkbox:checked');
    const selectedIds = Array.from(selectedCheckboxes).map(checkbox => checkbox.getAttribute('data-id'));
    return selectedIds;
}

function setupButtonAssign() {
    const btnAssignStudents = document.getElementById('btnAssignStudents');
    const teacherSelect = document.getElementById('teacherId');

    btnAssignStudents.addEventListener('click', async () => {
        const teacherId = teacherSelect.value;
        const studentsIds = getSelectedStudents();

        // Validaciones
        if (!teacherId) {
            alert('Por favor, selecciona un profesor');
            return;
        }

        if (!studentsIds || studentsIds.length === 0) {
            alert('Por favor, selecciona al menos un estudiante');
            return;
        }

        // Confirmación antes de proceder
        const confirmMessage = `¿Estás seguro de que deseas asignar ${studentsIds.length} estudiante(s) al profesor seleccionado?`;
        const userConfirmed = confirm(confirmMessage);

        if (!userConfirmed) {
            return; // El usuario canceló la operación
        }

        try {
            // Deshabilitar el botón mientras se procesa
            btnAssignStudents.disabled = true;
            btnAssignStudents.textContent = 'Asignando...';

            // Preparar los datos según el DTO de .NET
            const assignData = {
                TeacherId: parseInt(teacherId),
                StudentIds: studentsIds.map(id => parseInt(id))
            };

            // Enviar la petición al endpoint
            const response = await apiFetch(API_BASE_URL + '/inscriptions/assign-students', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(assignData)
            });

            // Verificar si la respuesta fue exitosa
            if (response.ok) {
                // Mostrar mensaje de éxito
                alert('Estudiantes asignados correctamente');

                // Refrescar la tabla de estudiantes del profesor
                await getStudentsByTeacher(teacherId);

                // Desseleccionar todos los checkboxes de la tabla de estudiantes por escuela
                const schoolCheckboxes = document.querySelectorAll('.student-checkbox');
                schoolCheckboxes.forEach(checkbox => {
                    checkbox.checked = false;
                });

                console.log('Estudiantes asignados exitosamente');
            } else {
                // Manejar errores de la respuesta
                const errorData = await response.json();
                throw new Error(errorData.message || 'Error al asignar estudiantes');
            }

        } catch (error) {
            console.error('Error al asignar estudiantes:', error);
            alert('Error al asignar estudiantes: ' + error.message);
        } finally {
            // Rehabilitar el botón
            btnAssignStudents.disabled = false;
            btnAssignStudents.textContent = 'Asignar Estudiantes';
        }
    });
}

function setupButtonRemove() {
    const btnRemoveStudents = document.getElementById('btnRemoveStudents');
    const teacherSelect = document.getElementById('teacherId');

    btnRemoveStudents.addEventListener('click', async () => {
        const teacherId = teacherSelect.value;
        const studentsIds = getSelectedTeacherStudents();

        // Validaciones
        if (!teacherId) {
            alert('Por favor, selecciona un profesor');
            return;
        }

        if (!studentsIds || studentsIds.length === 0) {
            alert('Por favor, selecciona al menos un estudiante');
            return;
        }

        // Confirmación antes de proceder
        const confirmMessage = `¿Estás seguro de que deseas quitar ${studentsIds.length} estudiante(s) del profesor seleccionado?`;
        const userConfirmed = confirm(confirmMessage);

        if (!userConfirmed) {
            return; // El usuario canceló la operación
        }

        try {
            // Deshabilitar el botón mientras se procesa
            btnRemoveStudents.disabled = true;
            btnRemoveStudents.textContent = 'Removiendo...';

            // Preparar los datos según el DTO de .NET
            const assignData = {
                TeacherId: parseInt(teacherId),
                StudentIds: studentsIds.map(id => parseInt(id))
            };

            // Enviar la petición al endpoint
            const response = await apiFetch(API_BASE_URL + '/inscriptions/remove-students', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(assignData)
            });

            // Verificar si la respuesta fue exitosa
            if (response.ok) {
                // Mostrar mensaje de éxito
                alert('Estudiantes removidos correctamente');

                // Refrescar la tabla de estudiantes del profesor
                await getStudentsByTeacher(teacherId);

                // Desseleccionar todos los checkboxes de la tabla de estudiantes por escuela
                const schoolCheckboxes = document.querySelectorAll('.student-checkbox');
                schoolCheckboxes.forEach(checkbox => {
                    checkbox.checked = false;
                });

                console.log('Estudiantes removidos exitosamente');
            } else {
                // Manejar errores de la respuesta
                const errorData = await response.json();
                throw new Error(errorData.message || 'Error al remover estudiantes');
            }

        } catch (error) {
            console.error('Error al remover estudiantes:', error);
            alert('Error al remover estudiantes: ' + error.message);
        } finally {
            // Rehabilitar el botón
            btnRemoveStudents.disabled = false;
            btnRemoveStudents.textContent = 'Remover estudiates';
        }
    });
}