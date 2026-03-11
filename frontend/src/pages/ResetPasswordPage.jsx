import { useMemo, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import Alert from "react-bootstrap/Alert";
import Button from "react-bootstrap/Button";
import Card from "react-bootstrap/Card";
import Col from "react-bootstrap/Col";
import Form from "react-bootstrap/Form";
import Row from "react-bootstrap/Row";
import ErrorAlert from "../components/ErrorAlert";
import LoadingState from "../components/LoadingState";
import PageContainer from "../components/PageContainer";
import { resetPassword } from "../services/authService";

function ResetPasswordPage() {
  const location = useLocation();
  const navigate = useNavigate();
  const query = useMemo(() => new URLSearchParams(location.search), [location]);
  const emailFromLink = query.get("email") ?? "";
  const tokenFromLink = query.get("token") ?? "";

  const [email, setEmail] = useState(emailFromLink);
  const [token, setToken] = useState(tokenFromLink);
  const [newPassword, setNewPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  async function handleSubmit(event) {
    event.preventDefault();
    setLoading(true);
    setError("");
    setMessage("");

    try {
      const response = await resetPassword({
        email: email.trim(),
        token: token.trim(),
        newPassword,
      });

      setMessage(response?.message || "Password reset successful.");
      setTimeout(() => {
        navigate("/login", { replace: true });
      }, 1200);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <PageContainer
      title="Reset Password"
      subtitle="Set a new password for your account"
      className="app-page auth-page"
      titleClassName="text-light text-center"
    >
      <Row className="justify-content-center">
        <Col xs={12} md={8} lg={6}>
          <Card className="auth-card">
            <Card.Body>
              <ErrorAlert message={error} onClose={() => setError("")} />
              {message ? <Alert variant="success">{message}</Alert> : null}

              <Form onSubmit={handleSubmit}>
                <Form.Group className="mb-3" controlId="resetEmail">
                  <Form.Label>Email</Form.Label>
                  <Form.Control
                    type="text"
                    inputMode="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                  />
                </Form.Group>

                <Form.Group className="mb-3" controlId="resetToken">
                  <Form.Label>Reset Token</Form.Label>
                  <Form.Control
                    type="text"
                    value={token}
                    onChange={(e) => setToken(e.target.value)}
                    required
                  />
                </Form.Group>

                <Form.Group className="mb-3" controlId="newPassword">
                  <Form.Label>New Password</Form.Label>
                  <Form.Control
                    type="password"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                    required
                  />
                </Form.Group>

                <Button type="submit" disabled={loading} className="w-100">
                  {loading ? "Resetting..." : "Reset Password"}
                </Button>
              </Form>

              {loading ? <LoadingState message="Updating password..." /> : null}

              <p className="mt-3 mb-0 text-center">
                Back to <Link to="/login">login</Link>
              </p>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </PageContainer>
  );
}

export default ResetPasswordPage;
