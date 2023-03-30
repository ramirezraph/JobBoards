using System.ComponentModel.DataAnnotations;

namespace JobBoards.WebApplication.ViewModels.Account;

public class RegisterViewModel
{
    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Password must have a minimum of eight characters, at least one letter, one number and one special character")]
    public string Password { get; set; }

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }

    public RegisterViewModel(string fullName, string email, string password, string confirmPassword)
    {
        FullName = fullName;
        Email = email;
        Password = password;
        ConfirmPassword = confirmPassword;
    }
#pragma warning disable CS8618
    public RegisterViewModel()
    {
    }
#pragma warning restore CS8618
}