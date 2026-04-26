import { useCallback, useEffect, useState } from "react";
import Button from "react-bootstrap/Button";
import Card from "react-bootstrap/Card";
import Col from "react-bootstrap/Col";
import Form from "react-bootstrap/Form";
import Row from "react-bootstrap/Row";
import Table from "react-bootstrap/Table";
import ErrorAlert from "../components/ErrorAlert";
import LoadingState from "../components/LoadingState";
import PageContainer from "../components/PageContainer";
import { getSeasons } from "../services/seasonService";
import { getStandings } from "../services/standingsService";

function LeagueTablePage() {
  const [tableRows, setTableRows] = useState([]);
  const [seasons, setSeasons] = useState([]);
  const [selectedSeasonId, setSelectedSeasonId] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const loadData = useCallback(async () => {
    setLoading(true);
    setError("");

    try {
      const [standingsData, seasonsData] = await Promise.all([
        getStandings(selectedSeasonId || undefined),
        getSeasons(),
      ]);

      setTableRows(Array.isArray(standingsData) ? standingsData : []);
      setSeasons(Array.isArray(seasonsData) ? seasonsData : []);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [selectedSeasonId]);

  useEffect(() => {
    loadData();
  }, [selectedSeasonId, loadData]);

  return (
    <PageContainer
      title="League Table"
      className="app-page"
    >
      <ErrorAlert message={error} onClose={() => setError("")} />

      <Card>
        <Card.Body>
          <Row className="g-3 mb-3 align-items-end">
            <Col xs={12} md={6}>
              <Form.Group controlId="seasonFilter">
                <Form.Label>Season Filter</Form.Label>
                <Form.Select
                  value={selectedSeasonId}
                  onChange={(event) => setSelectedSeasonId(event.target.value)}
                >
                  <option value="">All Seasons</option>
                  {seasons.map((season) => (
                    <option key={season.seasonId} value={season.seasonId}>
                      {season.name}
                    </option>
                  ))}
                </Form.Select>
              </Form.Group>
            </Col>
            <Col xs={12} md={6} className="d-flex justify-content-md-end">
              <Button
                variant="outline-primary"
                onClick={loadData}
                disabled={loading}
              >
                Refresh
              </Button>
            </Col>
          </Row>

          {loading ? (
            <LoadingState message="Calculating league table..." />
          ) : (
            <div className="table-responsive">
              <Table striped hover>
                <thead>
                  <tr>
                    <th>Pos</th>
                    <th>Team</th>
                    <th>P</th>
                    <th>W</th>
                    <th>D</th>
                    <th>L</th>
                    <th>GF</th>
                    <th>GA</th>
                    <th>GD</th>
                    <th>Pts</th>
                  </tr>
                </thead>
                <tbody>
                  {tableRows.map((row) => (
                    <tr key={row.teamId}>
                      <td>{row.position}</td>
                      <td>{row.teamName}</td>
                      <td>{row.played}</td>
                      <td>{row.won}</td>
                      <td>{row.drawn}</td>
                      <td>{row.lost}</td>
                      <td>{row.goalsFor}</td>
                      <td>{row.goalsAgainst}</td>
                      <td>{row.goalDifference}</td>
                      <td>
                        <strong>{row.points}</strong>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </Table>
            </div>
          )}
        </Card.Body>
      </Card>
    </PageContainer>
  );
}

export default LeagueTablePage;
