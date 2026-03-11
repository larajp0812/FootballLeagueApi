import { apiRequest } from "./apiClient";

export function getMatchEvents() {
  return apiRequest("/api/matchevents");
}

export function createMatchEvent(matchEvent) {
  return apiRequest("/api/matchevents", {
    method: "POST",
    body: JSON.stringify(matchEvent),
  });
}

export function updateMatchEvent(id, matchEvent) {
  return apiRequest(`/api/matchevents/${id}`, {
    method: "PUT",
    body: JSON.stringify(matchEvent),
  });
}

export function deleteMatchEvent(id) {
  return apiRequest(`/api/matchevents/${id}`, {
    method: "DELETE",
  });
}
