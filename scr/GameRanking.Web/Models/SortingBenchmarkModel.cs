using System.ComponentModel.DataAnnotations;

namespace GameRanking.Web.Models;

public enum RankingCriterion
{
    [Display(Name = "Tổng điểm")]
    Score,
    [Display(Name = "Cấp độ")]
    Level
}

public enum SortingAlgorithm
{
    [Display(Name = "So sánh cả sáu")]
    All,
    [Display(Name = "Bubble Sort")]
    Bubble,
    [Display(Name = "Selection Sort")]
    Selection,
    [Display(Name = "Insertion Sort")]
    Insertion,
    [Display(Name = "Merge Sort")]
    Merge,
    [Display(Name = "Quick Sort")]
    Quick,
    [Display(Name = "Heap Sort")]
    Heap
}

public enum BenchmarkData
{
    [Display(Name = "Ngẫu nhiên")]
    Random,
    [Display(Name = "Đã sắp xếp")]
    Sorted,
    [Display(Name = "Thứ tự ngược")]
    Reversed,
    [Display(Name = "Nhiều giá trị trùng")]
    ManyTies
}

public sealed class BenchmarkInput
{
    public static IReadOnlyList<int> Sizes { get; } = Array.AsReadOnly(new[] { 100, 500, 1000, 5000, 10000 });

    [Display(Name = "Số người chơi")]
    public int Size { get; set; } = 100;

    [Display(Name = "Tiêu chí")]
    public RankingCriterion Criterion { get; set; }

    [Display(Name = "Thuật toán")]
    public SortingAlgorithm Algorithm { get; set; }

    [Display(Name = "Đặc điểm dữ liệu")]
    public BenchmarkData Data { get; set; }

    [Display(Name = "Seed")]
    public int Seed { get; set; } = 42;
}

public sealed record BenchmarkResult(string Name, double MedianMilliseconds, bool IsCorrect);

public sealed record SortingBenchmarkModel(BenchmarkInput Input, IReadOnlyList<BenchmarkResult> Results);
