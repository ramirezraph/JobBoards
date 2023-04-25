using JobBoards.Data.Entities.Common;

namespace JobBoards.Data.Entities;

public class JobApplication : Entity<Guid>
{
    public Guid JobPostId { get; set; }
    public JobPost JobPost { get; set; } = default!;
    public Guid JobSeekerId { get; set; }
    public JobSeeker JobSeeker { get; set; } = default!;
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    private JobApplication(
        Guid jobApplicationId,
        Guid jobPostId,
        Guid jobSeekerId,
        string status,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt) : base(jobApplicationId)
    {
        JobPostId = jobPostId;
        JobSeekerId = jobSeekerId;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        DeletedAt = deletedAt;
    }

    public static JobApplication CreateNew(
        Guid jobPostId,
        Guid jobSeekerId,
        string status)
    {
        return new(
            Guid.NewGuid(),
            jobPostId,
            jobSeekerId,
            "Submitted",
            DateTime.Now,
            DateTime.Now,
            null);
    }
#pragma warning disable CS8618
    public JobApplication()
    {
    }
#pragma warning restore CS8618
}