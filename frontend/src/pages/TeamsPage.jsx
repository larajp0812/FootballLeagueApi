import { useEffect, useMemo, useState } from "react";
import Alert from "react-bootstrap/Alert";
import Button from "react-bootstrap/Button";
import Card from "react-bootstrap/Card";
import Col from "react-bootstrap/Col";
import Form from "react-bootstrap/Form";
import Row from "react-bootstrap/Row";
import Tab from "react-bootstrap/Tab";
import Table from "react-bootstrap/Table";
import Tabs from "react-bootstrap/Tabs";
import ErrorAlert from "../components/ErrorAlert";
import LoadingState from "../components/LoadingState";
import PageContainer from "../components/PageContainer";
import { useAuth } from "../contexts/AuthContext";
import {
  createTeam,
  deleteTeam,
  getTeams,
  updateTeam,
} from "../services/teamService";

const initialForm = {
  name: "",
  coach: "",
  foundedYear: "",
  venue: "",
};

function TeamsPage() {
  const { role } = useAuth();
  const [teams, setTeams] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [statusMessage, setStatusMessage] = useState("");
  const [form, setForm] = useState(initialForm);
  const [editingId, setEditingId] = useState(null);
  const [saving, setSaving] = useState(false);
  const [activeTab, setActiveTab] = useState("list");

  const isAdmin = role === "Admin";

  const sortedTeams = useMemo(
    () => [...teams].sort((a, b) => a.teamId - b.teamId),
    [teams],
  );

  async function loadTeams() {
    setLoading(true);
    setError("");

    try {
      const data = await getTeams();
      setTeams(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadTeams();
  }, []);

  function handleInputChange(event) {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  }

  function handleEdit(team) {
    setEditingId(team.teamId);
    setForm({
      name: team.name,
      coach: team.coach,
      foundedYear: String(team.foundedYear),
      venue: team.venue ?? "",
    });
    setStatusMessage("Editing selected team");
    setActiveTab("form");
  }

  function resetForm() {
    setEditingId(null);
    setForm(initialForm);
  }

  async function handleSave(event) {
    event.preventDefault();
    setSaving(true);
    setError("");
    setStatusMessage("");

    try {
      const payload = {
        name: form.name.trim(),
        coach: form.coach.trim(),
        foundedYear: Number(form.foundedYear),
        venue: form.venue.trim(),
      };

      if (editingId) {
        await updateTeam(editingId, payload);
        setStatusMessage("Team updated successfully");
      } else {
        await createTeam(payload);
        setStatusMessage("Team created successfully");
      }

      resetForm();
      setActiveTab("list");
      await loadTeams();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(teamId) {
    if (!window.confirm("Delete this team? This cannot be undone.")) {
      return;
    }

    setError("");
    setStatusMessage("");

    try {
      await deleteTeam(teamId);
      setStatusMessage("Team deleted successfully");
      await loadTeams();
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <PageContainer
      title="Teams"
      className="app-page"
    >
      <ErrorAlert message={error} onClose={() => setError("")} />

      {statusMessage ? (
        <Alert
          variant="success"
          dismissible
          onClose={() => setStatusMessage("")}
        >
          {statusMessage}
        </Alert>
      ) : null}

      <Tabs
        activeKey={activeTab}
        onSelect={(key) => setActiveTab(key ?? "list")}
        className="mb-3"
        justify
      >
        <Tab eventKey="list" title="Team List">
          <Card>
            <Card.Body>
              <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="h5 mb-0">All Teams</h2>
                <Button
                  variant="outline-primary"
                  onClick={loadTeams}
                  disabled={loading}
                >
                  Refresh
                </Button>
              </div>

              {loading ? (
                <LoadingState message="Loading teams..." />
              ) : (
                <div className="table-responsive">
                  <Table striped hover>
                    <thead>
                      <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Coach</th>
                        <th>Venue</th>
                        <th>Founded Year</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {sortedTeams.map((team) => (
                        <tr key={team.teamId}>
                          <td>{team.teamId}</td>
                          <td>{team.name}</td>
                          <td>{team.coach}</td>
                          <td>{team.venue}</td>
                          <td>{team.foundedYear}</td>
                          <td>
                            <div className="d-flex gap-2">
                              <Button
                                size="sm"
                                variant="warning"
                                onClick={() => handleEdit(team)}
                              >
                                Edit
                              </Button>
                              <Button
                                size="sm"
                                variant="danger"
                                onClick={() => handleDelete(team.teamId)}
                                disabled={!isAdmin}
                              >
                                Delete
                              </Button>
                            </div>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                </div>
              )}
            </Card.Body>
          </Card>
        </Tab>

        <Tab eventKey="form" title={editingId ? "Update Team" : "Create Team"}>
          <Card>
            <Card.Body>
              <h2 className="h5 mb-3">
                {editingId ? `Update Team #${editingId}` : "Create Team"}
              </h2>

              <Form onSubmit={handleSave}>
                <Row className="g-3">
                  <Col xs={12} md={6}>
                    <Form.Group controlId="teamName">
                      <Form.Label>Name</Form.Label>
                      <Form.Control
                        name="name"
                        value={form.name}
                        onChange={handleInputChange}
                        required
                      />
                    </Form.Group>
                  </Col>
                  <Col xs={12} md={6}>
                    <Form.Group controlId="teamCoach">
                      <Form.Label>Coach</Form.Label>
                      <Form.Control
                        name="coach"
                        value={form.coach}
                        onChange={handleInputChange}
                        required
                      />
                    </Form.Group>
                  </Col>
                  <Col xs={12} md={6}>
                    <Form.Group controlId="foundedYear">
                      <Form.Label>Founded Year</Form.Label>
                      <Form.Control
                        name="foundedYear"
                        type="number"
                        min="1800"
                        value={form.foundedYear}
                        onChange={handleInputChange}
                        required
                      />
                    </Form.Group>
                  </Col>
                  <Col xs={12} md={6}>
                    <Form.Group controlId="teamVenue">
                      <Form.Label>Home Venue</Form.Label>
                      <Form.Control
                        name="venue"
                        value={form.venue}
                        onChange={handleInputChange}
                        required
                      />
                    </Form.Group>
                  </Col>
                </Row>

                <div className="d-flex gap-2 mt-3">
                  <Button type="submit" disabled={saving}>
                    {saving
                      ? "Saving..."
                      : editingId
                        ? "Update Team"
                        : "Create Team"}
                  </Button>
                  <Button type="button" variant="secondary" onClick={resetForm}>
                    Clear
                  </Button>
                </div>
              </Form>

              {saving ? <LoadingState message="Submitting request..." /> : null}
            </Card.Body>
          </Card>
        </Tab>
      </Tabs>

      {!isAdmin ? (
        <Alert variant="info" className="mt-3 mb-0">
          Delete operations require an Admin role.
        </Alert>
      ) : null}
    </PageContainer>
  );

}

export default TeamsPage;
