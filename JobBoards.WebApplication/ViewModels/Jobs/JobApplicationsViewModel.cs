using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Jobs;

public class JobApplicationsViewModel
{
    public JobPost JobPost { get; set; } = default!;
    public List<JobApplication> JobApplications { get; set; } = new();

    public FilterForm Filters { get; set; } = new();

    public class FilterForm
    {
        public Guid? PostId { get; set; }
        public string? Search { get; set; }
        public string? Status { get; set; }
    }
}