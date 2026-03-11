import { apiRequest } from "./apiClient";

export function getTeams() {
  return apiRequest("/api/teams");
}

export function createTeam(team) {
  return apiRequest("/api/teams", {
    method: "POST",
    body: JSON.stringify(team),
  });
}

export function updateTeam(id, team) {
  return apiRequest(`/api/teams/${id}`, {
    method: "PUT",
    body: JSON.stringify(team),
  });
}

export function deleteTeam(id) {
  return apiRequest(`/api/teams/${id}`, {
    method: "DELETE",
  });
}
