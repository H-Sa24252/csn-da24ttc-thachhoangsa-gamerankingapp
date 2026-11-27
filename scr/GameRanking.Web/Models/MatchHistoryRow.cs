namespace GameRanking.Web.Models;

public sealed record MatchHistoryRow(int MatchId, DateTime PlayedAtUtc, string Opponent, bool Won);
