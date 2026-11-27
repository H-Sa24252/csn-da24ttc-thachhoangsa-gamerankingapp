using System.ComponentModel.DataAnnotations;

namespace GameRanking.Web.Models;

public sealed class MatchSimulationInput
{
    [Range(1, int.MaxValue, ErrorMessage = "Chọn người chơi thứ nhất.")]
    [Display(Name = "Người chơi thứ nhất")]
    public int FirstPlayerId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Chọn người chơi thứ hai.")]
    [Display(Name = "Người chơi thứ hai")]
    public int SecondPlayerId { get; set; }

    public Guid RequestId { get; set; }
}

public sealed record MatchSimulationResult(int MatchId, string Winner, string Loser);
