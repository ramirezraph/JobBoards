namespace JobBoards.WebApplication.ViewModels.Shared;

public class WithdrawApplicationModalViewModel
{
    public Guid ApplicationId { get; set; }
    public Guid JobSeekerId { get; set; }
    public string JobPostTitle { get; set; } = default!;
}