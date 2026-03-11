import Container from "react-bootstrap/Container";

function PageContainer({
  title,
  subtitle,
  children,
  className = "",
  titleClassName = "text-light",
  subtitleClassName = "text-light-emphasis",
}) {
  return (
    <Container className={`py-4 ${className}`.trim()}>
      <h1 className={`h3 mb-1 ${titleClassName}`.trim()}>{title}</h1>
      {subtitle ? (
        <p className={`${subtitleClassName} mb-4`.trim()}>{subtitle}</p>
      ) : null}
      {children}
    </Container>
  );
}

export default PageContainer;
