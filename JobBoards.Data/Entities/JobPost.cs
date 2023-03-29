using JobBoards.Data.Entities.Common;

namespace JobBoards.Data.Entities;

public class JobPost : Entity<Guid>
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Guid JobLocationId { get; private set; }
    public double MinSalary { get; private set; }
    public double MaxSalary { get; private set; }
    public bool IsActive { get; private set; }
    public Guid JobCategoryId { get; private set; }
    public Guid JobTypeId { get; private set; }
    public DateTime Expiration { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

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