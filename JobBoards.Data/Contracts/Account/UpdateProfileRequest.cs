using System.ComponentModel.DataAnnotations;

namespace JobBoards.Data.Contracts.Account;

public class UpdateProfileRequest
{
    [Required]
    public string FullName { get; set; } = default!;
}