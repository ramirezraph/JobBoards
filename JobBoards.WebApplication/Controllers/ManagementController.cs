using JobBoards.Data.Common.Models;
using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Repositories.JobApplications;
using JobBoards.Data.Persistence.Repositories.JobCategories;
using JobBoards.Data.Persistence.Repositories.JobLocations;
using JobBoards.Data.Persistence.Repositories.JobPosts;
using JobBoards.WebApplication.ViewModels.Management;
using JobBoards.WebApplication.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JobBoards.WebApplication.Controllers;

[Authorize(Roles = "Admin, Employer")]
public class ManagementController : BaseController
{
    private readonly IJobPostsRepository _jobPostsRepository;
    private readonly IJobApplicationsRepository _jobApplicationsRepository;
    private readonly IJobCategoriesRepository _jobCategoriesRepository;
    private readonly IJobLocationsRepository _jobLocationsRepository;

    public ManagementController(IJobPostsRepository jobPostsRepository, IJobApplicationsRepository jobApplicationsRepository, IJobCategoriesRepository jobCategoriesRepository, IJobLocationsRepository jobLocationsRepository)
    {
        _jobPostsRepository = jobPostsRepository;
        _jobApplicationsRepository = jobApplicationsRepository;
        _jobCategoriesRepository = jobCategoriesRepository;
        _jobLocationsRepository = jobLocationsRepository;
    }

