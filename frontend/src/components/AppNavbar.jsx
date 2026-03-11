import { useState } from "react";
import Container from "react-bootstrap/Container";
import Nav from "react-bootstrap/Nav";
import Navbar from "react-bootstrap/Navbar";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

function AppNavbar() {
  const { isAuthenticated, role, logout } = useAuth();
  const navigate = useNavigate();
  const [expanded, setExpanded] = useState(false);

  function handleNavClick() {
    setExpanded(false);
  }

  function handleLogout() {
    logout();
    navigate("/login");
    setExpanded(false);
  }

  return (
    <Navbar
      bg="dark"
      variant="dark"
      className="app-navbar"
      expand="xl"
      expanded={expanded}
      onToggle={setExpanded}
    >
      <Container>
        <Navbar.Brand as={Link} to="/">
          Football League Manager
        </Navbar.Brand>
        <Navbar.Toggle aria-controls="main-nav" />
        <Navbar.Collapse id="main-nav">
          <Nav className="me-auto">
            {isAuthenticated && (
              <>
                <Nav.Link as={Link} to="/teams" onClick={handleNavClick}>
                  Teams
                </Nav.Link>
                <Nav.Link as={Link} to="/players" onClick={handleNavClick}>
                  Players
                </Nav.Link>
                <Nav.Link as={Link} to="/seasons" onClick={handleNavClick}>
                  Seasons
                </Nav.Link>
                <Nav.Link as={Link} to="/matches" onClick={handleNavClick}>
                  Matches
                </Nav.Link>
                <Nav.Link as={Link} to="/table" onClick={handleNavClick}>
                  League Table
                </Nav.Link>
                <Nav.Link as={Link} to="/matchevents" onClick={handleNavClick}>
                  Match Events
                </Nav.Link>
                <Nav.Link as={Link} to="/roles" onClick={handleNavClick}>
                  Roles
                </Nav.Link>
              </>
            )}
          </Nav>
          <Nav>
            {!isAuthenticated ? (
              <>
                <Nav.Link as={Link} to="/login" onClick={handleNavClick}>
                  Login
                </Nav.Link>
                <Nav.Link as={Link} to="/register" onClick={handleNavClick}>
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
