using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Shared;

public class DeleteJobLocationModalViewModel
{
    public JobLocation JobLocation { get; set; } = null!;
    public int NumberOfJobPostsWithLocation { get; set; }
}