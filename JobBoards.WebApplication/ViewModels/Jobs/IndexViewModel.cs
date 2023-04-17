using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Jobs;

public class IndexViewModel
{
    public List<JobPost> JobPosts { get; set; } = new();
    public List<JobCategory> JobCategories { get; set; } = new();
    public List<JobLocation> JobLocations { get; set; } = new();
    public List<JobTypeCheckboxViewModel> JobTypes { get; set; } = new();
    public bool HasWriteAccess { get; set; } = false;
    public FilterForm Filters { get; set; } = new FilterForm();

    public class FilterForm
    {
        public string? Search { get; set; }
        public Guid? JobCategoryId { get; set; }
        public Guid? JobLocationId { get; set; }
        public double? MinSalary { get; set; }
        public double? MaxSalary { get; set; }
        public List<Guid> ActiveJobTypeIds { get; set; } = new();
    }

    public class JobTypeCheckboxViewModel
    {
        public Guid JobTypeId { get; set; }
        public String JobTypeName { get; set; } = default!;
        public bool IsChecked { get; set; }
    }
}

