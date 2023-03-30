using JobBoards.Data.Entities.Common;

namespace JobBoards.Data.Entities;

public class JobPost : Entity<Guid>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid JobLocationId { get; set; }
    public double MinSalary { get; set; }
    public double MaxSalary { get; set; }
    public bool IsActive { get; set; }
    public Guid JobCategoryId { get; set; }
    public Guid JobTypeId { get; set; }
    public DateTime Expiration { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

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
        DateTime expiration)
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
            DateTime.UtcNow,
            null,
            null);
    }
}