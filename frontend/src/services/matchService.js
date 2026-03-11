import { apiRequest } from "./apiClient";

export function getMatches() {
  return apiRequest("/api/matches");
}

export function createMatch(match) {
  return apiRequest("/api/matches", {
    method: "POST",
    body: JSON.stringify(match),
  });
}

export function updateMatch(id, match) {
  return apiRequest(`/api/matches/${id}`, {
    method: "PUT",
    body: JSON.stringify(match),
  });
}

export function deleteMatch(id) {
  return apiRequest(`/api/matches/${id}`, {
    method: "DELETE",
  });
}
