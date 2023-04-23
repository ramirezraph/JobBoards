using System.ComponentModel.DataAnnotations;

namespace JobBoards.Data.Contracts.JobSeekers;

public class AddResumeRequest
{
    [Required]
    public string Uri { get; set; } = default!;
    [Required]
    public string FileName { get; set; } = default!;
}