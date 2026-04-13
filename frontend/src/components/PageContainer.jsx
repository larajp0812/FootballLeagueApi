import Container from "react-bootstrap/Container";

/**
 * PageContainer Component - Consistent page layout wrapper
 *
 * Wraps page content with standard Bootstrap container and optional header.
 * Provides consistent spacing, titles, and styling across all pages.
 *
 * @component
 * @param {Object} props - Component props
 * @param {React.ReactNode} props.children - Page content to wrap
 * @param {string} props.title - Main page title (required)
 * @param {string} [props.subtitle] - Optional subtitle to display below main title
 * @param {string} [props.className=""] - Additional CSS classes for the container
 * @param {string} [props.titleClassName="text-light"] - CSS classes for the title element
 * @param {string} [props.subtitleClassName="text-light-emphasis"] - CSS classes for subtitle
 * @param {boolean} [props.hideHeader=false] - Hide the title header entirely
 * @returns {React.ReactElement} Container with header and children
 *
 * @example
 * <PageContainer
 *   title="Teams"
 *   subtitle="Manage league teams"
 *   className="app-page"
 * >
 *   <TeamsContent />
 * </PageContainer>
 */
function PageContainer({
  title,
  subtitle,
  children,
  className = "",
  titleClassName = "text-light",
  // subtitleClassName = "text-light-emphasis",
  hideHeader = false,
}) {
  const hasSubtitle = Boolean(subtitle);
  const titleSpacingClass = hasSubtitle ? "mb-1" : "mb-4";

  return (
    <Container className={`py-4 ${className}`.trim()}>
      {!hideHeader ? (
        <>
          <h1 className={`h3 ${titleSpacingClass} ${titleClassName}`.trim()}>
            {title}
          </h1>
        </>
      ) : null}
      {children}
    </Container>
  );
}

export default PageContainer;
