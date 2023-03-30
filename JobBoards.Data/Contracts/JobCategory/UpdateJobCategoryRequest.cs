
using System.ComponentModel.DataAnnotations;

namespace JobBoards.Data.Contracts.JobCategory;

public class UpdateJobCategoryRequest
{
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }

    public UpdateJobCategoryRequest(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}