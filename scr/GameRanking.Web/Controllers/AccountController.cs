using GameRanking.Web.Data;
using GameRanking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GameRanking.Web.Controllers;

public sealed class AccountController(
    GameRankingDbContext db,
    UserManager<IdentityUser> users,
    SignInManager<IdentityUser> signIn) : Controller
{
    [HttpGet]
    public IActionResult Register() => User.Identity?.IsAuthenticated == true
        ? RedirectToAction(nameof(Profile))
        : View(new RegisterInput());

    [HttpPost]
    public async Task<IActionResult> Register(RegisterInput input)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction(nameof(Profile));
        if (!ModelState.IsValid)
            return View(input);

        var email = input.Email.Trim();
        var user = new IdentityUser { UserName = email, Email = email };
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var result = await users.CreateAsync(user, input.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(input);
            }

            db.Players.Add(Player.Create(input.DisplayName.Trim(), DateTime.UtcNow, user.Id));
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException error) when (error.InnerException is SqlException { Number: 2601 or 2627 })
        {
            ModelState.AddModelError("", "Email này đã được sử dụng.");
            return View(input);
        }

        await signIn.SignInAsync(user, isPersistent: false);
        return RedirectToAction(nameof(Profile));
    }

    [HttpGet]
    public IActionResult Login() => User.Identity?.IsAuthenticated == true
        ? RedirectToAction(nameof(Profile))
        : View(new LoginInput());

    [HttpPost]
    public async Task<IActionResult> Login(LoginInput input)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction(nameof(Profile));
        if (!ModelState.IsValid)
            return View(input);

        var result = await signIn.PasswordSignInAsync(input.Email.Trim(), input.Password, false, lockoutOnFailure: true);
        if (result.Succeeded)
            return RedirectToAction(nameof(Profile));

        ModelState.AddModelError("", "Không thể đăng nhập. Kiểm tra email và mật khẩu rồi thử lại.");
        return View(input);
    }

    [Authorize, HttpPost]
    public async Task<IActionResult> Logout()
    {
        await signIn.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [Authorize, HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await users.GetUserAsync(User);
        if (user is null)
            return Challenge();

        var player = await db.Players.AsNoTracking().SingleOrDefaultAsync(item => item.UserId == user.Id);
        if (player is null)
            return NotFound();

        return View(new ProfileViewModel(new ProfileInput { DisplayName = player.DisplayName }, user.Email!, player));
    }

    [Authorize, HttpPost]
    public async Task<IActionResult> Profile([Bind(Prefix = "Input")] ProfileInput input)
    {
        var user = await users.GetUserAsync(User);
        if (user is null)
            return Challenge();

        var player = await db.Players.SingleOrDefaultAsync(item => item.UserId == user.Id);
        if (player is null)
            return NotFound();

        if (ModelState.IsValid)
        {
            player.DisplayName = input.DisplayName.Trim();
            try
            {
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Profile));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Hồ sơ vừa được cập nhật. Tải lại trang và thử lại.");
            }
        }

        return View(new ProfileViewModel(input, user.Email!, player));
    }
}
