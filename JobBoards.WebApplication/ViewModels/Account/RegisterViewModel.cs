using System.ComponentModel.DataAnnotations;

namespace JobBoards.WebApplication.ViewModels.Account;

public class RegisterViewModel
{
    [Required]
    public string FullName { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Password must have a minimum of eight characters, at least one letter, one number and one special character")]
    public string Password { get; set; } = default!;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = default!;

    public RegisterViewModel()
    {
    }
}