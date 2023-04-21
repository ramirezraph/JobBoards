namespace JobBoards.Data.Contracts.JobPost;

public class JobPostResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public JobPostLocationResponse JobLocation { get; set; } = default!;
    public int MinSalary { get; set; }
    public int MaxSalary { get; set; }
    public bool IsActive { get; set; }
    public JobPostCategoryResponse JobCategory { get; set; } = default!;
    public JobPostTypeResponse JobType { get; set; } = default!;
    public DateTime Expiration { get; set; }
    public JobPostCreatedByResponse CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
}

public class JobPostLocationResponse
{
    public Guid Id { get; set; }
    public string City { get; set; } = default!;
    public string County { get; set; } = default!;
}

public class JobPostCategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
}

public class JobPostTypeResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
}

public class JobPostCreatedByResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}