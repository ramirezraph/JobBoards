using JobBoards.Data.Entities.Common;
using JobBoards.Data.Identity;
using Newtonsoft.Json;

namespace JobBoards.Data.Entities;

public class JobPost : Entity<Guid>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid JobLocationId { get; set; }
    public JobLocation JobLocation { get; set; } = default!;
    public double MinSalary { get; set; }
    public double MaxSalary { get; set; }
    public bool IsActive { get; set; }
    public Guid JobCategoryId { get; set; }
    public JobCategory JobCategory { get; set; } = default!;
    public Guid JobTypeId { get; set; }
    public JobType JobType { get; set; } = default!;
    public DateTime Expiration { get; set; }
    public string CreatedById { get; set; }
    public ApplicationUser CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    [JsonIgnore]
    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();

    private JobPost(
        Guid jobPostId,
        string title,
        string description,
        Guid jobLocationId,
        double minSalary,
        double maxSalary,
        bool isActive,
        Guid jobCategoryId,
        Guid jobTypeId,
        DateTime expiration,
        string createdById,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt) : base(jobPostId)
    {
        Title = title;
        Description = description;
        JobLocationId = jobLocationId;
        MinSalary = minSalary;
        MaxSalary = maxSalary;
        IsActive = isActive;
        JobCategoryId = jobCategoryId;
        JobTypeId = jobTypeId;
        Expiration = expiration;
        CreatedById = createdById;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        DeletedAt = deletedAt;
    }

    public static JobPost CreateNew(
        string title,
        string description,
        Guid jobLocationId,
        double minSalary,
        double maxSalary,
        bool isActive,
        Guid jobCategoryId,
        Guid jobTypeId,
        DateTime expiration,
        string createdById)
    {
        return new(
            Guid.NewGuid(),
            title,
            description,
            jobLocationId,
            minSalary,
            maxSalary,
            isActive,
            jobCategoryId,
            jobTypeId,
            expiration,
            createdById,
            DateTime.UtcNow,
            DateTime.UtcNow,
            null);
    }

#pragma warning disable CS8618
    public JobPost()
    {
    }
#pragma warning restore CS8618
}