namespace JobBoards.Data.Contracts.JobSeekers;

public class JobApplicationResponse
{
    public Guid Id { get; set; }
    public JobApplicationJobPostResponse JobPost { get; set; } = default!;
    public ResumeResponse Resume { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class JobApplicationJobPostResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Category { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Location { get; set; } = default!;
    public double MinSalary { get; set; }
    public double MaxSalary { get; set; }
}

