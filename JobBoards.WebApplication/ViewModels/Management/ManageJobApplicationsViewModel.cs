
using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Management;

public class ManageJobApplicationsViewModel
{
    public List<JobCategory> JobCategories { get; set; } = new();
    public List<JobLocation> JobLocations { get; set; } = new();
    public List<JobPost> JobPosts { get; set; } = new();
    public FilterForm Filters { get; set; } = new();

    public class FilterForm
    {
        public string? JobTitle { get; set; } = default!;
        public Guid? JobCategoryId { get; set; }
        public Guid? JobLocationId { get; set; }
    }
}