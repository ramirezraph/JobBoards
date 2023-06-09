using JobBoards.Data.Entities.Common;
using JobBoards.Data.Identity;
using Newtonsoft.Json;

namespace JobBoards.Data.Entities;

public class JobSeeker : Entity<Guid>
{
    public string UserId { get; set; }
    public ApplicationUser User { get; set; } = default!;
    public Guid ResumeId { get; set; }
    public Resume Resume { get; set; } = default!;

    [JsonIgnore]
    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();

    private JobSeeker(
        Guid jobSeekerId,
        string userId,
        Guid resumeId) : base(jobSeekerId)
    {
        UserId = userId;
        ResumeId = resumeId;
    }
    public static JobSeeker RegisterUserAsJobseeker(string userId)
    {
        return new(Guid.NewGuid(), userId, Guid.Empty);
    }
#pragma warning disable CS8618
    public JobSeeker()
    {
    }
#pragma warning restore CS8618

}