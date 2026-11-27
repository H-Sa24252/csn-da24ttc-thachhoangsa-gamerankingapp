using GameRanking.Web.Models;

namespace GameRanking.Web.Services;

public static class PlayerRanking
{
    public static IReadOnlyList<RankedPlayer> ByScore(PublicPlayer[] players) =>
        Rank(players, CompareScore);

    public static IReadOnlyList<RankedPlayer> ByLevel(PublicPlayer[] players) =>
        Rank(players, CompareLevel);

    internal static Comparison<PublicPlayer> Criteria(RankingCriterion criterion) => criterion switch
    {
        RankingCriterion.Score => CompareScore,
        RankingCriterion.Level => CompareLevel,
        _ => throw new ArgumentOutOfRangeException(nameof(criterion))
    };

    private static int CompareScore(PublicPlayer left, PublicPlayer right)
    {
        var score = right.TotalScore.CompareTo(left.TotalScore);
        return score != 0 ? score : left.ScoreReachedAtUtc.CompareTo(right.ScoreReachedAtUtc);
    }

    private static int CompareLevel(PublicPlayer left, PublicPlayer right)
    {
        var level = right.Level.CompareTo(left.Level);
        if (level != 0)
            return level;

        var exp = right.TotalExp.CompareTo(left.TotalExp);
        return exp != 0 ? exp : left.ExpReachedAtUtc.CompareTo(right.ExpReachedAtUtc);
    }

    internal static Comparison<PublicPlayer> DisplayOrder(Comparison<PublicPlayer> compare) =>
        (left, right) =>
        {
            var order = compare(left, right);
            return order != 0 ? order : left.PlayerId.CompareTo(right.PlayerId);
        };

    private static RankedPlayer[] Rank(PublicPlayer[] players, Comparison<PublicPlayer> compare)
    {
        PlayerSorting.Merge(players, DisplayOrder(compare));
        return AssignRanks(players, compare);
    }

    internal static RankedPlayer[] AssignRanks(PublicPlayer[] players, Comparison<PublicPlayer> compare)
    {
        var results = new RankedPlayer[players.Length];
        var rank = 0;
        for (var index = 0; index < players.Length; index++)
        {
            if (index == 0 || compare(players[index - 1], players[index]) != 0)
                rank++;
            results[index] = new RankedPlayer(rank, players[index]);
        }
        return results;
    }
}
