using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Management;

public class DashboardViewModel
{
    public int NumberOfJobApplicationsToday { get; set; }
    public int TotalNumberOfJobPosts { get; set; }
    public int TotalNumberOfJobApplications { get; set; }
    public List<JobApplication> RecentJobApplications { get; set; } = new();
    public List<JobPost> RecentJobPosts { get; set; } = new();
}