using JobBoards.Data.Entities.Common;

namespace JobBoards.Data.Entities;

public class JobType : Entity<Guid>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private JobType(
        Guid jobTypeId,
        string name,
        string description,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt) : base(jobTypeId)
    {
        Name = name;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        DeletedAt = deletedAt;
    }

    public static JobType CreateNew(string name, string description)
    {
        return new(
            Guid.NewGuid(),
            name,
            description,
            DateTime.UtcNow,
            null,
            null);
    }
}