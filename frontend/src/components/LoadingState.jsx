import Spinner from "react-bootstrap/Spinner";

function LoadingState({ message = "Loading..." }) {
  return (
    <div className="d-flex align-items-center gap-2 py-2">
      <Spinner animation="border" size="sm" />
      <span>{message}</span>
    </div>
  );
}

export default LoadingState;
