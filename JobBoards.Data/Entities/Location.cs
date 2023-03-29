using JobBoards.Data.Entities.Common;

namespace JobBoards.Data.Entities;

public class Location : Entity<Guid>
{
    public string City { get; private set; }
    public string Country { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private Location(
        Guid locationId,
        string city,
        string country,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt) : base(locationId)
    {
        City = city;
        Country = country;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        DeletedAt = deletedAt;
    }

    public static Location CreateNew(string city, string country)
    {
        return new(
            Guid.NewGuid(),
            city,
            country,
            DateTime.UtcNow,
            null,
            null);
    }
}