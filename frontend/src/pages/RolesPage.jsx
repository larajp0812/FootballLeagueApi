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
  assignRoleToUser,
  createRole,
  deleteRole,
  getRoles,
  updateRole,
} from "../services/roleService";

const initialRoleForm = {
  roleName: "",
};

const initialAssignForm = {
  userId: "",
  roleName: "",
};

function RolesPage() {
  const { role } = useAuth();
  const [roles, setRoles] = useState([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [assigning, setAssigning] = useState(false);
  const [error, setError] = useState("");
  const [statusMessage, setStatusMessage] = useState("");
  const [roleForm, setRoleForm] = useState(initialRoleForm);
  const [assignForm, setAssignForm] = useState(initialAssignForm);
  const [editingRoleId, setEditingRoleId] = useState(null);

  const isAdmin = role === "Admin";

  const sortedRoles = useMemo(() => {
    const source = Array.isArray(roles) ? roles : [];
    return [...source].sort((a, b) =>
      (a.name || "").localeCompare(b.name || ""),
    );
  }, [roles]);

  async function loadRoles() {
    setLoading(true);
    setError("");

    try {
      const data = await getRoles();
      setRoles(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadRoles();
  }, []);

  function handleRoleInputChange(event) {
    const { name, value } = event.target;
    setRoleForm((current) => ({ ...current, [name]: value }));
  }

  function handleAssignInputChange(event) {
    const { name, value } = event.target;
    setAssignForm((current) => ({ ...current, [name]: value }));
  }

  function resetRoleForm() {
    setEditingRoleId(null);
    setRoleForm(initialRoleForm);
  }

  function handleEdit(selectedRole) {
    setEditingRoleId(selectedRole.id);
    setRoleForm({ roleName: selectedRole.name ?? "" });
    setStatusMessage("Editing selected role");
  }

  async function handleSaveRole(event) {
    event.preventDefault();
    setSaving(true);
    setError("");
    setStatusMessage("");

    try {
      if (editingRoleId) {
        await updateRole(editingRoleId, roleForm.roleName.trim());
        setStatusMessage("Role updated successfully");
      } else {
        await createRole(roleForm.roleName.trim());
        setStatusMessage("Role created successfully");
      }

      resetRoleForm();
      await loadRoles();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(roleId) {
    if (!window.confirm("Delete this role? This cannot be undone.")) {
      return;
    }

    setError("");
    setStatusMessage("");

    try {
      await deleteRole(roleId);
      setStatusMessage("Role deleted successfully");
      await loadRoles();
    } catch (err) {
      setError(err.message);
    }
  }

  async function handleAssignRole(event) {
    event.preventDefault();
    setAssigning(true);
    setError("");
    setStatusMessage("");

    try {
      await assignRoleToUser(assignForm.userId.trim(), assignForm.roleName);
      setStatusMessage("Role assigned to user successfully");
      setAssignForm(initialAssignForm);
    } catch (err) {
      setError(err.message);
    } finally {
      setAssigning(false);
    }
  }

  return (
    <PageContainer
      title="Roles"
      subtitle="Admin role management integrated with /api/roles endpoints"
    >
      {!isAdmin ? (
        <Alert variant="danger">This page is for Admin users only.</Alert>
      ) : null}

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
        <Tab eventKey="list" title="Role List">
          <Card>
            <Card.Body>
              <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="h5 mb-0">All Roles</h2>
                <Button
                  variant="outline-primary"
                  onClick={loadRoles}
                  disabled={loading}
                >
                  Refresh
                </Button>
              </div>

              {loading ? (
                <LoadingState message="Loading roles..." />
              ) : (
                <div className="table-responsive">
                  <Table striped hover>
                    <thead>
                      <tr>
                        <th>Role ID</th>
                        <th>Name</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {sortedRoles.map((item) => (
                        <tr key={item.id}>
                          <td>{item.id}</td>
                          <td>{item.name}</td>
                          <td>
                            <div className="d-flex gap-2">
                              <Button
                                size="sm"
                                variant="warning"
                                onClick={() => handleEdit(item)}
                                disabled={!isAdmin}
                              >
                                Edit
                              </Button>
                              <Button
                                size="sm"
                                variant="danger"
                                onClick={() => handleDelete(item.id)}
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
          eventKey="manage"
          title={editingRoleId ? "Update Role" : "Create Role"}
        >
          <Card>
            <Card.Body>
              <h2 className="h5 mb-3">
                {editingRoleId ? "Update Role" : "Create New Role"}
              </h2>
              <Form onSubmit={handleSaveRole}>
                <Row className="g-3">
                  <Col xs={12} md={8}>
                    <Form.Group controlId="roleName">
                      <Form.Label>Role Name</Form.Label>
                      <Form.Control
                        name="roleName"
                        value={roleForm.roleName}
                        onChange={handleRoleInputChange}
                        required
                      />
                    </Form.Group>
                  </Col>
                </Row>

                <div className="d-flex gap-2 mt-3">
                  <Button type="submit" disabled={saving || !isAdmin}>
                    {saving
                      ? "Saving..."
                      : editingRoleId
                        ? "Update Role"
                        : "Create Role"}
                  </Button>
                  <Button
                    type="button"
                    variant="secondary"
                    onClick={resetRoleForm}
                  >
                    Clear
                  </Button>
                </div>
              </Form>

              {saving ? (
                <LoadingState message="Submitting role request..." />
              ) : null}
            </Card.Body>
          </Card>
        </Tab>

        <Tab eventKey="assign" title="Assign Role to User">
          <Card>
            <Card.Body>
              <h2 className="h5 mb-3">Assign Role by User ID</h2>
              <Form onSubmit={handleAssignRole}>
                <Row className="g-3">
                  <Col xs={12} md={6}>
                    <Form.Group controlId="userId">
                      <Form.Label>User ID (Identity ID)</Form.Label>
                      <Form.Control
                        name="userId"
                        value={assignForm.userId}
                        onChange={handleAssignInputChange}
                        required
                      />
                    </Form.Group>
                  </Col>
                  <Col xs={12} md={6}>
                    <Form.Group controlId="assignRoleName">
                      <Form.Label>Role Name</Form.Label>
                      <Form.Select
                        name="roleName"
                        value={assignForm.roleName}
                        onChange={handleAssignInputChange}
                        required
                      >
                        <option value="">Select role</option>
                        {sortedRoles.map((item) => (
                          <option key={item.id} value={item.name}>
                            {item.name}
                          </option>
                        ))}
                      </Form.Select>
                    </Form.Group>
                  </Col>
                </Row>

                <div className="d-flex gap-2 mt-3">
                  <Button type="submit" disabled={assigning || !isAdmin}>
                    {assigning ? "Assigning..." : "Assign Role"}
                  </Button>
                </div>
              </Form>

              {assigning ? (
                <LoadingState message="Submitting assignment..." />
              ) : null}
            </Card.Body>
          </Card>
        </Tab>
      </Tabs>
    </PageContainer>
  );
}

export default RolesPage;
