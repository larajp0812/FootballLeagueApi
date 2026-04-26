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
  createMatch,
  deleteMatch,
  getMatches,
  updateMatch,
} from "../services/matchService";
import { getSeasons } from "../services/seasonService";
import { getTeams } from "../services/teamService";

const initialForm = {
  homeTeamId: "",
  awayTeamId: "",
  seasonId: "",
  kickoffTime: "",
  homeScore: "0",
  awayScore: "0",
};

function formatDateTime(value) {
  if (!value) return "-";
  return new Date(value).toLocaleString();
}

function toDateTimeLocal(value) {
  if (!value) return "";
  const date = new Date(value);
  const tzOffset = date.getTimezoneOffset() * 60000;
  return new Date(date.getTime() - tzOffset).toISOString().slice(0, 16);
}

function MatchesPage() {
  const { role } = useAuth();
  const [matches, setMatches] = useState([]);
  const [teams, setTeams] = useState([]);
  const [seasons, setSeasons] = useState([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [statusMessage, setStatusMessage] = useState("");
  const [form, setForm] = useState(initialForm);
  const [editingId, setEditingId] = useState(null);
  const [activeTab, setActiveTab] = useState("list");

  const isAdmin = role === "Admin";

  const sortedMatches = useMemo(
    () => [...matches].sort((a, b) => a.matchId - b.matchId),
    [matches],
  );

  const teamNameById = useMemo(() => {
    const map = new Map();
    teams.forEach((item) => map.set(item.teamId, item.name));
    return map;
  }, [teams]);

  const seasonNameById = useMemo(() => {
    const map = new Map();
    seasons.forEach((item) => map.set(item.seasonId, item.name));
    return map;
  }, [seasons]);

  const teamVenueById = useMemo(() => {
    const map = new Map();
    teams.forEach((item) => map.set(item.teamId, item.venue));
    return map;
  }, [teams]);

  async function loadData() {
    setLoading(true);
    setError("");

    try {
      const [matchesData, teamsData, seasonsData] = await Promise.all([
        getMatches(),
        getTeams(),
        getSeasons(),
      ]);

      setMatches(Array.isArray(matchesData) ? matchesData : []);
      setTeams(Array.isArray(teamsData) ? teamsData : []);
      setSeasons(Array.isArray(seasonsData) ? seasonsData : []);
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

  function handleEdit(match) {
    setEditingId(match.matchId);
    setForm({
      homeTeamId: String(match.homeTeamId),
      awayTeamId: String(match.awayTeamId),
      seasonId: String(match.seasonId),
      kickoffTime: toDateTimeLocal(match.kickoffTime),
      homeScore: String(match.homeScore),
      awayScore: String(match.awayScore),
    });
    setStatusMessage("Editing selected match");
    setActiveTab("form");
  }

  async function handleSave(event) {
    event.preventDefault();
    setSaving(true);
    setError("");
    setStatusMessage("");

    try {
      if (editingId) {
        if (form.homeTeamId === form.awayTeamId) {
          throw new Error("Home and away team must be different");
        }

        await updateMatch(editingId, {
          homeTeamId: Number(form.homeTeamId),
          awayTeamId: Number(form.awayTeamId),
          seasonId: Number(form.seasonId),
          homeScore: Number(form.homeScore),
          awayScore: Number(form.awayScore),
          kickoffTime: new Date(form.kickoffTime).toISOString(),
        });
        setStatusMessage("Match updated successfully");
      } else {
        if (form.homeTeamId === form.awayTeamId) {
          throw new Error("Home and away team must be different");
        }

        await createMatch({
          homeTeamId: Number(form.homeTeamId),
          awayTeamId: Number(form.awayTeamId),
          seasonId: Number(form.seasonId),
          homeScore: Number(form.homeScore),
          awayScore: Number(form.awayScore),
          kickoffTime: new Date(form.kickoffTime).toISOString(),
        });
        setStatusMessage("Match created successfully");
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

  async function handleDelete(matchId) {
    if (!window.confirm("Delete this match? This cannot be undone.")) {
      return;
    }

    setError("");
    setStatusMessage("");

    try {
      await deleteMatch(matchId);
      setStatusMessage("Match deleted successfully");
      await loadData();
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <PageContainer
      title="Matches"
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
        <Tab eventKey="list" title="Match List">
          <Card>
            <Card.Body>
              <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="h5 mb-0">All Matches</h2>
                <Button
                  variant="outline-primary"
                  onClick={loadData}
                  disabled={loading}
                >
                  Refresh
                </Button>
              </div>

              {loading ? (
                <LoadingState message="Loading matches..." />
              ) : (
                <div className="table-responsive">
                  <Table striped hover>
                    <thead>
                      <tr>
                        <th>ID</th>
                        <th>Fixture</th>
                        <th>Season</th>
                        <th>Venue</th>
                        <th>Kickoff</th>
                        <th>Score</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {sortedMatches.map((match) => (
                        <tr key={match.matchId}>
                          <td>{match.matchId}</td>
                          <td>
                            {(teamNameById.get(match.homeTeamId) ??
                              `#${match.homeTeamId}`) +
                              " vs " +
                              (teamNameById.get(match.awayTeamId) ??
                                `#${match.awayTeamId}`)}
                          </td>
                          <td>
                            {seasonNameById.get(match.seasonId) ??
                              `#${match.seasonId}`}
                          </td>
                          <td>
                            {match.venue ||
                              teamVenueById.get(match.homeTeamId) ||
                              "-"}
                          </td>
                          <td>{formatDateTime(match.kickoffTime)}</td>
                          <td>{`${match.homeScore} - ${match.awayScore}`}</td>
                          <td>
                            <div className="d-flex gap-2">
                              <Button
                                size="sm"
                                variant="warning"
                                onClick={() => handleEdit(match)}
                              >
                                Edit
                              </Button>
                              <Button
                                size="sm"
                                variant="danger"
                                onClick={() => handleDelete(match.matchId)}
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
          title={editingId ? "Update Match" : "Create Match"}
        >
          <Card>
            <Card.Body>
              <h2 className="h5 mb-3">
                {editingId ? `Update Match #${editingId}` : "Create Match"}
              </h2>

              <Form onSubmit={handleSave}>
                <Row className="g-3">
                  <Col xs={12} md={6}>
                    <Form.Group controlId="homeTeamId">
                      <Form.Label>Home Team</Form.Label>
                      <Form.Select
                        name="homeTeamId"
                        value={form.homeTeamId}
                        onChange={handleInputChange}
                        required
                      >
                        <option value="">Select home team</option>
                        {teams.map((item) => (
                          <option key={item.teamId} value={item.teamId}>
                            {item.name}
                          </option>
                        ))}
                      </Form.Select>
                    </Form.Group>
                  </Col>

                  <Col xs={12} md={6}>
                    <Form.Group controlId="awayTeamId">
                      <Form.Label>Away Team</Form.Label>
                      <Form.Select
                        name="awayTeamId"
                        value={form.awayTeamId}
                        onChange={handleInputChange}
                        required
                      >
                        <option value="">Select away team</option>
                        {teams.map((item) => (
                          <option key={item.teamId} value={item.teamId}>
                            {item.name}
                          </option>
                        ))}
                      </Form.Select>
                    </Form.Group>
                  </Col>

                  <Col xs={12} md={4}>
                    <Form.Group controlId="seasonId">
                      <Form.Label>Season</Form.Label>
                      <Form.Select
                        name="seasonId"
                        value={form.seasonId}
                        onChange={handleInputChange}
                        required
                      >
                        <option value="">Select season</option>
                        {seasons.map((item) => (
                          <option key={item.seasonId} value={item.seasonId}>
                            {item.name}
                          </option>
                        ))}
                      </Form.Select>
                    </Form.Group>
                  </Col>

                  <Col xs={12} md={4}>
                    <Form.Group controlId="kickoffTime">
                      <Form.Label>Kickoff Time</Form.Label>
                      <Form.Control
                        name="kickoffTime"
                        type="datetime-local"
                        value={form.kickoffTime}
                        onChange={handleInputChange}
                        required
                      />
                    </Form.Group>
                  </Col>

                  <Col xs={12} md={6}>
                    <Form.Group controlId="homeScore">
                      <Form.Label>Home Score</Form.Label>
                      <Form.Control
                        name="homeScore"
                        type="number"
                        min="0"
                        value={form.homeScore}
                        onChange={handleInputChange}
                        required
                      />
                    </Form.Group>
                  </Col>

                  <Col xs={12} md={6}>
                    <Form.Group controlId="awayScore">
                      <Form.Label>Away Score</Form.Label>
                      <Form.Control
                        name="awayScore"
                        type="number"
                        min="0"
                        value={form.awayScore}
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
                        ? "Update Match"
                        : "Create Match"}
                  </Button>
                  <Button type="button" variant="secondary" onClick={resetForm}>
                    Clear
                  </Button>
                </div>
              </Form>

              {saving ? <LoadingState message="Submitting request..." /> : null}

              {editingId ? (
                <Alert variant="secondary" className="mt-3 mb-0">
                  Venue is automatically based on the selected home team.
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

export default MatchesPage;