    public async Task<IActionResult> Dashboard()
    {
        var viewModel = new DashboardViewModel
        {
            NumberOfJobApplicationsToday = await _jobApplicationsRepository.GetNumberOfJobApplicationsTodayAsync(),
            TotalNumberOfJobPosts = await _jobPostsRepository.GetCountAsync(),
            TotalNumberOfJobApplications = await _jobApplicationsRepository.GetCountAsync(),
            TotalNumberOfDeletedJobPosts = await _jobPostsRepository.GetCountOfDeletedPostsAsync(),
            RecentJobApplications = await _jobApplicationsRepository.GetThreeRecentJobApplicationAsync(),
            RecentJobPosts = await _jobPostsRepository.GetNewListingsAsync()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> JobApplications(
        int? pageNumber = null,
        string? search = null,
        Guid? jobCategoryId = null,
        Guid? jobLocationId = null)
    {
        IQueryable<JobPost> jobPosts = _jobPostsRepository.GetAllQueryable()
                                            .OrderByDescending(jp => jp.UpdatedAt)
                                                .ThenByDescending(jp => jp.CreatedAt);

        if (!string.IsNullOrWhiteSpace(search))
        {
            jobPosts = jobPosts.Where(jp => jp.Title.ToLower().Contains(search.ToLower()));
        }

        if (jobCategoryId != null && jobCategoryId != Guid.Empty)
        {
            jobPosts = jobPosts.Where(jp => jp.JobCategoryId == jobCategoryId);
        }

        if (jobLocationId != null && jobLocationId != Guid.Empty)
        {
            jobPosts = jobPosts.Where(jp => jp.JobLocationId == jobLocationId);
        }

        var paginatedJobPosts = await PaginatedResult<JobPost>.CreateAsync(jobPosts, pageNumber ?? 1, 4);

        var viewModel = new ManageJobApplicationsViewModel
        {
            Pagination = paginatedJobPosts,
            JobCategories = await _jobCategoriesRepository.GetAllAsync(),
            JobLocations = await _jobLocationsRepository.GetAllAsync(),
            Filters = new ManageJobApplicationsViewModel.FilterForm
            {
                JobTitle = search,
                JobCategoryId = jobCategoryId,
                JobLocationId = jobLocationId,
            }
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult RefineSearchResult(ManageJobApplicationsViewModel viewModel)
    {
        return RedirectToAction(
            controllerName: "Management",
            actionName: "JobApplications",
            routeValues: new
            {
                search = viewModel.Filters.JobTitle,
                jobCategoryId = viewModel.Filters.JobCategoryId,
                jobLocationId = viewModel.Filters.JobLocationId
            });
    }

    public async Task<IActionResult> DeletedJobPosts()
    {
        var deletedJobPosts = await _jobPostsRepository.GetAllDeletedAsync();

        return View(deletedJobPosts);
    }

    public async Task<IActionResult> DisplayRestoreModal(Guid jobPostId)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(jobPostId);
        if (jobPost is null)
        {
            return NotFound();
        }

        var viewModel = new RestoreJobPostModalViewModel
        {
            JobPostId = jobPost.Id,
            JobPostTitle = jobPost.Title
        };

        return PartialView("~/Views/Shared/Modals/_RestoreJobPostModal.cshtml", viewModel);
    }

    public async Task<IActionResult> RestoreJobPost(Guid jobPostId)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(jobPostId);
        if (jobPost is null)
        {
            return NotFound();
        }

        jobPost.DeletedAt = null;

        await _jobPostsRepository.UpdateAsync(jobPost.Id, jobPost);

        TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
        {
            Title = "Success",
            Message = $"{jobPost.Title} has been restored.",
            Type = "success"
        });

        return RedirectToAction(controllerName: "Jobs", actionName: "Details", routeValues: new { id = jobPost.Id });
    }

    public IActionResult ViewJobApplicants(Guid jobPostId, string requestPath)
    {
        return RedirectToAction(
            controllerName: "Jobs",
            actionName: "ManageJobApplications",
            routeValues: new { id = jobPostId, returnUrl = requestPath });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> JobCategories()
    {
        var viewModel = new ManageJobCategoriesViewModel
        {
            JobCategories = await _jobCategoriesRepository.GetAllAsync()
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> CreateJobCategory()
    {
        var viewModel = new ManageJobCategoriesViewModel
        {
            JobCategories = await _jobCategoriesRepository.GetAllAsync()
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateJobCategory(ManageJobCategoriesViewModel.JobCategoryForm form)
    {
        if (ModelState.IsValid)
        {
            var jobCategory = new JobCategory
            {
                Name = form.Name,
                Description = form.Description
            };
            await _jobCategoriesRepository.AddAsync(jobCategory);

            TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
            {
                Title = "Success",
                Message = "Job Category created successfully.",
                Type = "success"
            });

            return RedirectToAction(nameof(JobCategories));
        }
        else
        {
            var viewModel = new ManageJobCategoriesViewModel
            {
                JobCategoriesForm = form
            };
            return View(viewModel);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> EditJobCategory(Guid id)
    {
        var jobCategory = await _jobCategoriesRepository.GetByIdAsync(id);
        if (jobCategory is null)
        {
            return NotFound();
        }

        var viewModel = new ManageJobCategoriesViewModel
        {
            JobCategoriesForm = new ManageJobCategoriesViewModel.JobCategoryForm(jobCategory),
            JobCategories = await _jobCategoriesRepository.GetAllAsync()
        };

        return View(viewModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> EditJobCategory(ManageJobCategoriesViewModel viewModel)

    {
        if (!ModelState.IsValid)
        {
            viewModel.JobCategories = await _jobCategoriesRepository.GetAllAsync();
            return View(viewModel);
        }

        var formValues = viewModel.JobCategoriesForm;

        var updatedJobCategory = JobCategory.CreateNew(
                   formValues.Name,
                   formValues.Description
               );

        await _jobCategoriesRepository.UpdateAsync(viewModel.JobCategoriesForm.JobCategoryId, updatedJobCategory);

        TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
        {
            Title = "Success",
            Message = "Job Category updated successfully.",
            Type = "success"
        });

        return RedirectToAction(controllerName: "Management", actionName: "JobCategories");
    }

    public async Task<IActionResult> DisplayDeleteJobCategoryConfirmationModal(Guid jobCategoryId)
    {
        var jobCategory = await _jobCategoriesRepository.GetByIdAsync(jobCategoryId);

        if (jobCategory is null)
        {
            return NotFound();
        }

        var jobPosts = await _jobPostsRepository.GetAllAsync();

        var viewModel = new DeleteJobCategoryModalViewModel
        {
            JobCategory = jobCategory,
            NumberOfJobPostsWithCategory = jobPosts.Count(jp => jp.JobCategoryId == jobCategory.Id)
        };

        return PartialView("~/Views/Shared/Modals/_DeleteJobCategoryModal.cshtml", viewModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteJobCategory(Guid id)
    {
        var jobCategory = await _jobCategoriesRepository.GetByIdAsync(id);
        if (jobCategory is null)
        {
            return NotFound();
        }

        var jobPosts = await _jobPostsRepository.GetAllAsync();
        if (jobPosts.Count(jp => jp.JobCategoryId == jobCategory.Id) > 0)
        {
            TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
            {
                Title = "Failed",
                Message = "Unable to remove category. Category is still in used.",
                Type = "danger"
            });
            return RedirectToAction("JobCategories");
        }

        await _jobCategoriesRepository.RemoveAsync(jobCategory);

        TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
        {
            Title = "Success",
            Message = "Job Category deleted successfully.",
            Type = "success"
        });

        return RedirectToAction("JobCategories");
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> JobLocations()
    {
        var viewModel = new ManageJobLocationsViewModel
        {
            JobLocations = await _jobLocationsRepository.GetAllAsync()
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> CreateJobLocation()
    {
        var viewModel = new ManageJobLocationsViewModel
        {
            JobLocations = await _jobLocationsRepository.GetAllAsync()
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateJobLocation(ManageJobLocationsViewModel.JobLocationForm form)
    {
        if (ModelState.IsValid)
        {
            var jobLocation = new JobLocation
            {
                City = form.City,
                Country = form.Country
            };
            await _jobLocationsRepository.AddAsync(jobLocation);

            TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
            {
                Title = "Success",
                Message = "Job Location created successfully.",
                Type = "success"
            });

            return RedirectToAction(nameof(JobLocations));
        }
        else
        {
            var viewModel = new ManageJobLocationsViewModel
            {
                JobLocationsForm = form
            };

            return View(viewModel);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> EditJobLocation(Guid id)
    {
        var jobLocation = await _jobLocationsRepository.GetByIdAsync(id);
        if (jobLocation is null)
        {
            return NotFound();
        }

        var viewModel = new ManageJobLocationsViewModel
        {
            JobLocationsForm = new ManageJobLocationsViewModel.JobLocationForm(jobLocation),
            JobLocations = await _jobLocationsRepository.GetAllAsync()
        };

        return View(viewModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> EditJobLocation(ManageJobLocationsViewModel viewModel)

    {
        if (!ModelState.IsValid)
        {
            viewModel.JobLocations = await _jobLocationsRepository.GetAllAsync();
            return View(viewModel);
        }

        var formValues = viewModel.JobLocationsForm;

        var updatedJobLocation = JobLocation.CreateNew(
                   formValues.City,
                   formValues.Country
               );

        await _jobLocationsRepository.UpdateAsync(viewModel.JobLocationsForm.JobLocationId, updatedJobLocation);

        TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
        {
            Title = "Success",
            Message = "Job Location updated successfully.",
            Type = "success"
        });

        return RedirectToAction(controllerName: "Management", actionName: "JobLocations");
    }

    public async Task<IActionResult> DisplayDeleteJobLocationConfirmationModal(Guid jobLocationId)
    {
        var jobLocation = await _jobLocationsRepository.GetByIdAsync(jobLocationId);

        if (jobLocation is null)
        {
            return NotFound();
        }

        var jobPosts = await _jobPostsRepository.GetAllAsync();

        var viewModel = new DeleteJobLocationModalViewModel
        {
            JobLocation = jobLocation,
            NumberOfJobPostsWithLocation = jobPosts.Count(jp => jp.JobLocationId == jobLocation.Id)
        };

        return PartialView("~/Views/Shared/Modals/_DeleteJobLocationModal.cshtml", viewModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteJobLocation(Guid id)
    {
        var jobLocation = await _jobLocationsRepository.GetByIdAsync(id);
        if (jobLocation is null)
        {
            return NotFound();
        }

        var jobPosts = await _jobPostsRepository.GetAllAsync();
        if (jobPosts.Count(jp => jp.JobLocationId == jobLocation.Id) > 0)
        {
            TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
            {
                Title = "Failed",
                Message = "Unable to remove location. Location is still in used.",
                Type = "danger"
            });
            return RedirectToAction("JobLocations");
        }

        await _jobLocationsRepository.RemoveAsync(jobLocation);

        TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
        {
            Title = "Success",
            Message = "Job Location deleted successfully.",
            Type = "success"
        });

        return RedirectToAction("JobLocations");
    }

}