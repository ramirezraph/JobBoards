using System.ComponentModel.DataAnnotations;

namespace JobBoards.Data.Contracts.JobPost;
public class CreateJobPostRequest
{
    [Required]
    public string Title { get; set; } = default!;

    [Required]
    public string Description { get; set; } = default!;

    [Required]
    public Guid JobLocationId { get; set; }

    [Required]
    public double MinSalary { get; set; } = 0;

    [Required]
    public double MaxSalary { get; set; } = 0;
    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public Guid JobCategoryId { get; set; }

    [Required]
    public Guid JobTypeId { get; set; }

    [Required]
    public DateTime Expiration { get; set; }

    public CreateJobPostRequest(string title, string description, Guid jobLocationId, double minSalary, double maxSalary, bool isActive, Guid jobCategoryId, Guid jobTypeId, DateTime expiration)
    {
        Title = title;
        Description = description;
        JobLocationId = jobLocationId;
        MinSalary = minSalary;
        MaxSalary = maxSalary;
        IsActive = isActive;
        JobCategoryId = jobCategoryId;
        JobTypeId = jobTypeId;
        Expiration = expiration;
    }
}