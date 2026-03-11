import { apiRequest } from "./apiClient";

export function getPlayers() {
  return apiRequest("/api/players");
}

export function createPlayer(player) {
  return apiRequest("/api/players", {
    method: "POST",
    body: JSON.stringify(player),
  });
}

export function updatePlayer(id, player) {
  return apiRequest(`/api/players/${id}`, {
    method: "PUT",
    body: JSON.stringify(player),
  });
}

export function deletePlayer(id) {
  return apiRequest(`/api/players/${id}`, {
    method: "DELETE",
  });
}
