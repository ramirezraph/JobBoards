using JobBoards.Data.Contracts.JobSeekers;

namespace JobBoards.Data.Contracts.JobPost;

public class BasicJobApplicationResponse
{
    public Guid Id { get; set; }
    public ResumeResponse Resume { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}