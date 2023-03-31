using System.ComponentModel.DataAnnotations;

namespace JobBoards.WebApplication.ViewModels.Account;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;

    public LoginViewModel()
    {
    }
}