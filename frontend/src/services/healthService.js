const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? "https://localhost:5240";

export async function getApiHealth() {
  const response = await fetch(`${API_BASE_URL}/health`);

  if (!response.ok) {
    throw new Error(`Health check failed with status ${response.status}`);
  }

  return true;
}
