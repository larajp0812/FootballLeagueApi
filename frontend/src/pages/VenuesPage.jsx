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
  createVenue,
  deleteVenue,
  getVenues,
  updateVenue,
} from "../services/venueService";

const initialForm = {
  name: "",
  address: "",
};

function VenuesPage() {
  const { role } = useAuth();
  const [venues, setVenues] = useState([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [statusMessage, setStatusMessage] = useState("");
  const [form, setForm] = useState(initialForm);
  const [editingId, setEditingId] = useState(null);

  const isAdmin = role === "Admin";

  const sortedVenues = useMemo(
    () => [...venues].sort((a, b) => a.venueId - b.venueId),
    [venues],
  );

  async function loadVenues() {
    setLoading(true);
    setError("");

    try {
      const data = await getVenues();
      setVenues(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadVenues();
  }, []);

  function handleInputChange(event) {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  }

  function resetForm() {
    setEditingId(null);
    setForm(initialForm);
  }

  function handleEdit(venue) {
    setEditingId(venue.venueId);
    setForm({
      name: venue.name,
      address: venue.address ?? "",
    });
    setStatusMessage("Editing selected venue");
  }

  async function handleSave(event) {
    event.preventDefault();
    setSaving(true);
    setError("");
    setStatusMessage("");

    try {
      const payload = {
        name: form.name.trim(),
        address: form.address.trim() || null,
      };

      if (editingId) {
        await updateVenue(editingId, payload);
        setStatusMessage("Venue updated successfully");
      } else {
        await createVenue(payload);
        setStatusMessage("Venue created successfully");
      }

      resetForm();
      await loadVenues();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(venueId) {
    if (!window.confirm("Delete this venue? This cannot be undone.")) {
      return;
    }

    setError("");
    setStatusMessage("");

    try {
      await deleteVenue(venueId);
      setStatusMessage("Venue deleted successfully");
      await loadVenues();
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <PageContainer
      title="Venues"
      subtitle="CRUD module integrated with /api/venues endpoints"
    >
      <ErrorAlert message={error} onClose={() => setError("")} />

      {statusMessage ? (
        <Alert variant="success" dismissible onClose={() => setStatusMessage("")}>
          {statusMessage}
        </Alert>
      ) : null}

      <Tabs defaultActiveKey="list" className="mb-3" justify>
        <Tab eventKey="list" title="Venue List">
          <Card>
            <Card.Body>
              <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="h5 mb-0">All Venues</h2>
                <Button variant="outline-primary" onClick={loadVenues} disabled={loading}>
                  Refresh
                </Button>
              </div>

              {loading ? (
                <LoadingState message="Loading venues..." />
              ) : (
                <div className="table-responsive">
                  <Table striped hover>
                    <thead>
                      <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Address</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {sortedVenues.map((venue) => (
                        <tr key={venue.venueId}>
                          <td>{venue.venueId}</td>
                          <td>{venue.name}</td>
                          <td>{venue.address || "-"}</td>
                          <td>
                            <div className="d-flex gap-2">
                              <Button
                                size="sm"
                                variant="warning"
                                onClick={() => handleEdit(venue)}
                              >
                                Edit
                              </Button>
                              <Button
                                size="sm"
                                variant="danger"
                                onClick={() => handleDelete(venue.venueId)}
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

        <Tab eventKey="form" title={editingId ? "Update Venue" : "Create Venue"}>
          <Card>
            <Card.Body>
              <h2 className="h5 mb-3">
                {editingId ? `Update Venue #${editingId}` : "Create Venue"}
              </h2>

              <Form onSubmit={handleSave}>
                <Row className="g-3">
                  <Col xs={12} md={6}>
                    <Form.Group controlId="venueName">
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
                    <Form.Group controlId="venueAddress">
                      <Form.Label>Address</Form.Label>
                      <Form.Control
                        name="address"
                        value={form.address}
                        onChange={handleInputChange}
                      />
                    </Form.Group>
                  </Col>
                </Row>

                <div className="d-flex gap-2 mt-3">
                  <Button type="submit" disabled={saving}>
                    {saving ? "Saving..." : editingId ? "Update Venue" : "Create Venue"}
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

export default VenuesPage;
