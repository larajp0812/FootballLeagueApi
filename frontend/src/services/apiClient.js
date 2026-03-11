const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? "https://localhost:7195";

const tokenStorageKey = "football_token";
const roleStorageKey = "football_role";
const unauthorizedEventName = "auth:unauthorized";

async function parseResponse(response) {
  if (response.status === 204) {
    return null;
  }

  const text = await response.text();
  if (!text) {
    return null;
  }

  try {
    return JSON.parse(text);
  } catch {
    return text;
  }
}

function normalizeError(status, payload) {
  if (!payload) {
    return `Request failed with status ${status}`;
  }

  if (typeof payload === "string") {
    return payload;
  }

  if (Array.isArray(payload)) {
    return payload
      .map((err) => err.description ?? JSON.stringify(err))
      .join(", ");
  }

  if (payload.title) {
    return payload.title;
  }

  if (payload.error) {
    return payload.error;
  }

  if (payload.message) {
    return payload.message;
  }

  if (payload.errors && typeof payload.errors === "object") {
    const values = Object.values(payload.errors).flat();
    if (values.length > 0) return values.join(", ");
  }

  return `Request failed with status ${status}`;
}

export async function apiRequest(path, options = {}) {
  const token = localStorage.getItem(tokenStorageKey);
  const headers = {
    "Content-Type": "application/json",
    ...(options.headers ?? {}),
  };

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers,
  });

  const payload = await parseResponse(response);

  if (!response.ok) {
    if (response.status === 401) {
      localStorage.removeItem(tokenStorageKey);
      localStorage.removeItem(roleStorageKey);
      window.dispatchEvent(new Event(unauthorizedEventName));
    }

    const error = new Error(normalizeError(response.status, payload));
    error.status = response.status;
    error.payload = payload;
    throw error;
  }

  return payload;
}
