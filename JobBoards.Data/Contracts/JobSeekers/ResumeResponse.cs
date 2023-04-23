namespace JobBoards.Data.Contracts.JobSeekers;

public class ResumeResponse
{
    public Guid Id { get; set; }
    public Uri Uri { get; set; } = default!;
    public string FileName { get; set; } = default!;
}