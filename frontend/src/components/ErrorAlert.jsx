import Alert from "react-bootstrap/Alert";

/**
 * ErrorAlert Component - Displays error messages to the user
 *
 * Shows a dismissible alert with error message. Auto-hides if no message is provided.
 * Used throughout the app to provide user-friendly error feedback.
 *
 * @component
 * @param {Object} props - Component props
 * @param {string} [props.message] - Error message to display. If falsy, component returns null
 * @param {Function} [props.onClose] - Callback function when alert is dismissed
 * @returns {React.ReactElement|null} Bootstrap Alert component or null if no message
 *
 * @example
 * const [error, setError] = useState('');
 * return (
 *   <ErrorAlert
 *     message={error}
 *     onClose={() => setError('')}
 *   />
 * );
 */
function ErrorAlert({ message, onClose }) {
  if (!message) return null;

  return (
    <Alert
      variant="danger"
      dismissible={Boolean(onClose)}
      onClose={onClose}
      transition={false}
    >
      {message}
    </Alert>
  );
}

export default ErrorAlert;
