using Microsoft.AspNetCore.Identity;

namespace JobBoards.Data.Identity;

public class ApplicationUser : IdentityUser
{
    public String FullName { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
}