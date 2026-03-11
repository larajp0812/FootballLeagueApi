import { useEffect, useState } from "react";
import Container from "react-bootstrap/Container";
import Nav from "react-bootstrap/Nav";
import Navbar from "react-bootstrap/Navbar";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

function AppNavbar() {
  const { isAuthenticated, role, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [expanded, setExpanded] = useState(false);

  useEffect(() => {
    setExpanded(false);
  }, [location.pathname]);

  function handleLogout() {
    logout();
    navigate("/login");
    setExpanded(false);
  }

  return (
    <Navbar
      bg="dark"
      variant="dark"
      expand="xl"
      expanded={expanded}
      onToggle={setExpanded}
    >
      <Container>
        <Navbar.Brand as={Link} to="/">
          Football League Frontend
        </Navbar.Brand>
        <Navbar.Toggle aria-controls="main-nav" />
        <Navbar.Collapse id="main-nav">
          <Nav className="me-auto">
            {isAuthenticated && (
              <>
                <Nav.Link as={Link} to="/">
                  Dashboard
                </Nav.Link>
                <Nav.Link as={Link} to="/teams">
                  Teams
                </Nav.Link>
                <Nav.Link as={Link} to="/players">
                  Players
                </Nav.Link>
                <Nav.Link as={Link} to="/seasons">
                  Seasons
                </Nav.Link>
                <Nav.Link as={Link} to="/venues">
                  Venues
                </Nav.Link>
                <Nav.Link as={Link} to="/matches">
                  Matches
                </Nav.Link>
                <Nav.Link as={Link} to="/table">
                  League Table
                </Nav.Link>
                <Nav.Link as={Link} to="/matchevents">
                  Match Events
                </Nav.Link>
                <Nav.Link as={Link} to="/roles">
                  Roles
                </Nav.Link>
              </>
            )}
          </Nav>
          <Nav>
            {!isAuthenticated ? (
              <>
                <Nav.Link as={Link} to="/login">
                  Login
                </Nav.Link>
                <Nav.Link as={Link} to="/register">
                  Register
                </Nav.Link>
              </>
            ) : (
              <>
                <Navbar.Text className="me-3">Role: {role}</Navbar.Text>
                <Nav.Link onClick={handleLogout}>Logout</Nav.Link>
              </>
            )}
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

export default AppNavbar;
