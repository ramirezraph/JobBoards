using System.ComponentModel.DataAnnotations;

namespace JobBoards.Data.Contracts.Account;

public class RegisterRequest
{
    [Required]
    public string FullName { get; set; } = default!;
    [Required]
    public string Email { get; set; } = default!;
    [Required]
    public string Password { get; set; } = default!;
}