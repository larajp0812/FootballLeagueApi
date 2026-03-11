import { apiRequest } from "./apiClient";

export function getRoles() {
  return apiRequest("/api/roles");
}

export function getRoleById(roleId) {
  return apiRequest(`/api/roles/${roleId}`);
}

export function createRole(roleName) {
  return apiRequest("/api/roles", {
    method: "POST",
    body: JSON.stringify({ roleName }),
  });
}

export function updateRole(roleId, newRoleName) {
  return apiRequest("/api/roles", {
    method: "PUT",
    body: JSON.stringify({ roleId, newRoleName }),
  });
}

export function deleteRole(roleId) {
  return apiRequest(`/api/roles/${roleId}`, {
    method: "DELETE",
  });
}

export function assignRoleToUser(userId, roleName) {
  return apiRequest("/api/roles/assign-role-to-user", {
    method: "POST",
    body: JSON.stringify({ userId, roleName }),
  });
}
