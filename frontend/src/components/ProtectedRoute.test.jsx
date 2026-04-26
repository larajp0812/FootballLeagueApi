import { render, screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { describe, expect, it } from "vitest";
import ProtectedRoute from "./ProtectedRoute";
import { AuthContext } from "../contexts/AuthContext";

function renderProtectedRoute({ isAuthenticated }) {
  return render(
    <AuthContext.Provider
      value={{
        token: isAuthenticated ? "test-token" : null,
        role: "User",
        isAuthenticated,
        loading: false,
        error: "",
        login: async () => ({ token: "test-token" }),
        register: async () => ({}),
        logout: () => {},
        clearError: () => {},
      }}
    >
      <MemoryRouter initialEntries={["/protected"]}>
        <Routes>
          <Route
            path="/protected"
            element={
              <ProtectedRoute>
                <div>Protected Content</div>
              </ProtectedRoute>
            }
          />
          <Route path="/login" element={<div>Login Page</div>} />
        </Routes>
      </MemoryRouter>
    </AuthContext.Provider>,
  );
}

describe("ProtectedRoute", () => {
  it("renders children when user is authenticated", () => {
    renderProtectedRoute({ isAuthenticated: true });

    expect(screen.getByText("Protected Content")).toBeInTheDocument();
  });

  it("redirects to login when user is not authenticated", () => {
    renderProtectedRoute({ isAuthenticated: false });

    // When redirected, protected content should not be visible.
    expect(screen.queryByText("Protected Content")).not.toBeInTheDocument();
    expect(screen.getByText("Login Page")).toBeInTheDocument();
  });
});
