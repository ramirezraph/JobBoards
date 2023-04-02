using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Jobseekers;

public class JobApplicationsViewModel
{
    public List<JobApplication> JobApplications { get; set; } = new();
}