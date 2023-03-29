using JobBoards.Data.Entities.Common;

namespace JobBoards.Data.Entities;

public class Resume : Entity<Guid>
{
    public Guid JobSeekerId { get; private set; }
    public string DownloadLink { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

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
}