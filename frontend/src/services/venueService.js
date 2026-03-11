import { apiRequest } from "./apiClient";

export function getVenues() {
  return apiRequest("/api/venues");
}

export function createVenue(venue) {
  return apiRequest("/api/venues", {
    method: "POST",
    body: JSON.stringify(venue),
  });
}

export function updateVenue(id, venue) {
  return apiRequest(`/api/venues/${id}`, {
    method: "PUT",
    body: JSON.stringify(venue),
  });
}

export function deleteVenue(id) {
  return apiRequest(`/api/venues/${id}`, {
    method: "DELETE",
  });
}
