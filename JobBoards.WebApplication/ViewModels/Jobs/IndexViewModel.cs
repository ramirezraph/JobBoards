using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Jobs;

public class IndexViewModel
{
    public List<JobPost> JobPosts { get; set; } = new();
    public List<JobCategory> JobCategories { get; set; } = new();
    public List<JobLocation> JobLocations { get; set; } = new();
    public List<JobType> JobTypes { get; set; } = new();
}