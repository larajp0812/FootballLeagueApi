import Container from "react-bootstrap/Container";

function PageContainer({ title, subtitle, children }) {
  return (
    <Container className="py-4">
      <h1 className="h3 mb-1">{title}</h1>
      {subtitle ? <p className="text-muted mb-4">{subtitle}</p> : null}
      {children}
    </Container>
  );
}

export default PageContainer;
