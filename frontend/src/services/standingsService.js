import { apiRequest } from "./apiClient";

export function getStandings(seasonId) {
  const query = seasonId ? `?seasonId=${encodeURIComponent(seasonId)}` : "";
  return apiRequest(`/api/standings${query}`);
}
