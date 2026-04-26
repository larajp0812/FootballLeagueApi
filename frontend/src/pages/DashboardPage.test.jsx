import { render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";
import DashboardPage from "./DashboardPage";

vi.mock("../contexts/AuthContext", () => ({
  useAuth: vi.fn(),
}));

vi.mock("../services/healthService", () => ({
  getApiHealth: vi.fn(),
}));

import { useAuth } from "../contexts/AuthContext";
import { getApiHealth } from "../services/healthService";

function renderPage() {
  return render(
    <MemoryRouter>
      <DashboardPage />
    </MemoryRouter>,
  );
}

describe("DashboardPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("shows API online status when health endpoint succeeds", async () => {
    useAuth.mockReturnValue({ role: "User" });
    getApiHealth.mockResolvedValue(true);

    renderPage();

    await waitFor(() => {
      expect(screen.getByText(/api status: online/i)).toBeInTheDocument();
    });
  });

  it("shows API unreachable status when health endpoint fails", async () => {
    useAuth.mockReturnValue({ role: "User" });
    getApiHealth.mockRejectedValue(new Error("offline"));

    renderPage();

    await waitFor(() => {
      expect(
        screen.getByText(/api status: unreachable\. some actions may be unavailable\./i),
      ).toBeInTheDocument();
    });
  });

  it("hides roles module for non-admin users", () => {
    useAuth.mockReturnValue({ role: "User" });
    getApiHealth.mockResolvedValue(true);

    renderPage();

    expect(screen.queryByText(/open roles/i)).not.toBeInTheDocument();
  });

  it("shows roles module for admin users", () => {
    useAuth.mockReturnValue({ role: "Admin" });
    getApiHealth.mockResolvedValue(true);

    renderPage();

    expect(screen.getByText(/open roles/i)).toBeInTheDocument();
  });
});
