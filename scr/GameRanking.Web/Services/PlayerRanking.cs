using GameRanking.Web.Models;

namespace GameRanking.Web.Services;

public static class PlayerRanking
{
    public static IReadOnlyList<RankedPlayer> ByScore(PublicPlayer[] players) =>
        Rank(players, CompareScore);

    public static IReadOnlyList<RankedPlayer> ByLevel(PublicPlayer[] players) =>
        Rank(players, CompareLevel);

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

    private static IReadOnlyList<RankedPlayer> Rank(PublicPlayer[] players, Comparison<PublicPlayer> compare)
    {
        var buffer = new PublicPlayer[players.Length];
        MergeSort(players, buffer, 0, players.Length, compare);
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

    private static void MergeSort(PublicPlayer[] players, PublicPlayer[] buffer, int start, int end,
        Comparison<PublicPlayer> compare)
    {
        if (end - start < 2)
            return;

        var middle = start + (end - start) / 2;
        MergeSort(players, buffer, start, middle, compare);
        MergeSort(players, buffer, middle, end, compare);

        var left = start;
        var right = middle;
        var index = start;
        while (left < middle && right < end)
        {
            var order = compare(players[left], players[right]);
            if (order == 0)
                order = players[left].PlayerId.CompareTo(players[right].PlayerId);
            buffer[index++] = order <= 0 ? players[left++] : players[right++];
        }
        while (left < middle)
            buffer[index++] = players[left++];
        while (right < end)
            buffer[index++] = players[right++];

        Array.Copy(buffer, start, players, start, end - start);
    }
}
