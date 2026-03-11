import Button from "react-bootstrap/Button";
import Card from "react-bootstrap/Card";
import Col from "react-bootstrap/Col";
import Row from "react-bootstrap/Row";
import { Link } from "react-router-dom";
import PageContainer from "../components/PageContainer";

function DashboardPage() {
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
    },
  ];

  return (
    <PageContainer>
      <Row className="g-3">
        {modules.map((module) => (
          <Col key={module.title} xs={12} md={6} lg={4}>
            <Card className="h-100">
              <Card.Body className="d-flex flex-column">
                <Card.Title className="mb-2">{module.title}</Card.Title>
                <Card.Text className="mb-3">{module.description}</Card.Text>
                <Button as={Link} to={module.route} className="mt-auto">
                  Open {module.title}
                </Button>
              </Card.Body>
            </Card>
          </Col>
        ))}
      </Row>
    </PageContainer>
  );
}

export default DashboardPage;
