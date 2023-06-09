using JobBoards.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace JobBoards.Data.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }

    [JsonIgnore]
    public ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
    public Guid JobSeekerId { get; set; }
    public JobSeeker JobSeeker { get; set; } = default!;
}