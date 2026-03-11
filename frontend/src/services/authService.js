import { apiRequest } from "./apiClient";

export function registerUser({ userName, email, password }) {
  return apiRequest("/api/auth/register", {
    method: "POST",
    body: JSON.stringify({ userName, email, password }),
  });
}

export function loginUser({ email, password }) {
  return apiRequest("/api/auth/login", {
    method: "POST",
    body: JSON.stringify({ email, password }),
  });
}

export function forgotPassword({ email }) {
  return apiRequest("/api/auth/forgot-password", {
    method: "POST",
    body: JSON.stringify({ email }),
  });
}

export function resetPassword({ email, token, newPassword }) {
  return apiRequest("/api/auth/reset-password", {
    method: "POST",
    body: JSON.stringify({ email, token, newPassword }),
  });
}
