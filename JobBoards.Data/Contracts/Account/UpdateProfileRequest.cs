using System.ComponentModel.DataAnnotations;

namespace JobBoards.Data.Contracts.Account;

public class UpdateProfileRequest
{
    [Required]
    [RegularExpression(@"^[\p{L}\s'-]+$", ErrorMessage = "Full name must only contain letters, spaces, hyphens, and apostrophes")]
    public string FullName { get; set; }

}