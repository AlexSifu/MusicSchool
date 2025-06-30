// wwwroot/js/auth-ui.js
import { getToken } from "./auth.js";

document.addEventListener("DOMContentLoaded", () => {
    const navbar = document.getElementById("navbarLinks");
    const token = getToken();

    if (!navbar) return;

    navbar.innerHTML = "";

    if (token) {
        navbar.innerHTML += `
            <li class="nav-item">
                <a class="nav-link text-dark" href="/Students">Estudiantes</a>
            </li>
            <li class="nav-item">
                <a class="nav-link text-dark" href="/Teachers">Profesores</a>
            </li>
            <li class="nav-item">
                <a class="nav-link text-dark" href="/Schools">Escuelas</a>
            </li>
            <li class="nav-item">
                <a class="nav-link text-dark" href="/Inscriptions">Inscripciones</a>
            </li>
            <li class="nav-item">
                <a class="nav-link text-dark" href="#" id="logoutLink">Cerrar sesión</a>
            </li>
        `;

        document.getElementById("logoutLink").addEventListener("click", (e) => {
            e.preventDefault();
            localStorage.removeItem("token");
            window.location.href = "/Login";
        });

    } else {
        navbar.innerHTML = `
            <li class="nav-item">
                <a class="nav-link text-dark" href="/Login">Iniciar sesión</a>
            </li>
        `;
    }
});
