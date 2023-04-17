namespace JobBoards.WebApplication.ViewModels.Shared;

public class ChangeApplicationStatusModalViewModel
{
    public Guid JobApplicationId { get; set; }
    public string NewStatus { get; set; } = default!;
    public string ApplicantName { get; set; } = default!;
}