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
}