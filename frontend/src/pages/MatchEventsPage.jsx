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
  createMatchEvent,
  deleteMatchEvent,
  getMatchEvents,
  updateMatchEvent,
} from "../services/matchEventService";
import { getMatches } from "../services/matchService";
import { getPlayers } from "../services/playerService";

const initialForm = {
  matchId: "",
  playerId: "",
  minute: "",
  eventType: "Goal",
};

const eventTypes = ["Goal", "YellowCard", "RedCard", "Substitution", "OwnGoal"];

function MatchEventsPage() {
  const { role } = useAuth();
  const [events, setEvents] = useState([]);
  const [matches, setMatches] = useState([]);
  const [players, setPlayers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [statusMessage, setStatusMessage] = useState("");
  const [form, setForm] = useState(initialForm);
  const [editingId, setEditingId] = useState(null);
  const [activeTab, setActiveTab] = useState("list");

  const isAdmin = role === "Admin";

  const sortedEvents = useMemo(
    () => [...events].sort((a, b) => a.matchEventId - b.matchEventId),
    [events],
  );

  const playerNameById = useMemo(() => {
    const map = new Map();
    players.forEach((item) => map.set(item.playerId, item.fullName));
    return map;
  }, [players]);

  const matchLabelById = useMemo(() => {
    const map = new Map();
    matches.forEach((item) => {
      map.set(item.matchId, `Match #${item.matchId}`);
    });
    return map;
  }, [matches]);

  async function loadData() {
    setLoading(true);
    setError("");

    try {
      const [eventsData, matchesData, playersData] = await Promise.all([
        getMatchEvents(),
        getMatches(),
        getPlayers(),
      ]);

      setEvents(Array.isArray(eventsData) ? eventsData : []);
      setMatches(Array.isArray(matchesData) ? matchesData : []);
      setPlayers(Array.isArray(playersData) ? playersData : []);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadData();
  }, []);

  function handleInputChange(event) {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  }

  function resetForm() {
    setEditingId(null);
    setForm(initialForm);
  }

  function handleEdit(matchEvent) {
    setEditingId(matchEvent.matchEventId);
    setForm({
      matchId: String(matchEvent.matchId),
      playerId: matchEvent.playerId ? String(matchEvent.playerId) : "",
      minute: String(matchEvent.minute),
      eventType: matchEvent.eventType,
    });
    setStatusMessage("Editing selected match event");
    setActiveTab("form");
  }

  async function handleSave(event) {
    event.preventDefault();
    setSaving(true);
    setError("");
    setStatusMessage("");

    try {
      if (editingId) {
        await updateMatchEvent(editingId, {
          minute: Number(form.minute),
          eventType: form.eventType,
          playerId: form.playerId ? Number(form.playerId) : null,
        });
        setStatusMessage("Match event updated successfully");
      } else {
        await createMatchEvent({
          matchId: Number(form.matchId),
          playerId: form.playerId ? Number(form.playerId) : null,
          minute: Number(form.minute),
          eventType: form.eventType,
        });
        setStatusMessage("Match event created successfully");
      }

      resetForm();
      setActiveTab("list");
      await loadData();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(matchEventId) {
    if (!window.confirm("Delete this match event? This cannot be undone.")) {
      return;
    }

    setError("");
    setStatusMessage("");

    try {
      await deleteMatchEvent(matchEventId);
      setStatusMessage("Match event deleted successfully");
      await loadData();
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <PageContainer
      title="Match Events"
      subtitle="CRUD module integrated with /api/matchevents endpoints"
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
        <Tab eventKey="list" title="Event List">
          <Card>
            <Card.Body>
              <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="h5 mb-0">All Match Events</h2>
                <Button
                  variant="outline-primary"
                  onClick={loadData}
                  disabled={loading}
                >
                  Refresh
                </Button>
              </div>

              {loading ? (
                <LoadingState message="Loading match events..." />
              ) : (
                <div className="table-responsive">
                  <Table striped hover>
                    <thead>
                      <tr>
                        <th>ID</th>
                        <th>Match</th>
                        <th>Player</th>
                        <th>Minute</th>
                        <th>Event Type</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {sortedEvents.map((matchEvent) => (
                        <tr key={matchEvent.matchEventId}>
                          <td>{matchEvent.matchEventId}</td>
                          <td>
                            {matchLabelById.get(matchEvent.matchId) ??
                              `#${matchEvent.matchId}`}
                          </td>
                          <td>
                            {matchEvent.playerId
                              ? (playerNameById.get(matchEvent.playerId) ??
                                `#${matchEvent.playerId}`)
                              : "-"}
                          </td>
                          <td>{matchEvent.minute}</td>
                          <td>{matchEvent.eventType}</td>
                          <td>
                            <div className="d-flex gap-2">
                              <Button
                                size="sm"
                                variant="warning"
                                onClick={() => handleEdit(matchEvent)}
                              >
                                Edit
                              </Button>
                              <Button
                                size="sm"
                                variant="danger"
                                onClick={() =>
                                  handleDelete(matchEvent.matchEventId)
                                }
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
          title={editingId ? "Update Event" : "Create Event"}
        >
          <Card>
            <Card.Body>
              <h2 className="h5 mb-3">
                {editingId
                  ? `Update Event #${editingId}`
                  : "Create Match Event"}
              </h2>

              <Form onSubmit={handleSave}>
                <Row className="g-3">
                  <Col xs={12} md={6}>
                    <Form.Group controlId="matchId">
                      <Form.Label>Match</Form.Label>
                      <Form.Select
                        name="matchId"
                        value={form.matchId}
                        onChange={handleInputChange}
                        disabled={Boolean(editingId)}
                        required
                      >
                        <option value="">Select match</option>
                        {matches.map((item) => (
                          <option key={item.matchId} value={item.matchId}>
                            Match #{item.matchId}
                          </option>
                        ))}
                      </Form.Select>
                    </Form.Group>
                  </Col>

                  <Col xs={12} md={6}>
                    <Form.Group controlId="playerId">
                      <Form.Label>Player (Optional)</Form.Label>
                      <Form.Select
                        name="playerId"
                        value={form.playerId}
                        onChange={handleInputChange}
                      >
                        <option value="">No specific player</option>
                        {players.map((item) => (
                          <option key={item.playerId} value={item.playerId}>
                            {item.fullName}
                          </option>
                        ))}
                      </Form.Select>
                    </Form.Group>
                  </Col>

                  <Col xs={12} md={6}>
                    <Form.Group controlId="minute">
                      <Form.Label>Minute</Form.Label>
                      <Form.Control
                        name="minute"
                        type="number"
                        min="0"
                        max="130"
                        value={form.minute}
                        onChange={handleInputChange}
                        required
                      />
                    </Form.Group>
                  </Col>

                  <Col xs={12} md={6}>
                    <Form.Group controlId="eventType">
                      <Form.Label>Event Type</Form.Label>
                      <Form.Select
                        name="eventType"
                        value={form.eventType}
                        onChange={handleInputChange}
                        required
                      >
                        {eventTypes.map((type) => (
                          <option key={type} value={type}>
                            {type}
                          </option>
                        ))}
                      </Form.Select>
                    </Form.Group>
                  </Col>
                </Row>

                <div className="d-flex gap-2 mt-3">
                  <Button type="submit" disabled={saving}>
                    {saving
                      ? "Saving..."
                      : editingId
                        ? "Update Event"
                        : "Create Event"}
                  </Button>
                  <Button type="button" variant="secondary" onClick={resetForm}>
                    Clear
                  </Button>
                </div>
              </Form>

              {saving ? <LoadingState message="Submitting request..." /> : null}

              {editingId ? (
                <Alert variant="secondary" className="mt-3 mb-0">
                  Match cannot be changed on update because backend update DTO
                  does not include MatchId.
                </Alert>
              ) : null}
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

export default MatchEventsPage;
