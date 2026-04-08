import { useContext } from "react";
import { AuthContext } from "../contexts/AuthContext";

/**
 * useAuth Hook - Access authentication context
 *
 * Custom hook that provides access to the authentication context.
 * Returns the current authentication state and auth methods.
 * Must be called within an AuthProvider component.
 *
 * @returns {Object} Authentication context value containing:
 *   - {string} token - JWT authentication token
 *   - {string} role - User role (e.g., "User", "Admin")
 *   - {boolean} isAuthenticated - Whether user is logged in
 *   - {boolean} loading - Whether an auth operation is in progress
 *   - {string} error - Current error message, if any
 *   - {Function} login - Function to log in user
 *   - {Function} register - Function to register new user
 *   - {Function} logout - Function to log out user
 *   - {Function} clearError - Function to clear error message
 * @throws {Error} If not called within AuthProvider
 *
 * @example
 * function MyComponent() {
 *   const { token, role, login, logout } = useAuth();
 *   // Use auth state and methods
 * }
 */
export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return context;
}
