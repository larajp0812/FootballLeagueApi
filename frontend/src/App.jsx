import { Navigate, Route, Routes } from "react-router-dom";
import AppNavbar from "./components/AppNavbar";
import ProtectedRoute from "./components/ProtectedRoute";
import DashboardPage from "./pages/DashboardPage";
import LoginPage from "./pages/LoginPage";
import MatchEventsPage from "./pages/MatchEventsPage";
import MatchesPage from "./pages/MatchesPage";
import PlayersPage from "./pages/PlayersPage";
import RegisterPage from "./pages/RegisterPage";
import RolesPage from "./pages/RolesPage";
import SeasonsPage from "./pages/SeasonsPage";
import TeamsPage from "./pages/TeamsPage";
import VenuesPage from "./pages/VenuesPage";

function App() {
  return (
    <>
      <AppNavbar />
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route
          path="/"
          element={
            <ProtectedRoute>
              <DashboardPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/teams"
          element={
            <ProtectedRoute>
              <TeamsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/players"
          element={
            <ProtectedRoute>
              <PlayersPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/seasons"
          element={
            <ProtectedRoute>
              <SeasonsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/venues"
          element={
            <ProtectedRoute>
              <VenuesPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/matches"
          element={
            <ProtectedRoute>
              <MatchesPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/matchevents"
          element={
            <ProtectedRoute>
              <MatchEventsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/roles"
          element={
            <ProtectedRoute>
              <RolesPage />
            </ProtectedRoute>
          }
        />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </>
  );
}

export default App;
