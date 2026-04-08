import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

/**
 * ProtectedRoute Component - Route guard for authenticated pages
 *
 * Wrapper component that protects routes requiring authentication.
 * Redirects unauthenticated users to login page while preserving the
 * destination URL in location state for post-login redirect.
 *
 * @component
 * @param {Object} props - Component props
 * @param {React.ReactNode} props.children - Page content to render if authenticated
 * @returns {React.ReactElement} Either the children or Navigate to login page
 *
 * @example
 * <Route
 *   path="/teams"
 *   element={
 *     <ProtectedRoute>
 *       <TeamsPage />
 *     </ProtectedRoute>
 *   }
 * />
 */
function ProtectedRoute({ children }) {
  const { isAuthenticated } = useAuth();
  const location = useLocation();

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return children;
}

export default ProtectedRoute;
