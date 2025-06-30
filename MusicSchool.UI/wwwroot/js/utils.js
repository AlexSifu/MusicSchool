/**
 * Convierte una fecha ISO string a formato dd/mm/yyyy y calcula la edad exacta
 * @param {string} fechaISO - Fecha en formato ISO (ej: "2000-06-22T00:00:00")
 * @returns {Object} - Objeto con fecha formateada y edad
 */
export const convertirFechaYEdad = (fechaISO) => {
    if (!fechaISO) {
        throw new Error('La fecha es requerida');
    }

    const fecha = new Date(fechaISO);

    // Validar que la fecha sea válida
    if (isNaN(fecha.getTime())) {
        throw new Error('Formato de fecha inválido');
    }

    const hoy = new Date();

    // Formatear fecha a dd/mm/yyyy
    const dia = fecha.getDate().toString().padStart(2, '0');
    const mes = (fecha.getMonth() + 1).toString().padStart(2, '0');
    const año = fecha.getFullYear();
    const fechaFormateada = `${dia}/${mes}/${año}`;

    // Calcular edad exacta
    let edad = hoy.getFullYear() - fecha.getFullYear();
    const mesActual = hoy.getMonth();
    const diaActual = hoy.getDate();
    const mesNacimiento = fecha.getMonth();
    const diaNacimiento = fecha.getDate();

    // Si no ha llegado el mes de cumpleaños o si es el mes pero no el día
    if (mesActual < mesNacimiento || (mesActual === mesNacimiento && diaActual < diaNacimiento)) {
        edad--;
    }

    return {
        fechaFormateada,
        edad
    };
};

// Función alternativa que retorna solo la fecha formateada
export const formatearFecha = (fechaISO) => {
    if (!fechaISO) {
        throw new Error('La fecha es requerida');
    }

    const fecha = new Date(fechaISO);

    if (isNaN(fecha.getTime())) {
        throw new Error('Formato de fecha inválido');
    }

    const dia = fecha.getDate().toString().padStart(2, '0');
    const mes = (fecha.getMonth() + 1).toString().padStart(2, '0');
    const año = fecha.getFullYear();

    return `${dia}/${mes}/${año}`;
};

// Función alternativa que retorna solo la edad
export const calcularEdad = (fechaISO) => {
    if (!fechaISO) {
        throw new Error('La fecha es requerida');
    }

    const fecha = new Date(fechaISO);

    if (isNaN(fecha.getTime())) {
        throw new Error('Formato de fecha inválido');
    }

    const hoy = new Date();
    let edad = hoy.getFullYear() - fecha.getFullYear();
    const mesActual = hoy.getMonth();
    const diaActual = hoy.getDate();
    const mesNacimiento = fecha.getMonth();
    const diaNacimiento = fecha.getDate();

    if (mesActual < mesNacimiento || (mesActual === mesNacimiento && diaActual < diaNacimiento)) {
        edad--;
    }

    return edad;
};

// Función para convertir a formato yyyy-mm-dd
export const formatearFechaISO = (fechaISO) => {
    if (!fechaISO) {
        throw new Error('La fecha es requerida');
    }

    const fecha = new Date(fechaISO);

    if (isNaN(fecha.getTime())) {
        throw new Error('Formato de fecha inválido');
    }

    const año = fecha.getFullYear();
    const mes = (fecha.getMonth() + 1).toString().padStart(2, '0');
    const dia = fecha.getDate().toString().padStart(2, '0');

    return `${año}-${mes}-${dia}`;
};