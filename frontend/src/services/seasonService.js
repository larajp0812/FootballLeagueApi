import { apiRequest } from "./apiClient";

export function getSeasons() {
  return apiRequest("/api/seasons");
}

export function createSeason(season) {
  return apiRequest("/api/seasons", {
    method: "POST",
    body: JSON.stringify(season),
  });
}

export function updateSeason(id, season) {
  return apiRequest(`/api/seasons/${id}`, {
    method: "PUT",
    body: JSON.stringify(season),
  });
}

export function deleteSeason(id) {
  return apiRequest(`/api/seasons/${id}`, {
    method: "DELETE",
  });
}
