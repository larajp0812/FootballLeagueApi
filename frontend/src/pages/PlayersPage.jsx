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
import { getTeams } from "../services/teamService";
import {
  createPlayer,
  deletePlayer,
  getPlayers,
  updatePlayer,
} from "../services/playerService";

const initialForm = {
  fullName: "",
  shirtNumber: "",
  position: "",
  teamId: "",
};

function PlayersPage() {
  const { role } = useAuth();
  const [players, setPlayers] = useState([]);
  const [teams, setTeams] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [statusMessage, setStatusMessage] = useState("");
  const [form, setForm] = useState(initialForm);
  const [editingPlayerId, setEditingPlayerId] = useState(null);
  const [saving, setSaving] = useState(false);

  const isAdmin = role === "Admin";

  const sortedPlayers = useMemo(
    () => [...players].sort((a, b) => a.playerId - b.playerId),
    [players],
  );

  const teamNameById = useMemo(() => {
    const lookup = new Map();
    teams.forEach((team) => {
      lookup.set(team.teamId, team.name);
    });
    return lookup;
  }, [teams]);

  async function loadData() {
    setLoading(true);
    setError("");

    try {
      const [playersData, teamsData] = await Promise.all([
        getPlayers(),
        getTeams(),
      ]);
      setPlayers(Array.isArray(playersData) ? playersData : []);
      setTeams(Array.isArray(teamsData) ? teamsData : []);
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

  function handleEdit(player) {
    setEditingPlayerId(player.playerId);
    setForm({
      fullName: player.fullName,
      shirtNumber: String(player.shirtNumber),
      position: player.position ?? "",
      teamId: String(player.teamId),
    });
    setStatusMessage("Editing selected player");
  }

  function resetForm() {
    setEditingPlayerId(null);
    setForm(initialForm);
  }

  async function handleSave(event) {
    event.preventDefault();
    setSaving(true);
    setError("");
    setStatusMessage("");

    try {
      const commonData = {
        fullName: form.fullName.trim(),
        shirtNumber: Number(form.shirtNumber),
        position: form.position.trim() || null,
      };

      if (editingPlayerId) {
        await updatePlayer(editingPlayerId, commonData);
        setStatusMessage("Player updated successfully");
      } else {
        await createPlayer({ ...commonData, teamId: Number(form.teamId) });
        setStatusMessage("Player created successfully");
      }

      resetForm();
      await loadData();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(playerId) {
    if (!window.confirm("Delete this player? This cannot be undone.")) {
      return;
    }

    setError("");
    setStatusMessage("");

    try {
      await deletePlayer(playerId);
      setStatusMessage("Player deleted successfully");
      await loadData();
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <PageContainer
      title="Players"
      subtitle="CRUD module integrated with /api/players endpoints"
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

      <Tabs defaultActiveKey="list" className="mb-3" justify>
        <Tab eventKey="list" title="Player List">
          <Card>
            <Card.Body>
              <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="h5 mb-0">All Players</h2>
                <Button
                  variant="outline-primary"
                  onClick={loadData}
                  disabled={loading}
                >
                  Refresh
                </Button>
              </div>

              {loading ? (
                <LoadingState message="Loading players..." />
              ) : (
                <div className="table-responsive">
                  <Table striped hover>
                    <thead>
                      <tr>
                        <th>ID</th>
                        <th>Full Name</th>
                        <th>Shirt No.</th>
                        <th>Position</th>
                        <th>Team</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {sortedPlayers.map((player) => (
                        <tr key={player.playerId}>
                          <td>{player.playerId}</td>
                          <td>{player.fullName}</td>
                          <td>{player.shirtNumber}</td>
                          <td>{player.position || "-"}</td>
                          <td>
                            {teamNameById.get(player.teamId) ??
                              `Team #${player.teamId}`}
                          </td>
                          <td>
                            <div className="d-flex gap-2">
                              <Button
                                size="sm"
                                variant="warning"
                                onClick={() => handleEdit(player)}
                              >
                                Edit
                              </Button>
                              <Button
                                size="sm"
                                variant="danger"
                                onClick={() => handleDelete(player.playerId)}
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
          title={editingPlayerId ? "Update Player" : "Create Player"}
        >
          <Card>
            <Card.Body>
              <h2 className="h5 mb-3">
                {editingPlayerId
                  ? `Update Player #${editingPlayerId}`
                  : "Create Player"}
              </h2>

              <Form onSubmit={handleSave}>
                <Row className="g-3">
                  <Col xs={12} md={6}>
                    <Form.Group controlId="fullName">
                      <Form.Label>Full Name</Form.Label>
                      <Form.Control
                        name="fullName"
                        value={form.fullName}
                        onChange={handleInputChange}
                        required
                      />
                    </Form.Group>
                  </Col>

                  <Col xs={12} md={6}>
                    <Form.Group controlId="shirtNumber">
                      <Form.Label>Shirt Number</Form.Label>
                      <Form.Control
                        name="shirtNumber"
                        type="number"
                        min="1"
                        value={form.shirtNumber}
                        onChange={handleInputChange}
                        required
                      />
                    </Form.Group>
                  </Col>

                  <Col xs={12} md={6}>
                    <Form.Group controlId="position">
                      <Form.Label>Position</Form.Label>
                      <Form.Control
                        name="position"
                        value={form.position}
                        onChange={handleInputChange}
                        placeholder="e.g. Midfielder"
                      />
                    </Form.Group>
                  </Col>

                  <Col xs={12} md={6}>
                    <Form.Group controlId="teamId">
                      <Form.Label>Team</Form.Label>
                      <Form.Select
                        name="teamId"
                        value={form.teamId}
                        onChange={handleInputChange}
                        disabled={Boolean(editingPlayerId)}
                        required
                      >
                        <option value="">Select team</option>
                        {teams.map((team) => (
                          <option key={team.teamId} value={team.teamId}>
                            {team.name}
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
                      : editingPlayerId
                        ? "Update Player"
                        : "Create Player"}
                  </Button>
                  <Button type="button" variant="secondary" onClick={resetForm}>
                    Clear
                  </Button>
                </div>
              </Form>

              {saving ? <LoadingState message="Submitting request..." /> : null}
              {editingPlayerId ? (
                <Alert variant="secondary" className="mt-3 mb-0">
                  Team cannot be changed on update because backend update DTO
                  does not include TeamId.
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

export default PlayersPage;
