using System.Data;
using GameRanking.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GameRanking.Web.Data;

public static class DemoPlayers
{
    public static async Task SeedAsync(GameRankingDbContext db)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        // An update lock prevents two starters from both deciding to seed an empty table.
        if (!await db.Players.FromSqlRaw("SELECT * FROM dbo.Players WITH (UPDLOCK, HOLDLOCK)").AnyAsync())
        {
            var utcNow = DateTime.UtcNow;
            for (var number = 1; number <= 10; number++)
            {
                db.Players.Add(Player.Create($"Người chơi mẫu {number:D2}", utcNow));
            }
            await db.SaveChangesAsync();
        }
        await transaction.CommitAsync();
    }
}
