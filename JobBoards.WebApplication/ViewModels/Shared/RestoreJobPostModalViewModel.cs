namespace JobBoards.WebApplication.ViewModels.Shared;

public class RestoreJobPostModalViewModel
{
    public Guid JobPostId { get; set; }
    public string JobPostTitle { get; set; } = default!;
}