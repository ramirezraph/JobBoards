using JobBoards.Data.Entities.Common;

namespace JobBoards.Data.Entities;

public class Resume : Entity<Guid>
{
    public Guid JobSeekerId { get; set; }
    public string DownloadLink { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    private Resume(
        Guid resumeId,
        Guid jobSeekerId,
        string downloadLink,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt) : base(resumeId)
    {
        JobSeekerId = jobSeekerId;
        DownloadLink = downloadLink;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        DeletedAt = deletedAt;
    }

    public static Resume CreateNew(Guid jobSeekerId, string downloadLink)
    {
        return new(
            Guid.NewGuid(),
            jobSeekerId,
            downloadLink,
            DateTime.UtcNow,
            null,
            null);
    }

#pragma warning disable CS8618
    public Resume() { }
#pragma warning restore CS8618
}