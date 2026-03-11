import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import Button from "react-bootstrap/Button";
import Card from "react-bootstrap/Card";
import Col from "react-bootstrap/Col";
import Form from "react-bootstrap/Form";
import Row from "react-bootstrap/Row";
import ErrorAlert from "../components/ErrorAlert";
import LoadingState from "../components/LoadingState";
import PageContainer from "../components/PageContainer";
import { useAuth } from "../contexts/AuthContext";

function RegisterPage() {
  const { register, loading, error, clearError } = useAuth();
  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();

  async function handleSubmit(event) {
    event.preventDefault();
    await register(userName, email, password);
    navigate("/", { replace: true });
  }

  return (
    <PageContainer
      title="Register"
      subtitle="Create a new account and immediately receive a JWT token"
      className="app-page"
      titleClassName="text-light text-center"
    >
      <Row className="justify-content-center">
        <Col xs={12} md={8} lg={6}>
          <Card>
            <Card.Body>
              <ErrorAlert message={error} onClose={clearError} />
              <Form onSubmit={handleSubmit}>
                <Form.Group className="mb-3" controlId="userName">
                  <Form.Label>Username</Form.Label>
                  <Form.Control
                    type="text"
                    value={userName}
                    onChange={(e) => setUserName(e.target.value)}
                    required
                  />
                </Form.Group>

                <Form.Group className="mb-3" controlId="email">
                  <Form.Label>Email</Form.Label>
                  <Form.Control
                    type="text"
                    inputMode="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
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
                  {loading ? "Registering..." : "Register"}
                </Button>
              </Form>

              {loading ? <LoadingState message="Creating account..." /> : null}

              <p className="mt-3 mb-0 text-center">
                Already registered? <Link to="/login">Sign in</Link>
              </p>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </PageContainer>
  );
}

export default RegisterPage;
