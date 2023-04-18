using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Shared;

public class DeleteJobCategoryModalViewModel
{
    public JobCategory JobCategory { get; set; } = null!;
    public int NumberOfJobPostsWithCategory { get; set; }
}