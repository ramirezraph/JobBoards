using JobBoards.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace JobBoards.WebApplication.ViewModels.Management
{
    public class ManageJobCategoriesViewModel
    {
        public List<JobCategory> JobCategories { get; set; } = new();
        public JobCategoryForm JobCategoriesForm { get; set; } = new(); 

        public class JobCategoryForm
        {
            public Guid JobCategoryId { get; set; }
            [Required(ErrorMessage = "Please enter a job category name.")]
            public string Name { get; set; } = default!;
            public string? Description { get; set; } = default!;

            public JobCategoryForm(Guid jobCategoryId, string name, string? description)
            {
                JobCategoryId = jobCategoryId;
                Name = name;
                Description = description;
            }

            public JobCategoryForm(JobCategory jobCategory)
            {
                JobCategoryId = jobCategory.Id;
                Name = jobCategory.Name;
                Description = jobCategory.Description;
            }

            public JobCategoryForm()
            {
            }
        }

    }
}

