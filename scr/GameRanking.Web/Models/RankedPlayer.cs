namespace GameRanking.Web.Models;

public sealed record PublicPlayer(
    int PlayerId, string DisplayName, int TotalScore, int TotalExp, int Wins, int Losses,
    DateTime ScoreReachedAtUtc, DateTime ExpReachedAtUtc)
{
    public int Level => 1 + TotalExp / 500;
    public long TotalMatches => (long)Wins + Losses;
    public decimal WinRate => TotalMatches == 0 ? 0 : (decimal)Wins / TotalMatches;
}

public sealed record RankedPlayer(int Rank, PublicPlayer Player);
