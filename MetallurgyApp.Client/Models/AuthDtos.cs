using System.ComponentModel.DataAnnotations;

namespace MetallurgyApp.Client.Models;

public enum UserRole
{
    Viewer = 0,
    Operator = 1,
    Admin = 2
}

public class LoginDto
{
    [Required(ErrorMessage = "Введите логин")]
    [Display(Name = "Логин")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;
}

public class RegisterDto
{
    [Required(ErrorMessage = "Введите логин")]
    [Display(Name = "Логин")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Роль")]
    public UserRole Role { get; set; } = UserRole.Viewer;
}

public class TokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}