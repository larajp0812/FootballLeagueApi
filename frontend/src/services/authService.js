import { apiRequest } from "./apiClient";

/**
 * Register a new user account
 * @param {Object} credentials - User registration credentials
 * @param {string} credentials.userName - Desired username
 * @param {string} credentials.email - User email address
 * @param {string} credentials.password - User password
 * @returns {Promise<Object>} Response containing user data and authentication token
 * @throws {Error} If registration fails (duplicate email, validation errors, etc.)
 */
export function registerUser({ userName, email, password }) {
  return apiRequest("/api/auth/register", {
    method: "POST",
    body: JSON.stringify({ userName, email, password }),
  });
}

/**
 * Authenticate user with email and password, returns JWT token
 * @param {Object} credentials - Login credentials
 * @param {string} credentials.email - User email address
 * @param {string} credentials.password - User password
 * @returns {Promise<Object>} Response containing JWT token and user role
 * @throws {Error} If authentication fails (invalid credentials, user not found, etc.)
 */
export function loginUser({ email, password }) {
  return apiRequest("/api/auth/login", {
    method: "POST",
    body: JSON.stringify({ email, password }),
  });
}

/**
 * Request password reset email for forgotten passwords
 * @param {Object} payload - Password recovery request
 * @param {string} payload.email - Email address of the account
 * @returns {Promise<Object>} Response from the server
 * @throws {Error} If email not found or reset request fails
 */
export function forgotPassword({ email }) {
  return apiRequest("/api/auth/forgot-password", {
    method: "POST",
    body: JSON.stringify({ email }),
  });
}

/**
 * Reset user password using reset token from email
 * @param {Object} payload - Password reset information
 * @param {string} payload.email - User email address
 * @param {string} payload.token - Password reset token from email link
 * @param {string} payload.newPassword - New password to set
 * @returns {Promise<Object>} Response from the server
 * @throws {Error} If token is invalid/expired or password reset fails
 */
export function resetPassword({ email, token, newPassword }) {
  return apiRequest("/api/auth/reset-password", {
    method: "POST",
    body: JSON.stringify({ email, token, newPassword }),
  });
}

/**
 * Request a new access token and refresh token pair
 * @param {Object} payload - Refresh token request payload
 * @param {string} payload.email - User email address
 * @param {string} payload.refreshToken - Existing refresh token
 * @returns {Promise<Object>} New token pair and expiry details
 * @throws {Error} If refresh token is invalid or expired
 */
export function refreshAuthToken({ email, refreshToken }) {
  return apiRequest("/api/auth/refresh", {
    method: "POST",
    body: JSON.stringify({ email, refreshToken }),
  });
}
