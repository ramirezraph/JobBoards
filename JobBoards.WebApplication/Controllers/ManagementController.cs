using JobBoards.Data.Entities;
using JobBoards.Data.Entities.Common;
using JobBoards.Data.Persistence.Repositories.JobApplications;
using JobBoards.Data.Persistence.Repositories.JobCategories;
using JobBoards.Data.Persistence.Repositories.JobLocations;
using JobBoards.Data.Persistence.Repositories.JobPosts;
using JobBoards.WebApplication.ViewModels.Jobs;
using JobBoards.WebApplication.ViewModels.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobBoards.WebApplication.Controllers;

[Authorize(Roles = "Admin")]
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
            RecentJobApplications = await _jobApplicationsRepository.GetThreeRecentJobApplicationAsync(),
            RecentJobPosts = await _jobPostsRepository.GetNewListingsAsync()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> JobApplications(
        string? search = null,
        Guid? jobCategoryId = null,
        Guid? jobLocationId = null)
    {
        var jobPosts = await _jobPostsRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            jobPosts = jobPosts.Where(jp => jp.Title.ToLower().Contains(search.ToLower())).ToList();
        }

        if (jobCategoryId != null && jobCategoryId != Guid.Empty)
        {
            jobPosts = jobPosts.Where(jp => jp.JobCategoryId == jobCategoryId).ToList();
        }

        if (jobLocationId != null && jobLocationId != Guid.Empty)
        {
            jobPosts = jobPosts.Where(jp => jp.JobLocationId == jobLocationId).ToList();
        }

        var viewModel = new ManageJobApplicationsViewModel
        {
            JobPosts = jobPosts.OrderByDescending(jp => jp.CreatedAt).ToList(),
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

    public IActionResult ViewJobApplicants(Guid jobPostId, string requestPath)
    {
        return RedirectToAction(
            controllerName: "Jobs",
            actionName: "ManageJobApplications",
            routeValues: new { id = jobPostId, returnUrl = requestPath });
    }

    public async Task<IActionResult> JobCategories()
    {
        var viewModel = new ManageJobCategoriesViewModel
        {
            JobCategories = await _jobCategoriesRepository.GetAllAsync()
        };
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> CreateJobCategory()
    {
        var viewModel = new ManageJobCategoriesViewModel
        {
            JobCategories = await _jobCategoriesRepository.GetAllAsync()
        };
        return View(viewModel);
    }

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

        return RedirectToAction(controllerName: "Management", actionName: "JobCategories", routeValues: new { id = formValues.JobCategoryId });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteJobCategory(Guid id)
    {
        var jobCategory = await _jobCategoriesRepository.GetByIdAsync(id);
        if (jobCategory is null)
        {
            return NotFound();
        }
        await _jobCategoriesRepository.RemoveAsync(jobCategory);
        return RedirectToAction("JobCategories");
    }

    public async Task<IActionResult> JobLocations()
    {
        var viewModel = new ManageJobLocationsViewModel
        {
            JobLocations = await _jobLocationsRepository.GetAllAsync()
        };
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> CreateJobLocation()
    {
        var viewModel = new ManageJobLocationsViewModel
        {
            JobLocations = await _jobLocationsRepository.GetAllAsync()
        };
        return View(viewModel);
    }

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

        return RedirectToAction(controllerName: "Management", actionName: "JobLocations", routeValues: new { id = formValues.JobLocationId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteJobLocation(Guid id)
    {
        var jobLocation = await _jobLocationsRepository.GetByIdAsync(id);
        if (jobLocation is null)
        {
            return NotFound();
        }
        await _jobLocationsRepository.RemoveAsync(jobLocation);
        return RedirectToAction("JobLocations");
    }

}