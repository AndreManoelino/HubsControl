const API_URL = "http://localhost:5162/api";
// const API_URL = import.meta.env.VITE_API_URL;
console.log("API_URL:", API_URL);

async function request(endpoint, options = {}) {
    const token = localStorage.getItem("controlhub_token");

    const response = await fetch(`${API_URL}${endpoint}`, {
        ...options,
        headers: {
            "Content-Type": "application/json",

            ...(token
                ? { Authorization: `Bearer ${token}` }
                : {}),

            ...(options.headers || {}),
        },
    });

    const contentType = response.headers.get("content-type");

    const data = contentType?.includes("application/json")
        ? await response.json()
        : null;

    if (!response.ok) {
        throw new Error(
            data?.mensagem ||
            data?.message ||
            "Ocorreu um erro na comunicação com a API."
        );
    }

    return data;
}

export const api = {
    get: (endpoint) =>
        request(endpoint, {
            method: "GET",
        }),

    post: (endpoint, body) =>
        request(endpoint, {
            method: "POST",
            body: JSON.stringify(body),
        }),

    put: (endpoint, body) =>
        request(endpoint, {
            method: "PUT",
            body: JSON.stringify(body),
        }),

    delete: (endpoint) =>
        request(endpoint, {
            method: "DELETE",
        }),
};