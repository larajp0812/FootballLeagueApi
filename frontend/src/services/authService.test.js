import { beforeEach, describe, expect, it, vi } from "vitest";
import * as authService from "./authService";

// Mock the apiRequest function
vi.mock("./apiClient", () => ({
  apiRequest: vi.fn(),
}));

import { apiRequest } from "./apiClient";

describe("authService", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("registerUser", () => {
    it("calls apiRequest with correct parameters", async () => {
      const mockResponse = { token: "test-token", userId: "123" };
      apiRequest.mockResolvedValue(mockResponse);

      const credentials = {
        userName: "testuser",
        email: "test@example.com",
        password: "password123",
      };

      const result = await authService.registerUser(credentials);

      expect(apiRequest).toHaveBeenCalledWith("/api/auth/register", {
        method: "POST",
        body: JSON.stringify(credentials),
      });
      expect(result).toEqual(mockResponse);
    });

    it("throws error on registration failure", async () => {
      const mockError = new Error("Email already in use");
      apiRequest.mockRejectedValue(mockError);

      const credentials = {
        userName: "testuser",
        email: "test@example.com",
        password: "password123",
      };

      await expect(authService.registerUser(credentials)).rejects.toThrow(
        "Email already in use",
      );
    });
  });

  describe("loginUser", () => {
    it("calls apiRequest with correct parameters", async () => {
      const mockResponse = { token: "test-token", role: "User" };
      apiRequest.mockResolvedValue(mockResponse);

      const credentials = {
        email: "test@example.com",
        password: "password123",
      };
      const result = await authService.loginUser(credentials);

      expect(apiRequest).toHaveBeenCalledWith("/api/auth/login", {
        method: "POST",
        body: JSON.stringify(credentials),
      });
      expect(result).toEqual(mockResponse);
    });

    it("throws error on login failure", async () => {
      const mockError = new Error("Invalid credentials");
      apiRequest.mockRejectedValue(mockError);

      const credentials = { email: "test@example.com", password: "wrong" };

      await expect(authService.loginUser(credentials)).rejects.toThrow(
        "Invalid credentials",
      );
    });
  });

  describe("forgotPassword", () => {
    it("sends password reset request", async () => {
      const mockResponse = { message: "Reset email sent" };
      apiRequest.mockResolvedValue(mockResponse);

      const payload = { email: "test@example.com" };
      const result = await authService.forgotPassword(payload);

      expect(apiRequest).toHaveBeenCalledWith("/api/auth/forgot-password", {
        method: "POST",
        body: JSON.stringify(payload),
      });
      expect(result).toEqual(mockResponse);
    });
  });

  describe("resetPassword", () => {
    it("calls apiRequest with reset credentials", async () => {
      const mockResponse = { message: "Password reset successful" };
      apiRequest.mockResolvedValue(mockResponse);

      const payload = {
        email: "test@example.com",
        token: "reset-token",
        newPassword: "newpass123",
      };

      const result = await authService.resetPassword(payload);

      expect(apiRequest).toHaveBeenCalledWith("/api/auth/reset-password", {
        method: "POST",
        body: JSON.stringify(payload),
      });
      expect(result).toEqual(mockResponse);
    });
  });

  describe("refreshAuthToken", () => {
    it("calls apiRequest with refresh token data", async () => {
      const mockResponse = {
        token: "new-token",
        refreshToken: "new-refresh-token",
      };
      apiRequest.mockResolvedValue(mockResponse);

      const payload = {
        email: "test@example.com",
        refreshToken: "existing-refresh-token",
      };

      const result = await authService.refreshAuthToken(payload);

      expect(apiRequest).toHaveBeenCalledWith("/api/auth/refresh", {
        method: "POST",
        body: JSON.stringify(payload),
      });
      expect(result).toEqual(mockResponse);
    });
  });
});
