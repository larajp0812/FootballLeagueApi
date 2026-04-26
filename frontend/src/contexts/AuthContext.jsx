/* eslint-disable react-refresh/only-export-components */
import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { loginUser, refreshAuthToken, registerUser } from "../services/authService";

export const AuthContext = createContext(null);

const tokenStorageKey = "football_token";
const roleStorageKey = "football_role";
const refreshTokenStorageKey = "football_refresh_token";
const userEmailStorageKey = "football_user_email";
const unauthorizedEventName = "auth:unauthorized";

/**
 * Decode JWT token payload (without verification)
 * @private
 * @param {string} token - JWT token string
 * @returns {Object|null} Decoded JWT payload or null if invalid
 */
function parseJwt(token) {
  try {
    const base64Url = token.split(".")[1];
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    return JSON.parse(window.atob(base64));
  } catch {
    return null;
  }
}

/**
 * Extract user role from JWT token
 * @private
 * @param {string} token - JWT token string
 * @returns {string} User role (e.g., "Admin", "User") or default "User"
 * @description
 *   Checks multiple claim names for role information:
 *   - Simple "role" claim
 *   - Azure/Microsoft identity framework role claim URI
 */
function getRoleFromToken(token) {
  const payload = parseJwt(token);
  if (!payload) return "User";

  const roleClaim =
    payload.role ||
    payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

  if (Array.isArray(roleClaim)) {
    return roleClaim[0] ?? "User";
  }

  return roleClaim ?? "User";
}

/**
 * AuthProvider Component - Global authentication state provider
 *
 * Manages authentication state, JWT tokens, and user roles across the app.
 * Provides login, register, and logout functionality through Context.
 * Handles token persistence and automatic session timeout.
 *
 * Must wrap the entire application to provide authentication context.
 *
 * @component
 * @param {Object} props
 * @param {React.ReactNode} props.children - App components to wrap
 * @returns {React.ReactElement} Context provider wrapping children
 *
 * @example
 * // In main.jsx or root component
 * <AuthProvider>
 *   <App />
 * </AuthProvider>
 */
export function AuthProvider({ children }) {
  const [token, setToken] = useState(() =>
    localStorage.getItem(tokenStorageKey),
  );
  const [role, setRole] = useState(
    () => localStorage.getItem(roleStorageKey) ?? "User",
  );
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    let isActive = true;

    async function restoreSession() {
      const existingToken = localStorage.getItem(tokenStorageKey);
      const savedRefreshToken = localStorage.getItem(refreshTokenStorageKey);
      const savedEmail = localStorage.getItem(userEmailStorageKey);

      if (!existingToken || !savedRefreshToken || !savedEmail) {
        return;
      }

      try {
        const refreshed = await refreshAuthToken({
          email: savedEmail,
          refreshToken: savedRefreshToken,
        });

        if (!isActive) {
          return;
        }

        if (refreshed?.token) {
          setToken(refreshed.token);
        }

        if (refreshed?.refreshToken) {
          localStorage.setItem(refreshTokenStorageKey, refreshed.refreshToken);
        }
      } catch {
        if (!isActive) {
          return;
        }

        setToken(null);
      }
    }

    restoreSession();

    return () => {
      isActive = false;
    };
  }, []);

  useEffect(() => {
    if (!token) {
      localStorage.removeItem(tokenStorageKey);
      localStorage.removeItem(roleStorageKey);
      localStorage.removeItem(refreshTokenStorageKey);
      localStorage.removeItem(userEmailStorageKey);
      setRole("User");
      return;
    }

    const derivedRole = getRoleFromToken(token);
    localStorage.setItem(tokenStorageKey, token);
    localStorage.setItem(roleStorageKey, derivedRole);
    setRole(derivedRole);
  }, [token]);

  useEffect(() => {
    function handleUnauthorized() {
      setToken(null);
      setError("Session expired. Please log in again.");
      localStorage.removeItem(refreshTokenStorageKey);
      localStorage.removeItem(userEmailStorageKey);
    }

    window.addEventListener(unauthorizedEventName, handleUnauthorized);

    return () => {
      window.removeEventListener(unauthorizedEventName, handleUnauthorized);
    };
  }, []);

  async function login(email, password) {
    setLoading(true);
    setError("");

    try {
      const normalizedEmail = email.trim();
      const response = await loginUser({ email: normalizedEmail, password });
      setToken(response.token);

      if (response?.refreshToken) {
        localStorage.setItem(refreshTokenStorageKey, response.refreshToken);
        localStorage.setItem(userEmailStorageKey, normalizedEmail);
      }

      return response;
    } catch (err) {
      setError(err.message);
      throw err;
    } finally {
      setLoading(false);
    }
  }

  async function register(userName, email, password) {
    setLoading(true);
    setError("");

    try {
      const response = await registerUser({
        userName: userName.trim(),
        email: email.trim(),
        password,
      });
      return response;
    } catch (err) {
      setError(err.message);
      throw err;
    } finally {
      setLoading(false);
    }
  }

  function logout() {
    setToken(null);
    setError("");
  }

  const value = useMemo(
    () => ({
      token,
      role,
      isAuthenticated: Boolean(token),
      loading,
      error,
      login,
      register,
      logout,
      clearError: () => setError(""),
    }),
    [token, role, loading, error],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return context;
}
