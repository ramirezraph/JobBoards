using JobBoards.Data.Entities.Common;

namespace JobBoards.Data.Entities;

public class JobCategory : Entity<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    private JobCategory(
        Guid jobCategoryId,
        string name,
        string? description,
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

    public static JobCategory CreateNew(string name, string? description)
    {
        return new(
            Guid.NewGuid(),
            name,
            description,
            DateTime.Now,
            null,
            null);
    }

#pragma warning disable CS8618
    public JobCategory() { }
#pragma warning restore CS8618
}