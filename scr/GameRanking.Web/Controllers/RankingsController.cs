using GameRanking.Web.Data;
using GameRanking.Web.Models;
using GameRanking.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameRanking.Web.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class RankingsController(GameRankingDbContext db) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Score(bool rows, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Bảng xếp hạng tổng điểm";
        var ranking = PlayerRanking.ByScore(await ReadPlayersAsync(cancellationToken));
        return rows ? PartialView("_Rows", ranking) : View("Index", ranking);
    }

    [HttpGet]
    public async Task<IActionResult> Level(bool rows, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Bảng xếp hạng cấp độ";
        var ranking = PlayerRanking.ByLevel(await ReadPlayersAsync(cancellationToken));
        return rows ? PartialView("_Rows", ranking) : View("Index", ranking);
    }

    private Task<PublicPlayer[]> ReadPlayersAsync(CancellationToken cancellationToken) => db.Players
        .Select(item => new PublicPlayer(item.PlayerId, item.DisplayName, item.TotalScore,
            item.TotalExp, item.Wins, item.Losses, item.ScoreReachedAtUtc, item.ExpReachedAtUtc))
        .ToArrayAsync(cancellationToken);
}
