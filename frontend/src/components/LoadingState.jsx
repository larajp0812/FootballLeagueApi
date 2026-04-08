import Spinner from "react-bootstrap/Spinner";

/**
 * LoadingState Component - Displays a loading spinner with message
 *
 * Shows an animated spinner along with a text message to indicate loading state.
 * Used to provide visual feedback during async operations like API calls.
 *
 * @component
 * @param {Object} props - Component props
 * @param {string} [props.message="Loading..."] - Message to display next to spinner
 * @returns {React.ReactElement} Div containing spinner and message
 *
 * @example
 * {isLoading ? (
 *   <LoadingState message="Fetching teams..." />
 * ) : (
 *   <TeamsTable teams={teams} />
 * )}
 */
function LoadingState({ message = "Loading..." }) {
  return (
    <div className="d-flex align-items-center gap-2 py-2">
      <Spinner animation="border" size="sm" />
      <span>{message}</span>
    </div>
  );
}

export default LoadingState;
