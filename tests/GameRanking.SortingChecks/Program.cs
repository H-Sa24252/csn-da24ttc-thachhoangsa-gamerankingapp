using GameRanking.Web.Models;
using GameRanking.Web.Services;

var epoch = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
var later = epoch.AddMinutes(1);
PublicPlayer[] fixture =
[
    new(5, "A", 100, 550, 0, 0, later, later),
    new(4, "B", 100, 600, 0, 0, epoch, later),
    new(3, "C", 90, 600, 0, 0, epoch, epoch),
    new(2, "D", 100, 499, 0, 0, epoch, epoch),
    new(1, "E", 80, 600, 0, 0, later, epoch)
];

CheckRanks(PlayerRanking.ByScore((PublicPlayer[])fixture.Clone()), [2, 4, 5, 3, 1], [1, 1, 2, 3, 4]);
CheckRanks(PlayerRanking.ByLevel((PublicPlayer[])fixture.Clone()), [1, 3, 4, 5, 2], [1, 1, 2, 3, 4]);

var sorterMethods = new Action<PublicPlayer[], Comparison<PublicPlayer>>[]
{
    PlayerSorting.Bubble, PlayerSorting.Selection, PlayerSorting.Insertion,
    PlayerSorting.Merge, PlayerSorting.Quick, PlayerSorting.Heap
};
foreach (var sort in sorterMethods)
{
    foreach (var criterion in Enum.GetValues<RankingCriterion>())
    {
        foreach (var input in new PublicPlayer[][] { [], [fixture[0]], fixture[..2], fixture,
                     fixture.Reverse().ToArray(), [fixture[0], fixture[0], fixture[0]] })
        {
            var snapshot = (PublicPlayer[])input.Clone();
            var expected = input.OrderBy(p => Key(p, criterion)).ThenBy(p => p.PlayerId).ToArray();
            Comparison<PublicPlayer> order = (a, b) =>
            {
                var comparison = Key(a, criterion).CompareTo(Key(b, criterion));
                return comparison != 0 ? comparison : a.PlayerId.CompareTo(b.PlayerId);
            };
            var working = (PublicPlayer[])input.Clone();
            sort(working, order);
            Require(working.SequenceEqual(expected) && input.SequenceEqual(snapshot),
                $"{sort.Method.Name}, {criterion}, {input.Length}");
        }
    }
    Console.WriteLine($"PASS {sort.Method.Name} (12 ca)");
}

foreach (var criterion in Enum.GetValues<RankingCriterion>())
foreach (var data in Enum.GetValues<BenchmarkData>())
{
    var results = SortingBenchmark.Run(new BenchmarkInput { Criterion = criterion, Data = data },
        CancellationToken.None);
    Require(results.Count == sorterMethods.Length && results.All(result => result.IsCorrect),
        $"{criterion}, {data}");
}

var quickStress = Enumerable.Range(0, 100_000).Select(id => fixture[0] with { PlayerId = id }).ToArray();
Array.Reverse(quickStress);
PlayerSorting.Quick(quickStress, (a, b) => a.PlayerId.CompareTo(b.PlayerId));
Require(quickStress.Select(player => player.PlayerId).SequenceEqual(Enumerable.Range(0, quickStress.Length)),
    "Quick Sort 100.000 phần tử ngược");
PlayerSorting.Quick(quickStress, (_, _) => 0);
Require(quickStress.Select(player => player.PlayerId).Distinct().Count() == quickStress.Length,
    "Quick Sort 100.000 khóa bằng nhau");
Console.WriteLine("PASS 2 bảng chính, 48 kết quả benchmark và 2 ca Quick Sort lớn.");

static (int, int, DateTime) Key(PublicPlayer player, RankingCriterion criterion) =>
    criterion == RankingCriterion.Score
        ? (-player.TotalScore, 0, player.ScoreReachedAtUtc)
        : (-player.Level, -player.TotalExp, player.ExpReachedAtUtc);

static void CheckRanks(IReadOnlyList<RankedPlayer> rows, int[] ids, int[] ranks) =>
    Require(rows.Select(row => row.Player.PlayerId).SequenceEqual(ids)
        && rows.Select(row => row.Rank).SequenceEqual(ranks), "Thứ tự và Dense Ranking");

static void Require(bool passed, string name)
{
    if (!passed)
        throw new InvalidOperationException(name);
}
