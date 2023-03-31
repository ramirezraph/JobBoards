using JobBoards.Data.Entities.Common;
using JobBoards.Data.Identity;

namespace JobBoards.Data.Entities;

public class JobSeeker : Entity<Guid>
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = default!;
    public Guid ResumeId { get; set; }

    public JobSeeker(
        Guid jobSeekerId,
        Guid userId,
        ApplicationUser user,
        Guid resumeId) : base(jobSeekerId)
    {
        UserId = userId;
        User = user;
        ResumeId = resumeId;
    }
}