// wwwroot/js/auth.js

export function isTokenExpired(token) {
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const now = Math.floor(Date.now() / 1000);
        return payload.exp < now;
    } catch {
        return true;
    }
}

export function getToken() {
    const token = localStorage.getItem("token");
    if (!token || isTokenExpired(token)) {
        localStorage.removeItem("token");
        return null;
    }
    return token;
}

export async function apiFetch(url, options = {}) {
    const token = getToken();

    if (!token) {
        window.location.href = "/Login";
        return null;
    }

    const response = await fetch(url, {
        ...options,
        headers: {
            ...(options.headers || {}),
            "Authorization": `Bearer ${token}`,
            "Content-Type": "application/json"
        }
    });

    if (response.status === 401) {
        localStorage.removeItem("token");
        window.location.href = "/Login";
        return null;
    }

    return response;
}
