using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Jobs;

public class CreateViewModel
{
    public List<JobType> JobTypes { get; set; } = new();
    public List<JobCategory> JobCategories { get; set; } = new();
    public List<JobLocation> JobLocations { get; set; } = new();
    public InputModel Form { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Please enter a job title.")]
        public string Title { get; set; } = default!;

        [Required(ErrorMessage = "Please enter a job description.")]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "Please select a job location.")]
        [DisplayName("Job Location")]
        public Guid JobLocationId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid salary.")]
        public double MinSalary { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid salary.")]
        [Compare("MinSalary", ErrorMessage = "Maximum salary must be greater than minimum salary.")]
        public double MaxSalary { get; set; }

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Please select a job category.")]
        [DisplayName("Job Category")]
        public Guid JobCategoryId { get; set; }

        [Required(ErrorMessage = "Please select a job type.")]
        [DisplayName("Job Type")]
        public Guid JobTypeId { get; set; }
    }
}