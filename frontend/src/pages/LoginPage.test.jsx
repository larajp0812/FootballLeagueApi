import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";
import LoginPage from "./LoginPage";

const mockNavigate = vi.fn();

vi.mock("../contexts/AuthContext", () => ({
  useAuth: vi.fn(),
}));

vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

import { useAuth } from "../contexts/AuthContext";

function renderPage(initialEntry = "/login") {
  return render(
    <MemoryRouter initialEntries={[initialEntry]}>
      <LoginPage />
    </MemoryRouter>,
  );
}

describe("LoginPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();

    useAuth.mockReturnValue({
      login: vi.fn().mockResolvedValue({ token: "token" }),
      loading: false,
      error: "",
      clearError: vi.fn(),
    });
  });

  it("shows registration success notice when registered=1", () => {
    renderPage("/login?registered=1");

    expect(
      screen.getByText(/registration successful\. please check your email/i),
    ).toBeInTheDocument();
  });

  it("shows email confirmation success notice when confirmed=1", () => {
    renderPage("/login?confirmed=1");

    expect(
      screen.getByText(/your email has been confirmed\. you can log in now\./i),
    ).toBeInTheDocument();
  });

  it("submits credentials and redirects", async () => {
    const loginMock = vi.fn().mockResolvedValue({ token: "token" });
    useAuth.mockReturnValue({
      login: loginMock,
      loading: false,
      error: "",
      clearError: vi.fn(),
    });

    renderPage("/login");

    fireEvent.change(screen.getByLabelText(/email/i), {
      target: { value: "user@example.com" },
    });
    fireEvent.change(screen.getByLabelText(/password/i), {
      target: { value: "Password123!" },
    });
    fireEvent.click(screen.getByRole("button", { name: /login/i }));

    await waitFor(() => {
      expect(loginMock).toHaveBeenCalledWith("user@example.com", "Password123!");
      expect(mockNavigate).toHaveBeenCalledWith("/", { replace: true });
    });
  });
});
