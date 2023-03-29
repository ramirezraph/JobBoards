using JobBoards.Data.Entities.Common;

namespace JobBoards.Data.Entities;

public class JobCategory : Entity<Guid>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private JobCategory(
        Guid jobCategoryId,
        string name,
        string description,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt) : base(jobCategoryId)
    {
        Name = name;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        DeletedAt = deletedAt;
    }

    public static JobCategory CreateNew(string name, string description)
    {
        return new(
            Guid.NewGuid(),
            name,
            description,
            DateTime.UtcNow,
            null,
            null);
    }

#pragma warning disable CS8618
    public JobCategory() { }
#pragma warning restore CS8618
}