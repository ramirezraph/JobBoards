using JobBoards.Data.Entities.Common;

namespace JobBoards.Data.Entities;

public class Resume : Entity<Guid>
{
    public Guid JobSeekerId { get; set; }
    public JobSeeker JobSeeker { get; set; } = default!;
    public Uri Uri { get; set; }
    public string FileName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    private Resume(
        Guid resumeId,
        Guid jobSeekerId,
        Uri uri,
        string fileName,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt) : base(resumeId)
    {
        JobSeekerId = jobSeekerId;
        Uri = uri;
        FileName = fileName;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        DeletedAt = deletedAt;
        FileName = fileName;
    }

    public static Resume CreateNew(Guid jobSeekerId, Uri uri, string fileName)
    {
        return new(
            Guid.NewGuid(),
            jobSeekerId,
            uri,
            fileName,
            DateTime.UtcNow,
            null,
            null);
    }

#pragma warning disable CS8618
    public Resume() { }
#pragma warning restore CS8618
}