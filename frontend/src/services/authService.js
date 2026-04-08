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
 * Change user password when already authenticated
 * @param {Object} payload - Password change information
 * @param {string} payload.currentPassword - Current password for verification
 * @param {string} payload.newPassword - New password to set
 * @returns {Promise<Object>} Response from the server
 * @throws {Error} If current password is incorrect or password change fails
 */
export function changePassword({ currentPassword, newPassword }) {
  return apiRequest("/api/auth/change-password", {
    method: "POST",
    body: JSON.stringify({ currentPassword, newPassword }),
  });
}
