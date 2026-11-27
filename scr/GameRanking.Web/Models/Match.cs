namespace GameRanking.Web.Models;

public sealed class Match
{
    public int MatchId { get; set; }
    public Guid? RequestId { get; set; }
    public int WinnerPlayerId { get; set; }
    public int LoserPlayerId { get; set; }
    public DateTime PlayedAtUtc { get; set; }
}
