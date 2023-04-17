using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Shared;

public class DeleteJobPostModalViewModel
{
    public Guid JobPostId { get; set; }
    public JobPost JobPost { get; set; } = null!;
    public int NumberOfPendingJobApplications { get; set; }
}