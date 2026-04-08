/**
 * API Base URL configuration
 * Reads from VITE_API_BASE_URL environment variable or defaults to localhost backend
 * @type {string}
 */\nconst API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? "https://localhost:7195";

const tokenStorageKey = "football_token";
const roleStorageKey = "football_role";
const unauthorizedEventName = "auth:unauthorized";

/**\n * Parse API response based on status code and content type\n * @private\n * @param {Response} response - Fetch API response object\n * @returns {Promise<any>} Parsed JSON response or null\n * @description\n *   - Returns null for 204 No Content status\n *   - Attempts JSON parsing, falls back to raw text if JSON parse fails\n *   - Returns null if response has no body\n */\nasync function parseResponse(response) {
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

/**\n * Normalize API error messages from various response formats\n * @private\n * @param {number} status - HTTP status code\n * @param {any} payload - Response body/payload\n * @returns {string} Human-readable error message\n * @description\n *   Extracts error messages from multiple backend error response formats:\n *   - Arrays of error objects\n *   - Nested validation error objects\n *   - Title/message/details fields\n *   - Generic status-based fallback\n */\nfunction normalizeError(status, payload) {
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

  if (payload.errors && typeof payload.errors === "object") {
    const values = Object.values(payload.errors).flat();
    if (values.length > 0) return values.join(", ");
  }

  if (payload.title) {
    return payload.title;
  }

  if (payload.error) {
    return payload.error;
  }

  if (payload.message) {
    if (
      payload.message ===
        "An internal server error occurred. Please try again later." &&
      payload.details
    ) {
      return payload.details;
    }

    return payload.message;
  }

  if (payload.details) {
    return payload.details;
  }

  return `Request failed with status ${status}`;
}

/**\n * Make authenticated API request to backend\n * @public\n * @param {string} path - API endpoint path (e.g., \"/api/teams\")\n * @param {Object} [options={}] - Fetch options\n * @param {string} [options.method=\"GET\"] - HTTP method\n * @param {string} [options.body] - Request body (JSON string)\n * @param {Object} [options.headers={}] - Additional headers\n * @returns {Promise<any>} Parsed response data\n * @throws {Error} Error object with status code and payload on failure\n * @description\n *   - Automatically includes JWT token in Authorization header\n *   - Handles 401 errors by clearing stored token and dispatching logout event\n *   - Normalizes error messages from backend\n *   - Handles various response content types\n * \n * @example\n * const teams = await apiRequest('/api/teams');\n * await apiRequest('/api/teams', {\n *   method: 'POST',\n *   body: JSON.stringify({ name: 'New Team' })\n * });\n */\nexport async function apiRequest(path, options = {}) {
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
