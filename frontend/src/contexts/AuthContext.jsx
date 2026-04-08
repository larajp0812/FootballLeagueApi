/* eslint-disable react-refresh/only-export-components */
import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { loginUser, registerUser } from "../services/authService";

export const AuthContext = createContext(null);

const tokenStorageKey = "football_token";
const roleStorageKey = "football_role";
const unauthorizedEventName = "auth:unauthorized";

function parseJwt(token) {
  try {
    const base64Url = token.split(".")[1];
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    return JSON.parse(window.atob(base64));
  } catch {
    return null;
  }
}

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
    if (!token) {
      localStorage.removeItem(tokenStorageKey);
      localStorage.removeItem(roleStorageKey);
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
      const response = await loginUser({ email: email.trim(), password });
      setToken(response.token);
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
