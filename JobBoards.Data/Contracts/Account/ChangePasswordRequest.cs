using System.ComponentModel.DataAnnotations;

namespace JobBoards.Data.Contracts.Account;

public class ChangePasswordRequest
{
    [Required]
    public string OldPassword { get; set; } = default!;
    [Required]
    public string NewPassword { get; set; } = default!;
}