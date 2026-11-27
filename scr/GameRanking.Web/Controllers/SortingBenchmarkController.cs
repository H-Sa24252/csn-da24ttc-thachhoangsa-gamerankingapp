using GameRanking.Web.Models;
using GameRanking.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameRanking.Web.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class SortingBenchmarkController : Controller
{
    private static readonly SemaphoreSlim BenchmarkSlot = new(1, 1);

    [HttpGet]
    public IActionResult Index() => View(new SortingBenchmarkModel(new BenchmarkInput(), []));

    [HttpPost]
    public IActionResult Index([Bind(Prefix = "Input")] BenchmarkInput input, CancellationToken cancellationToken)
    {
        if (!BenchmarkInput.Sizes.Contains(input.Size))
            ModelState.AddModelError("Input.Size", "Hãy chọn một trong các kích thước được hỗ trợ.");
        if (!ModelState.IsValid)
            return View(new SortingBenchmarkModel(input, []));

        if (!BenchmarkSlot.Wait(0))
        {
            ModelState.AddModelError(string.Empty, "Đang có lượt thử nghiệm khác. Vui lòng thử lại sau.");
            return View(new SortingBenchmarkModel(input, []));
        }

        try
        {
            return View(new SortingBenchmarkModel(input, SortingBenchmark.Run(input, cancellationToken)));
        }
        finally
        {
            BenchmarkSlot.Release();
        }
    }
}
