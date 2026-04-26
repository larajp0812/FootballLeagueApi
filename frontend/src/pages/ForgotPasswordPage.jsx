import { useMemo, useState } from "react";
import { Link, useLocation } from "react-router-dom";
import Alert from "react-bootstrap/Alert";
import Button from "react-bootstrap/Button";
import Card from "react-bootstrap/Card";
import Col from "react-bootstrap/Col";
import Form from "react-bootstrap/Form";
import Row from "react-bootstrap/Row";
import ErrorAlert from "../components/ErrorAlert";
import LoadingState from "../components/LoadingState";
import PageContainer from "../components/PageContainer";
import { forgotPassword } from "../services/authService";

function ForgotPasswordPage() {
  const location = useLocation();
  const query = useMemo(() => new URLSearchParams(location.search), [location]);
  const emailFromQuery = query.get("email") ?? "";

  const [email, setEmail] = useState(emailFromQuery);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  async function handleSubmit(event) {
    event.preventDefault();
    setLoading(true);
    setError("");
    setMessage("");

    try {
      const response = await forgotPassword({ email: email.trim() });
      setMessage(
        response?.message ||
          "If an account exists for that email, a reset link has been sent.",
      );
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <PageContainer
      title="Forgot Password"
      className="app-page auth-page"
      titleClassName="text-light text-center"
    >
      <Row className="justify-content-center">
        <Col xs={12} md={8} lg={6}>
          <Card className="auth-card">
            <Card.Body>
              <ErrorAlert message={error} onClose={() => setError("")} />
              {message ? <Alert variant="info">{message}</Alert> : null}

              <Form onSubmit={handleSubmit}>
                <Form.Group className="mb-3" controlId="forgotEmail">
                  <Form.Label>Email</Form.Label>
                  <Form.Control
                    type="text"
                    inputMode="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                  />
                </Form.Group>

                <Button type="submit" disabled={loading} className="w-100">
                  {loading ? "Sending..." : "Send Reset Link"}
                </Button>
              </Form>

              {loading ? (
                <LoadingState message="Sending reset link..." />
              ) : null}

              <p className="mt-3 mb-0 text-center">
                Remembered it? <Link to="/login">Back to login</Link>
              </p>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </PageContainer>
  );
}

export default ForgotPasswordPage;
