using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Jobs;

public class EditViewModel
{
    public List<JobType> JobTypes { get; set; } = new();
    public List<JobCategory> JobCategories { get; set; } = new();
    public List<JobLocation> JobLocations { get; set; } = new();
    public InputModel Form { get; set; } = new();

    public class InputModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please enter a job title.")]
        public string Title { get; set; } = default!;

        [Required(ErrorMessage = "Please enter a job description.")]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "Please select a job location.")]
        [DisplayName("Job Location")]
        public Guid JobLocationId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid salary.")]
        public double MinSalary { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid salary.")]
        public double MaxSalary { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Please select a job category.")]
        [DisplayName("Job Category")]
        public Guid JobCategoryId { get; set; }

        [Required(ErrorMessage = "Please select a job type.")]
        [DisplayName("Job Type")]
        public Guid JobTypeId { get; set; }

        public InputModel(Guid id, string title, string description, Guid jobLocationId, double minSalary, double maxSalary, bool isActive, Guid jobCategoryId, Guid jobTypeId)
        {
            Id = id;
            Title = title;
            Description = description;
            JobLocationId = jobLocationId;
            MinSalary = minSalary;
            MaxSalary = maxSalary;
            IsActive = isActive;
            JobCategoryId = jobCategoryId;
            JobTypeId = jobTypeId;
        }

        public InputModel(JobPost jobPost)
        {
            Id = jobPost.Id;
            Title = jobPost.Title;
            Description = jobPost.Description;
            JobLocationId = jobPost.JobLocationId;
            MinSalary = jobPost.MinSalary;
            MaxSalary = jobPost.MaxSalary;
            IsActive = jobPost.IsActive;
            JobCategoryId = jobPost.JobCategoryId;
            JobTypeId = jobPost.JobTypeId;
        }

        public InputModel()
        {
        }
    }
}