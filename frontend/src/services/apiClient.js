/**
 * API Base URL configuration
 * Reads from VITE_API_BASE_URL environment variable or defaults to localhost backend
 * @type {string}
 */
const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? "https://localhost:5240";

const tokenStorageKey = "football_token";
const roleStorageKey = "football_role";
const unauthorizedEventName = "auth:unauthorized";

/**
 * Parse API response based on status code and content type
 * @private
 * @param {Response} response - Fetch API response object
 * @returns {Promise<any>} Parsed JSON response or null
 * @description
 *   - Returns null for 204 No Content status
 *   - Attempts JSON parsing, falls back to raw text if JSON parse fails
 *   - Returns null if response has no body
 */
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

/**
 * Normalize API error messages from various response formats
 * @private
 * @param {number} status - HTTP status code
 * @param {any} payload - Response body/payload
 * @returns {string} Human-readable error message
 * @description
 *   Extracts error messages from multiple backend error response formats:
 *   - Arrays of error objects
 *   - Nested validation error objects
 *   - Title/message/details fields
 *   - Generic status-based fallback
 */
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

/**
 * Make authenticated API request to backend
 * @public
 * @param {string} path - API endpoint path (e.g., "/api/teams")
 * @param {Object} [options={}] - Fetch options
 * @param {string} [options.method="GET"] - HTTP method
 * @param {string} [options.body] - Request body (JSON string)
 * @param {Object} [options.headers={}] - Additional headers
 * @returns {Promise<any>} Parsed response data
 * @throws {Error} Error object with status code and payload on failure
 * @description
 *   - Automatically includes JWT token in Authorization header
 *   - Handles 401 errors by clearing stored token and dispatching logout event
 *   - Normalizes error messages from backend
 *   - Handles various response content types
 *
 * @example
 * const teams = await apiRequest('/api/teams');
 * await apiRequest('/api/teams', {
 *   method: 'POST',
 *   body: JSON.stringify({ name: 'New Team' })
 * });
 */
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
