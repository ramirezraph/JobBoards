using System.ComponentModel.DataAnnotations;

namespace JobBoards.Data.Contracts.Account;

public class RegisterRequest
{
    [Required]
    [RegularExpression(@"^[\p{L}\s'-]+$", ErrorMessage = "Full name must only contain letters, spaces, hyphens, and apostrophes")]
    public string FullName { get; set; }

    [Required]
    public string Email { get; set; } = default!;

    [Required]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Password must have a minimum of eight characters, at least one letter, one number and one special character")]
    public string Password { get; set; } = default!;
}