import Container from "react-bootstrap/Container";

function PageContainer({
  title,
  subtitle,
  children,
  className = "",
  titleClassName = "text-light",
  subtitleClassName = "text-light-emphasis",
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
