using GameRanking.Web.Data;
using GameRanking.Web.Models;
using GameRanking.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameRanking.Web.Controllers;

public sealed class RankingsController(GameRankingDbContext db) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Score()
    {
        ViewData["Title"] = "Bảng xếp hạng tổng điểm";
        return View("Index", PlayerRanking.ByScore(await ReadPlayersAsync()));
    }

    [HttpGet]
    public async Task<IActionResult> Level()
    {
        ViewData["Title"] = "Bảng xếp hạng cấp độ";
        return View("Index", PlayerRanking.ByLevel(await ReadPlayersAsync()));
    }

    private Task<PublicPlayer[]> ReadPlayersAsync() => db.Players
        .Select(item => new PublicPlayer(item.PlayerId, item.DisplayName, item.TotalScore,
            item.TotalExp, item.Wins, item.Losses, item.ScoreReachedAtUtc, item.ExpReachedAtUtc))
        .ToArrayAsync();
}
