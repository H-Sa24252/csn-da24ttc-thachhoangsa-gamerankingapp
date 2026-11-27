namespace GameRanking.Web.Models;

public sealed class Player
{
    public int PlayerId { get; set; }
    public string? UserId { get; set; }
    public required string DisplayName { get; set; }
    public int TotalScore { get; set; }
    public int TotalExp { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public DateTime ScoreReachedAtUtc { get; set; }
    public DateTime ExpReachedAtUtc { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public int Level => 1 + TotalExp / 500;
    public long TotalMatches => (long)Wins + Losses;
    public decimal WinRate => TotalMatches == 0 ? 0 : (decimal)Wins / TotalMatches;

    public static Player Create(string displayName, DateTime utcNow, string? userId = null) => new()
    {
        DisplayName = displayName,
        UserId = userId,
        ScoreReachedAtUtc = utcNow,
        ExpReachedAtUtc = utcNow
    };
}
