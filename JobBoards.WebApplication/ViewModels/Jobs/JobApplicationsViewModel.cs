using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Jobs;

public class JobApplicationsViewModel
{
    public JobPost JobPost { get; set; } = default!;
    public List<JobApplication> JobApplications { get; set; } = new();
}