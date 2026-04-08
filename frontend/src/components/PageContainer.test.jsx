import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import PageContainer from "./PageContainer";

describe("PageContainer", () => {
  it("renders title", () => {
    render(
      <PageContainer title="Test Title">
        <div>Content</div>
      </PageContainer>,
    );

    expect(screen.getByText("Test Title")).toBeInTheDocument();
  });

  it("renders subtitle when provided", () => {
    render(
      <PageContainer title="Title" subtitle="Test Subtitle">
        <div>Content</div>
      </PageContainer>,
    );

    expect(screen.getByText("Test Subtitle")).toBeInTheDocument();
  });

  it("renders children content", () => {
    render(
      <PageContainer title="Title">
        <div>Child Content</div>
      </PageContainer>,
    );

    expect(screen.getByText("Child Content")).toBeInTheDocument();
  });

  it("hides header when hideHeader prop is true", () => {
    render(
      <PageContainer title="Title" hideHeader={true}>
        <div>Content</div>
      </PageContainer>,
    );

    expect(screen.queryByText("Title")).not.toBeInTheDocument();
    expect(screen.getByText("Content")).toBeInTheDocument();
  });

  it("applies custom className", () => {
    const { container } = render(
      <PageContainer title="Title" className="custom-class">
        <div>Content</div>
      </PageContainer>,
    );

    // Find the Container - it should have the custom class
    const containerElement = container.querySelector(".custom-class");
    expect(containerElement).toBeInTheDocument();
  });
});
