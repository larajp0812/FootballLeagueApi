function createEmptyRow(team) {
  return {
    teamId: team.teamId,
    teamName: team.name,
    played: 0,
    won: 0,
    drawn: 0,
    lost: 0,
    goalsFor: 0,
    goalsAgainst: 0,
    goalDifference: 0,
    points: 0,
  };
}

function ensureRow(tableMap, teamsById, teamId) {
  if (!tableMap.has(teamId)) {
    const team = teamsById.get(teamId) ?? { teamId, name: `Team #${teamId}` };
    tableMap.set(teamId, createEmptyRow(team));
  }

  return tableMap.get(teamId);
}

export function buildLeagueTable(teams, matches) {
  const teamsById = new Map((teams ?? []).map((team) => [team.teamId, team]));
  const tableMap = new Map(
    (teams ?? []).map((team) => [team.teamId, createEmptyRow(team)]),
  );

  (matches ?? []).forEach((match) => {
    const home = ensureRow(tableMap, teamsById, match.homeTeamId);
    const away = ensureRow(tableMap, teamsById, match.awayTeamId);

    const homeScore = Number(match.homeScore ?? 0);
    const awayScore = Number(match.awayScore ?? 0);

    home.played += 1;
    away.played += 1;

    home.goalsFor += homeScore;
    home.goalsAgainst += awayScore;
    away.goalsFor += awayScore;
    away.goalsAgainst += homeScore;

    if (homeScore > awayScore) {
      home.won += 1;
      home.points += 3;
      away.lost += 1;
    } else if (homeScore < awayScore) {
      away.won += 1;
      away.points += 3;
      home.lost += 1;
    } else {
      home.drawn += 1;
      away.drawn += 1;
      home.points += 1;
      away.points += 1;
    }
  });

  const rows = [...tableMap.values()].map((row) => ({
    ...row,
    goalDifference: row.goalsFor - row.goalsAgainst,
  }));

  rows.sort((a, b) => {
    if (b.points !== a.points) return b.points - a.points;
    if (b.goalDifference !== a.goalDifference)
      return b.goalDifference - a.goalDifference;
    if (b.goalsFor !== a.goalsFor) return b.goalsFor - a.goalsFor;
    return a.teamName.localeCompare(b.teamName);
  });

  return rows;
}
