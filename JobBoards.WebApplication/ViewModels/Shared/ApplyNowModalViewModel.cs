namespace JobBoards.WebApplication.ViewModels.Shared;

public class ApplyNowModalViewModel
{
    public Guid JobPostId { get; set; }
    public string JobPostTitle { get; set; } = default!;
}