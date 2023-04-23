using System.ComponentModel.DataAnnotations;

namespace JobBoards.Data.Contracts.JobApplication;

public class NewStatusRequest
{
    [Required]
    public string NewStatus { get; set; } = default!;
}