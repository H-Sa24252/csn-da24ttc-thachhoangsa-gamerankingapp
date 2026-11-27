using System.ComponentModel.DataAnnotations;

namespace GameRanking.Web.Models;

public class LoginInput
{
    [Required(ErrorMessage = "Nhập email."), EmailAddress(ErrorMessage = "Email không hợp lệ."), StringLength(256)]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Nhập mật khẩu."), StringLength(128), DataType(DataType.Password)]
    public string Password { get; set; } = "";
}

public sealed class RegisterInput : LoginInput
{
    [Required(ErrorMessage = "Nhập tên hiển thị."), StringLength(80), Display(Name = "Tên hiển thị")]
    public string DisplayName { get; set; } = "";
}

public sealed class ProfileInput
{
    [Required(ErrorMessage = "Nhập tên hiển thị."), StringLength(80), Display(Name = "Tên hiển thị")]
    public string DisplayName { get; set; } = "";
}

public sealed record ProfileViewModel(ProfileInput Input, string Email, Player Player);
