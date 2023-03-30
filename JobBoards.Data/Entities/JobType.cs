using JobBoards.Data.Entities.Common;

namespace JobBoards.Data.Entities;

public class JobType : Entity<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    private JobType(
        Guid jobTypeId,
        string name,
        string? description,
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

    public static JobType CreateNew(string name, string? description)
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
    public JobType() { }
#pragma warning restore CS8618
}