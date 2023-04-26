using System.ComponentModel.DataAnnotations;

namespace JobBoards.Data.Contracts.Account;

public class ChangePasswordRequest
{
    [Required]
    public string OldPassword { get; set; } = default!;
    
    [Required]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Password must have a minimum of eight characters, at least one letter, one number and one special character")]
    public string NewPassword { get; set; } = default!;
}