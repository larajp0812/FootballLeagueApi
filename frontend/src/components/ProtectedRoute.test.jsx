import { render, screen } from "@testing-library/react";
import { BrowserRouter } from "react-router-dom";
import { describe, expect, it, vi } from "vitest";
import ProtectedRoute from "./ProtectedRoute";

// Mock the useAuth hook
vi.mock("../contexts/AuthContext", () => ({
  useAuth: vi.fn(),
}));

import { useAuth } from "../contexts/AuthContext";

describe("ProtectedRoute", () => {
  it("renders children when user is authenticated", () => {
    useAuth.mockReturnValue({
      isAuthenticated: true,
      token: "test-token",
    });

    render(
      <BrowserRouter>
        <ProtectedRoute>
          <div>Protected Content</div>
        </ProtectedRoute>
      </BrowserRouter>,
    );

    expect(screen.getByText("Protected Content")).toBeInTheDocument();
  });

  it("redirects to login when user is not authenticated", () => {
    useAuth.mockReturnValue({
      isAuthenticated: false,
      token: null,
    });

    render(
      <BrowserRouter>
        <ProtectedRoute>
          <div>Protected Content</div>
        </ProtectedRoute>
      </BrowserRouter>,
    );

    // When redirected, protected content should not be visible
    expect(screen.queryByText("Protected Content")).not.toBeInTheDocument();
  });
});
