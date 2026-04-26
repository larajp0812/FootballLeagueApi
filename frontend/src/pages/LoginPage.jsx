import { useState } from "react";
import { useLocation, useNavigate, Link } from "react-router-dom";
import Alert from "react-bootstrap/Alert";
import Button from "react-bootstrap/Button";
import Card from "react-bootstrap/Card";
import Col from "react-bootstrap/Col";
import Form from "react-bootstrap/Form";
import Row from "react-bootstrap/Row";
import ErrorAlert from "../components/ErrorAlert";
import LoadingState from "../components/LoadingState";
import PageContainer from "../components/PageContainer";
import { useAuth } from "../contexts/AuthContext";

function LoginPage() {
  const { login, loading, error, clearError } = useAuth();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();
  const location = useLocation();
  const redirectTo = location.state?.from?.pathname || "/";
  const query = new URLSearchParams(location.search);
  const registered = query.get("registered") === "1";
  const confirmed = query.get("confirmed");

  async function handleSubmit(event) {
    event.preventDefault();
    await login(email, password);
    navigate(redirectTo, { replace: true });
  }

  return (
    <PageContainer
      title="Login"
      className="app-page auth-page"
      titleClassName="text-light text-center"
    >
      <Row className="justify-content-center">
        <Col xs={12} md={8} lg={6}>
          <Card className="auth-card">
            <Card.Body>
              {registered ? (
                <Alert variant="info" className="mb-3">
                  Registration successful. Please check your email for the
                  confirmation link.
                </Alert>
              ) : null}

              {confirmed === "1" ? (
                <Alert variant="success" className="mb-3">
                  Your email has been confirmed. You can log in now.
                </Alert>
              ) : null}

              {confirmed === "0" ? (
                <Alert variant="warning" className="mb-3">
                  Email confirmation link is invalid or expired.
                </Alert>
              ) : null}

              <ErrorAlert message={error} onClose={clearError} />
              <Form onSubmit={handleSubmit}>
                <Form.Group className="mb-3" controlId="email">
                  <Form.Label>Email</Form.Label>
                  <Form.Control
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="Enter your email"
                    required
                  />
                </Form.Group>

                <Form.Group className="mb-3" controlId="password">
                  <Form.Label>Password</Form.Label>
                  <Form.Control
                    type="password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                  />
                </Form.Group>

                <Button type="submit" disabled={loading} className="w-100">
                  {loading ? "Signing in..." : "Login"}
                </Button>
              </Form>

              {loading ? (
                <LoadingState message="Authenticating user..." />
              ) : null}

              <p className="mt-3 mb-0 text-center">
                No account yet? <Link to="/register">Create one</Link>
              </p>
              <p className="mt-2 mb-0 text-center">
                <Link to="/forgot-password">Forgot password?</Link>
              </p>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </PageContainer>
  );
}

export default LoginPage;
