
using System.ComponentModel.DataAnnotations;

namespace JobBoards.Data.Contracts.JobCategory;

public class CreateJobCategoryRequest
{
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }

    public CreateJobCategoryRequest(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}