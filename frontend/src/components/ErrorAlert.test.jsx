import { render, screen, fireEvent } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import ErrorAlert from "./ErrorAlert";

describe("ErrorAlert", () => {
  it("does not render when message is empty", () => {
    const { container } = render(<ErrorAlert message="" />);
    expect(container.firstChild).toBeNull();
  });

  it("does not render when message is undefined", () => {
    const { container } = render(<ErrorAlert />);
    expect(container.firstChild).toBeNull();
  });

  it("renders error message when provided", () => {
    render(<ErrorAlert message="An error occurred" />);
    expect(screen.getByText("An error occurred")).toBeInTheDocument();
  });

  it("calls onClose when dismiss button is clicked", () => {
    const mockOnClose = vi.fn();
    render(<ErrorAlert message="Error message" onClose={mockOnClose} />);

    const dismissButton = screen.getByRole("button");
    fireEvent.click(dismissButton);

    expect(mockOnClose).toHaveBeenCalled();
  });

  it("renders as dismissible alert when onClose is provided", () => {
    const mockOnClose = vi.fn();
    const { container } = render(
      <ErrorAlert message="Error" onClose={mockOnClose} />,
    );

    // Check that the alert has the dismissible class and contains a close button
    const alert = container.querySelector(".alert");
    expect(alert).toHaveClass("alert-dismissible");

    const closeButton = container.querySelector(".btn-close");
    expect(closeButton).toBeInTheDocument();
  });
});
