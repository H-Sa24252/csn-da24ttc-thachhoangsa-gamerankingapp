using System.Diagnostics;
using GameRanking.Web.Models;

namespace GameRanking.Web.Services;

public static class SortingBenchmark
{
    public const int Runs = 5;

    private sealed record Sorter(SortingAlgorithm Algorithm, string Name,
        Action<PublicPlayer[], Comparison<PublicPlayer>> Sort);

    private static readonly Sorter[] Sorters =
    [
        new(SortingAlgorithm.Bubble, "Bubble Sort", PlayerSorting.Bubble),
        new(SortingAlgorithm.Selection, "Selection Sort", PlayerSorting.Selection),
        new(SortingAlgorithm.Insertion, "Insertion Sort", PlayerSorting.Insertion),
        new(SortingAlgorithm.Merge, "Merge Sort", PlayerSorting.Merge),
        new(SortingAlgorithm.Quick, "Quick Sort", PlayerSorting.Quick),
        new(SortingAlgorithm.Heap, "Heap Sort", PlayerSorting.Heap)
    ];

    public static IReadOnlyList<BenchmarkResult> Run(BenchmarkInput input, CancellationToken cancellationToken)
    {
        if (!BenchmarkInput.Sizes.Contains(input.Size) || !Enum.IsDefined(input.Algorithm)
            || !Enum.IsDefined(input.Criterion) || !Enum.IsDefined(input.Data))
            throw new ArgumentException("Cấu hình thử nghiệm không hợp lệ.", nameof(input));

        var criteria = PlayerRanking.Criteria(input.Criterion);
        var order = PlayerRanking.DisplayOrder(criteria);
        var original = Generate(input, order);
        var snapshot = (PublicPlayer[])original.Clone();
        var expected = ReferenceRanking(original, input.Criterion);
        var selected = Sorters.Where(sorter => input.Algorithm == SortingAlgorithm.All
            || sorter.Algorithm == input.Algorithm).ToArray();
        var samples = new double[selected.Length, Runs];
        var correct = Enumerable.Repeat(true, selected.Length).ToArray();

        var warmup = original[..Math.Min(original.Length, 128)];
        foreach (var sorter in selected)
            sorter.Sort((PublicPlayer[])warmup.Clone(), order);

        for (var run = 0; run < Runs; run++)
        {
            // Luân phiên vị trí chạy giữa các lượt, mỗi lần vẫn sắp xếp tuần tự.
            for (var offset = 0; offset < selected.Length; offset++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var index = (run + offset) % selected.Length;
                var working = (PublicPlayer[])original.Clone();
                var start = Stopwatch.GetTimestamp();
                selected[index].Sort(working, order);
                samples[index, run] = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
                correct[index] &= expected.SequenceEqual(PlayerRanking.AssignRanks(working, criteria));
            }
        }

        var originalUnchanged = original.SequenceEqual(snapshot);
        return selected.Select((sorter, index) => new BenchmarkResult(
            sorter.Name,
            Enumerable.Range(0, Runs).Select(run => samples[index, run]).Order().ElementAt(Runs / 2),
            correct[index] && originalUnchanged)).ToArray();
    }

    private static PublicPlayer[] Generate(BenchmarkInput input, Comparison<PublicPlayer> order)
    {
        var random = new Random(input.Seed);
        var epoch = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var manyTies = input.Data == BenchmarkData.ManyTies;
        var players = new PublicPlayer[input.Size];
        for (var i = 0; i < players.Length; i++)
        {
            var score = random.Next(manyTies ? 5 : 100) * 50;
            var exp = random.Next(manyTies ? 5 : 100) * 100;
            var scoreTime = epoch.AddMinutes(random.Next(manyTies ? 2 : 12));
            var expTime = epoch.AddMinutes(random.Next(manyTies ? 2 : 12));

            // Hai hồ sơ đầu bảo đảm có nhóm đồng hạng ở cả hai tiêu chí.
            players[i] = i == 1
                ? players[0] with { PlayerId = i + 1, DisplayName = $"Người chơi thử {i + 1}" }
                : new PublicPlayer(i + 1, $"Người chơi thử {i + 1}", score, exp, 0, 0, scoreTime, expTime);
        }

        if (input.Data is BenchmarkData.Sorted or BenchmarkData.Reversed)
        {
            Array.Sort(players, order);
            if (input.Data == BenchmarkData.Reversed)
                Array.Reverse(players);
        }
        else
        {
            random.Shuffle(players.AsSpan());
        }

        return players;
    }

    private static RankedPlayer[] ReferenceRanking(PublicPlayer[] original, RankingCriterion criterion)
    {
        // Đối chiếu độc lập bằng khóa nghiệp vụ và khóa hiển thị của LINQ.
        var keyed = original.Select(player => new
        {
            Player = player,
            Key = criterion == RankingCriterion.Score
                ? (-(long)player.TotalScore, 0, player.ScoreReachedAtUtc)
                : (-(1L + player.TotalExp / 500), -player.TotalExp, player.ExpReachedAtUtc)
        }).OrderBy(row => row.Key).ThenBy(row => row.Player.PlayerId);

        (long, int, DateTime)? previous = null;
        var rank = 0;
        var reference = new List<RankedPlayer>(original.Length);
        foreach (var row in keyed)
        {
            if (previous != row.Key)
                rank++;
            previous = row.Key;
            reference.Add(new RankedPlayer(rank, row.Player));
        }
        return reference.ToArray();
    }
}
