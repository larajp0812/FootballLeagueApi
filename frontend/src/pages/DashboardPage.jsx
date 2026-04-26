import { useEffect, useState } from "react";
import Alert from "react-bootstrap/Alert";
import Button from "react-bootstrap/Button";
import Col from "react-bootstrap/Col";
import Row from "react-bootstrap/Row";
import { Link } from "react-router-dom";
import PageContainer from "../components/PageContainer";
import { useAuth } from "../contexts/AuthContext";
import { getApiHealth } from "../services/healthService";

function DashboardPage() {
  const { role } = useAuth();
  const isAdmin = typeof role === "string" && role.toLowerCase() === "admin";
  const [apiHealth, setApiHealth] = useState("checking");

  useEffect(() => {
    let active = true;

    async function loadHealth() {
      try {
        await getApiHealth();
        if (active) {
          setApiHealth("online");
        }
      } catch {
        if (active) {
          setApiHealth("offline");
        }
      }
    }

    loadHealth();

    return () => {
      active = false;
    };
  }, []);

  const modules = [
    {
      title: "Teams",
      description: "Manage team names, coaches, and founded year.",
      route: "/teams",
    },
    {
      title: "Players",
      description:
        "Manage player profiles, shirt numbers, and team assignment.",
      route: "/players",
    },
    {
      title: "Seasons",
      description: "Create and manage league seasons.",
      route: "/seasons",
    },
    {
      title: "Matches",
      description:
        "Schedule and update fixtures with venue auto-set from home team.",
      route: "/matches",
    },
    {
      title: "League Table",
      description: "Automatic standings with points and goal difference.",
      route: "/table",
    },
    {
      title: "Match Events",
      description: "Track goals, cards, substitutions, and key events.",
      route: "/matchevents",
    },
    {
      title: "Roles",
      description: "Admin-only role management and assignment.",
      route: "/roles",
      adminOnly: true,
    },
  ];

  const visibleModules = modules.filter(
    (module) => !module.adminOnly || isAdmin,
  );

  return (
    <PageContainer
      title="League Management Hub"
      titleClassName="text-white"
      subtitleClassName="text-light"
    >
      <div className="dashboard-shell">
        {apiHealth === "online" ? (
          <Alert variant="success" className="mb-4">
            API status: Online
          </Alert>
        ) : null}

        {apiHealth === "offline" ? (
          <Alert variant="warning" className="mb-4">
            API status: Unreachable. Some actions may be unavailable.
          </Alert>
        ) : null}

        <Row className="g-4 dashboard-cards-grid justify-content-center">
          {visibleModules.map((module) => (
            <Col key={module.title} xs={12} md={6} lg={4}>
              <div className="module-card h-100">
                <div className="module-card-body d-flex flex-column">
                  <h3 className="module-card-title">{module.title}</h3>
                  <p className="module-card-text mb-3">{module.description}</p>
                  <Button
                    as={Link}
                    to={module.route}
                    variant="outline-light"
                    className="mt-auto w-100"
                  >
                    Open {module.title}
                  </Button>
                </div>
              </div>
            </Col>
          ))}
        </Row>
      </div>
    </PageContainer>
  );
}

export default DashboardPage;
