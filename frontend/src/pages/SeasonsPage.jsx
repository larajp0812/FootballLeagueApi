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
  createSeason,
  deleteSeason,
  getSeasons,
  updateSeason,
} from "../services/seasonService";

const initialForm = {
  name: "",
  startDate: "",
  endDate: "",
};

function toDateInput(value) {
  if (!value) return "";
  return new Date(value).toISOString().slice(0, 10);
}

function formatDate(value) {
  if (!value) return "-";
  return new Date(value).toLocaleDateString();
}

function SeasonsPage() {
  const { role } = useAuth();
  const [seasons, setSeasons] = useState([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [statusMessage, setStatusMessage] = useState("");
  const [form, setForm] = useState(initialForm);
  const [editingId, setEditingId] = useState(null);
  const [activeTab, setActiveTab] = useState("list");

  const isAdmin = role === "Admin";

  const sortedSeasons = useMemo(
    () => [...seasons].sort((a, b) => a.seasonId - b.seasonId),
    [seasons],
  );

  async function loadSeasons() {
    setLoading(true);
    setError("");

    try {
      const data = await getSeasons();
      setSeasons(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadSeasons();
  }, []);

  function handleInputChange(event) {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  }

  function resetForm() {
    setEditingId(null);
    setForm(initialForm);
  }

  function handleEdit(season) {
    setEditingId(season.seasonId);
    setForm({
      name: season.name,
      startDate: toDateInput(season.startDate),
      endDate: toDateInput(season.endDate),
    });
    setStatusMessage("Editing selected season");
    setActiveTab("form");
  }

  async function handleSave(event) {
    event.preventDefault();
    setSaving(true);
    setError("");
    setStatusMessage("");

    try {
      const payload = {
        name: form.name.trim(),
        startDate: form.startDate,
        endDate: form.endDate,
      };

      if (editingId) {
        await updateSeason(editingId, payload);
        setStatusMessage("Season updated successfully");
      } else {
        await createSeason(payload);
        setStatusMessage("Season created successfully");
      }

      resetForm();
      setActiveTab("list");
      await loadSeasons();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(seasonId) {
    if (!window.confirm("Delete this season? This cannot be undone.")) {
      return;
    }

    setError("");
    setStatusMessage("");

    try {
      await deleteSeason(seasonId);
      setStatusMessage("Season deleted successfully");
      await loadSeasons();
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <PageContainer
      title="Seasons"
      subtitle="CRUD module integrated with /api/seasons endpoints"
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
        <Tab eventKey="list" title="Season List">
          <Card>
            <Card.Body>
              <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="h5 mb-0">All Seasons</h2>
                <Button
                  variant="outline-primary"
                  onClick={loadSeasons}
                  disabled={loading}
                >
                  Refresh
                </Button>
              </div>

              {loading ? (
                <LoadingState message="Loading seasons..." />
              ) : (
                <div className="table-responsive">
                  <Table striped hover>
                    <thead>
                      <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Start Date</th>
                        <th>End Date</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {sortedSeasons.map((season) => (
                        <tr key={season.seasonId}>
                          <td>{season.seasonId}</td>
                          <td>{season.name}</td>
                          <td>{formatDate(season.startDate)}</td>
                          <td>{formatDate(season.endDate)}</td>
                          <td>
                            <div className="d-flex gap-2">
                              <Button
                                size="sm"
                                variant="warning"
                                onClick={() => handleEdit(season)}
                              >
                                Edit
                              </Button>
                              <Button
                                size="sm"
                                variant="danger"
                                onClick={() => handleDelete(season.seasonId)}
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

        <Tab
          eventKey="form"
          title={editingId ? "Update Season" : "Create Season"}
        >
          <Card>
            <Card.Body>
              <h2 className="h5 mb-3">
                {editingId ? `Update Season #${editingId}` : "Create Season"}
              </h2>

              <Form onSubmit={handleSave}>
                <Row className="g-3">
                  <Col xs={12} md={6}>
                    <Form.Group controlId="seasonName">
                      <Form.Label>Name</Form.Label>
                      <Form.Control
                        name="name"
                        value={form.name}
                        onChange={handleInputChange}
                        required
                      />
                    </Form.Group>
                  </Col>

                  <Col xs={12} md={3}>
                    <Form.Group controlId="seasonStartDate">
                      <Form.Label>Start Date</Form.Label>
                      <Form.Control
                        name="startDate"
                        type="date"
                        value={form.startDate}
                        onChange={handleInputChange}
                        required
                      />
                    </Form.Group>
                  </Col>

                  <Col xs={12} md={3}>
                    <Form.Group controlId="seasonEndDate">
                      <Form.Label>End Date</Form.Label>
                      <Form.Control
                        name="endDate"
                        type="date"
                        value={form.endDate}
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
                        ? "Update Season"
                        : "Create Season"}
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

export default SeasonsPage;
