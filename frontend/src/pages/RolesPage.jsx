import { useCallback, useEffect, useMemo, useState } from "react";
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
  deleteUserById,
  getRoles,
  getUsers,
} from "../services/roleService";

const initialAssignForm = {
  userId: "",
  roleName: "",
};

function RolesPage() {
  const { role } = useAuth();
  const [roles, setRoles] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [usersLoading, setUsersLoading] = useState(false);
  const [assigning, setAssigning] = useState(false);
  const [deletingUserId, setDeletingUserId] = useState("");
  const [error, setError] = useState("");
  const [statusMessage, setStatusMessage] = useState("");
  const [assignForm, setAssignForm] = useState(initialAssignForm);
  const [activeTab, setActiveTab] = useState("list");

  const isAdmin = typeof role === "string" && role.toLowerCase() === "admin";

  function getRolesErrorMessage(err, fallback) {
    if (err?.status === 401) {
      return "Unauthorized for roles endpoint. Please log out and log back in with an Admin account.";
    }

    return err?.message || fallback;
  }

  const sortedRoles = useMemo(() => {
    const source = Array.isArray(roles) ? roles : [];
    return [...source].sort((a, b) =>
      (a.name || "").localeCompare(b.name || ""),
    );
  }, [roles]);

  const loadRoles = useCallback(async () => {
    setLoading(true);
    setError("");

    try {
      const data = await getRoles();
      setRoles(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(getRolesErrorMessage(err, "Failed to load roles"));
    } finally {
      setLoading(false);
    }
  }, []);

  const loadUsers = useCallback(async () => {
    setUsersLoading(true);
    setError("");

    try {
      const data = await getUsers();
      setUsers(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(getRolesErrorMessage(err, "Failed to load users"));
    } finally {
      setUsersLoading(false);
    }
  }, []);

  useEffect(() => {
    if (!isAdmin) {
      setRoles([]);
      setUsers([]);
      return;
    }

    loadRoles();
    loadUsers();
  }, [isAdmin, loadRoles, loadUsers]);

  async function handleDeleteUser(userId, userName) {
    if (!window.confirm(`Delete user ${userName}? This cannot be undone.`)) {
      return;
    }

    setDeletingUserId(userId);
    setError("");
    setStatusMessage("");

    try {
      await deleteUserById(userId);
      setStatusMessage("User deleted successfully");
      await loadUsers();
    } catch (err) {
      setError(getRolesErrorMessage(err, "Failed to delete user"));
    } finally {
      setDeletingUserId("");
    }
  }

  function handleAssignInputChange(event) {
    const { name, value } = event.target;
    setAssignForm((current) => ({ ...current, [name]: value }));
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
      setError(getRolesErrorMessage(err, "Failed to assign role"));
    } finally {
      setAssigning(false);
    }
  }

  if (!isAdmin) {
    return (
      <PageContainer
        title="Roles"
        subtitle="Admin role management integrated with /api/roles endpoints"
        className="app-page"
      >
        <Alert variant="warning" className="mb-0">
          This page is for Admin users only. If you should have access, log out
          and log back in with the Admin account.
        </Alert>
      </PageContainer>
    );
  }

  return (
    <PageContainer
      title="Roles"
      subtitle="Admin role management integrated with /api/roles endpoints"
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
        <Tab eventKey="list" title="Role List">
          <Card>
            <Card.Body>
              <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="h5 mb-0">System Roles</h2>
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
                      </tr>
                    </thead>
                    <tbody>
                      {sortedRoles.map((item) => (
                        <tr key={item.id}>
                          <td>{item.id}</td>
                          <td>{item.name}</td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                </div>
              )}
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

        <Tab eventKey="users" title="User Accounts">
          <Card>
            <Card.Body>
              <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="h5 mb-0">All Registered Users</h2>
                <Button
                  variant="outline-primary"
                  onClick={loadUsers}
                  disabled={usersLoading}
                >
                  Refresh
                </Button>
              </div>

              {usersLoading ? (
                <LoadingState message="Loading users..." />
              ) : (
                <div className="table-responsive">
                  <Table striped hover>
                    <thead>
                      <tr>
                        <th>User ID</th>
                        <th>Username</th>
                        <th>Email</th>
                        <th>Roles</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {users.map((user) => (
                        <tr key={user.userId}>
                          <td>{user.userId}</td>
                          <td>{user.userName}</td>
                          <td>{user.email}</td>
                          <td>{user.roles?.join(", ") || "User"}</td>
                          <td>
                            <Button
                              size="sm"
                              variant="danger"
                              onClick={() =>
                                handleDeleteUser(user.userId, user.userName)
                              }
                              disabled={
                                deletingUserId === user.userId || !isAdmin
                              }
                            >
                              {deletingUserId === user.userId
                                ? "Deleting..."
                                : "Delete"}
                            </Button>
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
      </Tabs>
    </PageContainer>
  );
}

export default RolesPage;
