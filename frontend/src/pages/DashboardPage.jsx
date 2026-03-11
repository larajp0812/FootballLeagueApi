import Card from "react-bootstrap/Card";
import Col from "react-bootstrap/Col";
import Row from "react-bootstrap/Row";
import PageContainer from "../components/PageContainer";

function DashboardPage() {
  return (
    <PageContainer
      title="Coursework Dashboard"
      subtitle="Single-page React app integrated with your Football League API"
    >
      <Row className="g-3">
        <Col xs={12} md={6}>
          <Card>
            <Card.Body>
              <Card.Title>Completed in this commit</Card.Title>
              <Card.Text>
                Auth flow, protected routes, responsive layout, plus Teams and
                Players CRUD with loading/error states.
              </Card.Text>
            </Card.Body>
          </Card>
        </Col>
        <Col xs={12} md={6}>
          <Card>
            <Card.Body>
              <Card.Title>Next modules</Card.Title>
              <Card.Text>
                Players, Seasons, Venues, Matches, Match Events, and Roles
                endpoints will be added in follow-up commits.
              </Card.Text>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </PageContainer>
  );
}

export default DashboardPage;
