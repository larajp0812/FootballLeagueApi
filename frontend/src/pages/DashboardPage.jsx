import Badge from "react-bootstrap/Badge";
import Button from "react-bootstrap/Button";
import Card from "react-bootstrap/Card";
import Col from "react-bootstrap/Col";
import Row from "react-bootstrap/Row";
import { Link } from "react-router-dom";
import PageContainer from "../components/PageContainer";
import { useAuth } from "../contexts/AuthContext";

function DashboardPage() {
  const { role } = useAuth();

  const modules = [
    {
      title: "Teams",
      description: "Manage team names, coaches, and founded year.",
      route: "/teams",
      status: "Live",
      variant: "success",
    },
    {
      title: "Players",
      description:
        "Manage player profiles, shirt numbers, and team assignment.",
      route: "/players",
      status: "Live",
      variant: "success",
    },
    {
      title: "Seasons",
      description: "Create and manage league seasons.",
      route: "/seasons",
      status: "Live",
      variant: "success",
    },
    {
      title: "Venues",
      description: "Manage stadium and venue information.",
      route: "/venues",
      status: "Live",
      variant: "success",
    },
    {
      title: "Matches",
      description: "Schedule and update fixtures with teams and venues.",
      route: "/matches",
      status: "Live",
      variant: "success",
    },
    {
      title: "Match Events",
      description: "Track goals, cards, substitutions, and key events.",
      route: "/matchevents",
      status: "Live",
      variant: "success",
    },
    {
      title: "Roles",
      description: "Admin-only role management and assignment.",
      status: "Next",
      variant: "secondary",
    },
  ];

  return (
    <PageContainer
      title="Football League Control Panel"
      subtitle="Manage your league data from one place with authenticated API modules"
    >
      <Row className="g-3 mb-3">
        <Col xs={12} md={6}>
          <Card>
            <Card.Body>
              <Card.Title>Account</Card.Title>
              <Card.Text className="mb-0">
                Logged in role: <strong>{role}</strong>
              </Card.Text>
            </Card.Body>
          </Card>
        </Col>
        <Col xs={12} md={6}>
          <Card>
            <Card.Body>
              <Card.Title>Current Coverage</Card.Title>
              <Card.Text className="mb-0">
                Live now: Teams + Players + Seasons + Venues + Matches modules.
                + Match Events modules. Remaining endpoints are listed below for
                the next commits.
              </Card.Text>
            </Card.Body>
          </Card>
        </Col>
      </Row>

      <Row className="g-3">
        {modules.map((module) => (
          <Col key={module.title} xs={12} md={6} lg={4}>
            <Card className="h-100">
              <Card.Body className="d-flex flex-column">
                <div className="d-flex justify-content-between align-items-center mb-2">
                  <Card.Title className="mb-0">{module.title}</Card.Title>
                  <Badge bg={module.variant}>{module.status}</Badge>
                </div>
                <Card.Text className="mb-3">{module.description}</Card.Text>
                {module.route ? (
                  <Button as={Link} to={module.route} className="mt-auto">
                    Open {module.title}
                  </Button>
                ) : (
                  <Button
                    variant="outline-secondary"
                    disabled
                    className="mt-auto"
                  >
                    Coming in next commit
                  </Button>
                )}
              </Card.Body>
            </Card>
          </Col>
        ))}
      </Row>
    </PageContainer>
  );
}

export default DashboardPage;
