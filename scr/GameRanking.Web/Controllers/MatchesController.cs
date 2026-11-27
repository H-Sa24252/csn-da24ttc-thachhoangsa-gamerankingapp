using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GameRanking.Web.Data;
using GameRanking.Web.Models;
using GameRanking.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GameRanking.Web.Controllers;

[Authorize]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class MatchesController(GameRankingDbContext db, MatchSimulation simulation,
    ILogger<MatchesController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        await LoadPlayersAsync(cancellationToken);
        return View(new MatchSimulationInput { RequestId = Guid.NewGuid() });
    }

    [HttpPost]
    public async Task<IActionResult> Index(MatchSimulationInput input, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Challenge();

        if (input.RequestId == Guid.Empty)
            ModelState.AddModelError("", "Biểu mẫu không hợp lệ. Tải lại trang và thử lại.");
        if (input.FirstPlayerId > 0 && input.FirstPlayerId == input.SecondPlayerId)
            ModelState.AddModelError("", "Hãy chọn hai người chơi khác nhau.");

        if (ModelState.IsValid)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{userId}:{input.RequestId:N}"));
            var requestId = new Guid(hash.AsSpan(0, 16));
            try
            {
                var result = await simulation.SimulateAsync(input, requestId, cancellationToken);
                if (result is not null)
                {
                    ViewData["Result"] = result;
                    ModelState.Remove(nameof(MatchSimulationInput.RequestId));
                    input.RequestId = Guid.NewGuid();
                }
                else
                {
                    ModelState.AddModelError("", "Cặp người chơi không tồn tại hoặc biểu mẫu đã được dùng cho một cặp khác.");
                }
            }
            catch (Exception error) when (error is DbUpdateException or OverflowException)
            {
                logger.LogWarning(error, "Không thể lưu trận mô phỏng.");
                ModelState.AddModelError("", "Chưa thể lưu kết quả. Bạn có thể thử lại với cùng biểu mẫu.");
            }
        }

        await LoadPlayersAsync(cancellationToken);
        return View(input);
    }

    private async Task LoadPlayersAsync(CancellationToken cancellationToken)
    {
        ViewBag.Players = await db.Players.AsNoTracking().OrderBy(player => player.DisplayName)
            .ThenBy(player => player.PlayerId)
            .Select(player => new SelectListItem(player.DisplayName + " (#" + player.PlayerId + ")",
                player.PlayerId.ToString())).ToListAsync(cancellationToken);
    }
}
