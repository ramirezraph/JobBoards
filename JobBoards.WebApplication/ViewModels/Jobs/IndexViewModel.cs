using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Jobs;

public class IndexViewModel
{
    public List<JobPost> JobPosts { get; set; } = new();
    public List<JobCategory> JobCategories { get; set; } = new();
    public List<JobLocation> JobLocations { get; set; } = new();
    public List<JobType> JobTypes { get; set; } = new();
    public bool HasWriteAccess { get; set; } = false;
    public FilterForm Filters { get; set; } = new FilterForm();

    public class FilterForm
    {
        public Guid? JobCategoryId { get; set; }
        public Guid? JobLocationId { get; set; }
        public Guid? JobTypeId { get; set; }
        public double? MinSalary { get; set; }
        public double? MaxSalary { get; set; }
    }
}

