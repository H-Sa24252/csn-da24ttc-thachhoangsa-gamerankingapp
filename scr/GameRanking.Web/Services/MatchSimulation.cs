using GameRanking.Web.Data;
using GameRanking.Web.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GameRanking.Web.Services;

public sealed class MatchSimulation(GameRankingDbContext db)
{
    public async Task<MatchSimulationResult?> SimulateAsync(MatchSimulationInput input, Guid requestId,
        CancellationToken cancellationToken)
    {
        if (input.FirstPlayerId == input.SecondPlayerId)
            return null;

        const int maxAttempts = 4;
        for (var attempt = 1; ; attempt++)
        {
            var existing = await db.Matches.AsNoTracking()
                .SingleOrDefaultAsync(match => match.RequestId == requestId, cancellationToken);
            var players = await db.Players.Where(player => player.PlayerId == input.FirstPlayerId ||
                player.PlayerId == input.SecondPlayerId).ToDictionaryAsync(player => player.PlayerId, cancellationToken);
            if (players.Count != 2)
                return null;

            if (existing is not null)
            {
                return players.TryGetValue(existing.WinnerPlayerId, out var previousWinner) &&
                    players.TryGetValue(existing.LoserPlayerId, out var previousLoser)
                    ? new MatchSimulationResult(existing.MatchId, previousWinner.DisplayName, previousLoser.DisplayName)
                    : null;
            }

            var firstWins = Random.Shared.Next(2) == 0;
            var winner = players[firstWins ? input.FirstPlayerId : input.SecondPlayerId];
            var loser = players[firstWins ? input.SecondPlayerId : input.FirstPlayerId];
            var utcNow = DateTime.UtcNow;
            ApplyAwards(winner, true, utcNow);
            ApplyAwards(loser, false, utcNow);
            var match = new Match
            {
                RequestId = requestId,
                PlayedAtUtc = utcNow,
                WinnerPlayerId = winner.PlayerId,
                LoserPlayerId = loser.PlayerId
            };
            db.Matches.Add(match);

            try
            {
                // EF Core saves the match and both rowversion-protected updates in one transaction.
                await db.SaveChangesAsync(cancellationToken);
                return new MatchSimulationResult(match.MatchId, winner.DisplayName, loser.DisplayName);
            }
            catch (DbUpdateException error) when (attempt < maxAttempts &&
                (error is DbUpdateConcurrencyException || error.InnerException is SqlException { Number: 1205 or 2601 or 2627 }))
            {
                db.ChangeTracker.Clear();
            }
        }
    }

    private static void ApplyAwards(Player player, bool won, DateTime utcNow)
    {
        checked
        {
            player.TotalScore += MatchAwards.Score(won);
            player.TotalExp += MatchAwards.Exp(won);
            if (won)
                player.Wins++;
            else
                player.Losses++;
        }
        player.ScoreReachedAtUtc = utcNow;
        player.ExpReachedAtUtc = utcNow;
    }
}
