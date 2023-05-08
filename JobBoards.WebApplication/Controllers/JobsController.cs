using System.Security.Claims;
using AutoMapper;
using JobBoards.Data.Entities;
using JobBoards.Data.Identity;
using JobBoards.Data.Persistence.Repositories.JobCategories;
using JobBoards.Data.Persistence.Repositories.JobPosts;
using JobBoards.Data.Persistence.Repositories.JobLocations;
using JobBoards.Data.Persistence.Repositories.JobTypes;
using JobBoards.WebApplication.ViewModels.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JobBoards.Data.Persistence.Repositories.JobApplications;
using JobBoards.Data.Persistence.Repositories.JobSeekers;
using System.Web;
using JobBoards.WebApplication.ViewModels.Shared;
using Newtonsoft.Json;
using static JobBoards.WebApplication.ViewModels.Jobs.IndexViewModel;
using JobBoards.Data.Common.Models;
using JobBoards.Data.ApiServices.JobCategoryAPI;

namespace JobBoards.WebApplication.Controllers;

public class JobsController : BaseController
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJobPostsRepository _jobPostsRepository;
    private readonly IJobCategoriesRepository _jobCategoriesRepository;
    private readonly IJobLocationsRepository _jobLocationsRepository;
    private readonly IJobTypesRepository _jobTypesRepository;
    private readonly IJobApplicationsRepository _jobApplicationsRepository;
    private readonly IJobSeekersRepository _jobSeekersRepository;
    private readonly IJobCategoryAPI _jobCategoryAPI;

    public JobsController(
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJobCategoriesRepository jobCategoriesRepository,
        IJobPostsRepository jobPostsRepository,
        IJobLocationsRepository jobLocationsRepository,
        IJobTypesRepository jobTypesRepository,
        IJobApplicationsRepository jobApplicationsRepository,
        IJobSeekersRepository jobSeekersRepository,
        IJobCategoryAPI jobCategoryAPI)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _jobCategoriesRepository = jobCategoriesRepository;
        _jobPostsRepository = jobPostsRepository;
        _jobLocationsRepository = jobLocationsRepository;
        _jobTypesRepository = jobTypesRepository;
        _jobApplicationsRepository = jobApplicationsRepository;
        _jobSeekersRepository = jobSeekersRepository;
        _jobCategoryAPI = jobCategoryAPI;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        int? pageNumber = null,
        string? search = null,
        Guid? jobCategoryId = null,
        Guid? jobLocationId = null,
        double? minSalary = null,
        double? maxSalary = null,
        string? activeJobTypeIds = null)
    {
        IQueryable<JobPost> jobPosts = _jobPostsRepository.GetAllQueryable()
                                            .OrderByDescending(jp => jp.UpdatedAt)
                                                .ThenByDescending(jp => jp.CreatedAt);
        // Filter all Inactive
        if (!User.IsInRole("Admin") && !User.IsInRole("Employer"))
        {
            jobPosts = jobPosts.Where(jp => jp.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            jobPosts = jobPosts.Where(jp => jp.Title.ToLower().Contains(search.ToLower()) || jp.Description.ToLower().Contains(search.ToLower()));
        }

        if (jobCategoryId != null && jobCategoryId != Guid.Empty)
        {
            jobPosts = jobPosts.Where(jp => jp.JobCategoryId == jobCategoryId);
        }

        if (jobLocationId != null && jobLocationId != Guid.Empty)
        {
            jobPosts = jobPosts.Where(jp => jp.JobLocationId == jobLocationId);
        }

        if (minSalary != null)
        {
            jobPosts = jobPosts.Where(jp => jp.MinSalary >= minSalary || jp.MaxSalary >= minSalary);
        }

        if (maxSalary != null)
        {
            jobPosts = jobPosts.Where(jp => jp.MinSalary <= maxSalary || jp.MaxSalary <= maxSalary);
        }

        var activeJobTypes = !string.IsNullOrEmpty(activeJobTypeIds) ?
            activeJobTypeIds.Split(',').Select(Guid.Parse).ToList() :
            new List<Guid>();

        var jobTypes = await _jobTypesRepository.GetAllAsync();
        var jobTypesViewModels = jobTypes.ConvertAll(jt => new JobTypeCheckboxViewModel
        {
            JobTypeId = jt.Id,
            JobTypeName = jt.Name,
            IsChecked = (activeJobTypes.Any() ? activeJobTypes.Contains(jt.Id) : true)
        });

        if (activeJobTypeIds != null && activeJobTypes.Any())
        {
            jobPosts = jobPosts.Where(jp => activeJobTypes.Contains(jp.JobTypeId));
        }

        var paginatedJobPosts = await PaginatedResult<JobPost>.CreateAsync(jobPosts, pageNumber ?? 1, 4);

        var jobCategories = await _jobCategoryAPI.GetAllAsync();

        var viewModel = new IndexViewModel
        {
            Pagination = paginatedJobPosts,
            JobCategories = jobCategories.ToList(),
            JobLocations = await _jobLocationsRepository.GetAllAsync(),
            JobTypes = jobTypesViewModels,
            HasWriteAccess = User.IsInRole("Admin") || User.IsInRole("Employer"),
            Filters = new IndexViewModel.FilterForm
            {
                Search = search,
                JobCategoryId = jobCategoryId,
                JobLocationId = jobLocationId,
                MinSalary = minSalary,
                MaxSalary = maxSalary,
                ActiveJobTypeIds = activeJobTypes
            }
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult RefineSearchResult(IndexViewModel indexViewModel)
    {
        List<Guid>? activeJobTypeIds = null;

        if (indexViewModel.JobTypes.Any(t => t.IsChecked == false))
        {
            activeJobTypeIds = indexViewModel.JobTypes
                    .Where(jt => jt.IsChecked)
                    .Select(jt => jt.JobTypeId)
                    .ToList();
        }

        return RedirectToAction(
            controllerName: "Jobs",
            actionName: "Index",
            routeValues: new
            {
                search = indexViewModel.Filters.Search,
                jobCategoryId = indexViewModel.Filters.JobCategoryId,
                jobLocationId = indexViewModel.Filters.JobLocationId,
                minSalary = indexViewModel.Filters.MinSalary,
                maxSalary = indexViewModel.Filters.MaxSalary,
                activeJobTypeIds = string.Join(",", activeJobTypeIds ?? new())
            });
    }

    public IActionResult SearchForJobs(IndexViewModel indexViewModel)
    {
        return RedirectToAction(
           controllerName: "Jobs",
           actionName: "Index",
           routeValues: new
           {
               jobCategoryId = indexViewModel.Filters.JobCategoryId,
               jobLocationId = indexViewModel.Filters.JobLocationId,
               minSalary = indexViewModel.Filters.MinSalary,
               maxSalary = indexViewModel.Filters.MaxSalary
           });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, string? returnUrl)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(id);

        if (jobPost is null)
        {
            return NotFound();
        }

        var viewModel = new DetailsViewModel
        {
            JobPost = jobPost,
            IsSignedIn = User.Identity?.IsAuthenticated ?? false,
            HasWriteAccess = User.IsInRole("Admin") || User.IsInRole("Employer"),
            WithApplication = false
        };

        var user = await _userManager.GetUserAsync(User);
        if (user is not null)
        {
            var jobSeekerProfile = await _jobSeekersRepository.GetJobSeekerProfileByUserId(user.Id);
            if (jobSeekerProfile is not null)
            {
                // Check for an existing application.
                var jobApplication = await _jobApplicationsRepository.GetJobSeekerApplicationToJobPostAsync(jobSeekerProfile.Id, jobPost.Id);

                if (jobApplication is not null)
                {
                    viewModel.WithApplication = true;
                    viewModel.JobApplication = jobApplication;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(returnUrl))
        {
            ViewData["ReturnUrl"] = returnUrl;
        }

        return View(viewModel);
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Employer")]
    public async Task<IActionResult> Create()
    {
        var viewModel = new CreateViewModel
        {
            JobCategories = await _jobCategoriesRepository.GetAllAsync(),
            JobTypes = await _jobTypesRepository.GetAllAsync(),
            JobLocations = await _jobLocationsRepository.GetAllAsync(),
        };
        return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Employer")]
    public async Task<IActionResult> Create(CreateViewModel viewModel)
    {
        if (viewModel.Form.MaxSalary < viewModel.Form.MinSalary)
        {
            ModelState.AddModelError("Form.MaxSalary", "Max Salary must be greater than the Min Salary.");
        }

        if (!ModelState.IsValid)
        {
            viewModel.JobCategories = await _jobCategoriesRepository.GetAllAsync();
            viewModel.JobTypes = await _jobTypesRepository.GetAllAsync();
            viewModel.JobLocations = await _jobLocationsRepository.GetAllAsync();

            return View(viewModel);
        }

        var formValues = viewModel.Form;

        var signedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (signedInUserId is null)
        {
            return Unauthorized();
        }

        var newJobPost = JobPost.CreateNew(
            formValues.Title,
            formValues.Description,
            formValues.JobLocationId,
            formValues.MinSalary,
            formValues.MaxSalary,
            true, // TODO: Create a form field for IsActive
            formValues.JobCategoryId,
            formValues.JobTypeId,
            DateTime.UtcNow.AddYears(1), // TODO: Create a form field for Expiration
            signedInUserId
        );

        await _jobPostsRepository.AddAsync(newJobPost);

        TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
        {
            Title = "Success",
            Message = "Job post created successfully.",
            Type = "success"
        });

        return RedirectToAction(controllerName: "Jobs", actionName: "Details", routeValues: new { id = newJobPost.Id });
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Employer")]
    public async Task<IActionResult> Update(Guid id)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(id);
        if (jobPost is null)
        {
            return NotFound();
        }

        var viewModel = new EditViewModel
        {
            Form = new EditViewModel.InputModel(jobPost),
            JobCategories = await _jobCategoriesRepository.GetAllAsync(),
            JobTypes = await _jobTypesRepository.GetAllAsync(),
            JobLocations = await _jobLocationsRepository.GetAllAsync(),
        };

        return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Employer")]
    public async Task<IActionResult> Update(EditViewModel viewModel)
    {
        if (viewModel.Form.MaxSalary < viewModel.Form.MinSalary)
        {
            ModelState.AddModelError("Form.MaxSalary", "Max Salary must be greater than the Min Salary.");
        }

        if (!ModelState.IsValid)
        {
            viewModel.JobCategories = await _jobCategoriesRepository.GetAllAsync();
            viewModel.JobTypes = await _jobTypesRepository.GetAllAsync();
            viewModel.JobLocations = await _jobLocationsRepository.GetAllAsync();

            return View(viewModel);
        }

        var formValues = viewModel.Form;

        var signedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (signedInUserId is null)
        {
            return Unauthorized();
        }

        var updatedJobPost = JobPost.CreateNew(
                   formValues.Title,
                   formValues.Description,
                   formValues.JobLocationId,
                   formValues.MinSalary,
                   formValues.MaxSalary,
                   true, // TODO: Create a form field for IsActive
                   formValues.JobCategoryId,
                   formValues.JobTypeId,
                   DateTime.UtcNow.AddYears(1), // TODO: Create a form field for Expiration
                   signedInUserId
               );

        await _jobPostsRepository.UpdateAsync(viewModel.Form.Id, updatedJobPost);

        TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
        {
            Title = "Success",
            Message = "Job post updated successfully.",
            Type = "success"
        });

        return RedirectToAction(controllerName: "Jobs", actionName: "Details", routeValues: new { id = formValues.Id });
    }

    public async Task<IActionResult> DisplayDeleteConfirmationModal(Guid jobPostId, bool isSoftDelete = true)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(jobPostId);

        if (jobPost is null)
        {
            return NotFound();
        }

        var viewModel = new DeleteJobPostModalViewModel
        {
            JobPostId = jobPost.Id,
            JobPost = jobPost,
            NumberOfPendingJobApplications = jobPost.JobApplications.Count(ja => ja.Status.ToLower() != "withdrawn" && ja.Status.ToLower() != "not suitable"),
            IsSoftDelete = isSoftDelete
        };

        return PartialView("~/Views/Shared/Modals/_DeleteJobPostModal.cshtml", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(Guid id)
    {
        await _jobPostsRepository.SoftDeleteAsync(id);

        TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
        {
            Title = "Success",
            Message = "Job post deleted successfully.",
            Type = "success"
        });

        return RedirectToAction(controllerName: "Jobs", actionName: "Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDelete(Guid id)
    {
        await _jobPostsRepository.DeleteAsync(id);

        TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
        {
            Title = "Success",
            Message = "Job post deleted completely.",
            Type = "success"
        });

        return RedirectToAction(controllerName: "Jobs", actionName: "Index");
    }

    [HttpGet]
    [Route("[controller]/Applications/{id:guid}")]
    [Authorize(Roles = "Admin, Employer")]
    public async Task<IActionResult> ManageJobApplications(
        Guid id,
        string? search = null,
        string? status = null,
        string? returnUrl = null)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(id);

        if (jobPost is null)
        {
            return NotFound();
        }

        var jobApplications = await _jobApplicationsRepository.GetAllByPostIdAsync(id);
        jobApplications = jobApplications.OrderByDescending(ja => ja.UpdatedAt)
                                    .ThenByDescending(ja => ja.CreatedAt).ToList();

        if (!string.IsNullOrEmpty(search))
        {
            jobApplications = jobApplications.Where(ja => ja.JobSeeker.User.FullName.ToLower().Contains(search.ToLower())).ToList();
        }

        if (!string.IsNullOrEmpty(status))
        {
            jobApplications = jobApplications.Where(a => a.Status.ToLower().Replace(" ", "") == status.ToLower().Replace(" ", "")).ToList();
        }

        var viewModel = new JobApplicationsViewModel
        {
            JobPost = jobPost,
            JobApplications = jobApplications,
            Filters = new()
            {
                PostId = jobPost.Id,
                Search = search,
                Status = status
            }
        };

        if (!string.IsNullOrWhiteSpace(returnUrl))
        {
            ViewData["ReturnUrl"] = returnUrl;
        }

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult FilterJobApplications(JobApplicationsViewModel viewModel)
    {
        return RedirectToAction(
            controllerName: "Jobs",
            actionName: "ManageJobApplications",
            routeValues: new
            {
                id = viewModel.Filters.PostId,
                search = viewModel.Filters.Search,
                status = viewModel.Filters.Status
            });
    }

    [Authorize(Roles = "Admin, Employer")]
    public async Task<IActionResult> DisplayChangeApplicationStatusModal(Guid jobApplicationId, string status)
    {
        var jobApplication = await _jobApplicationsRepository.GetByIdAsync(jobApplicationId);

        if (jobApplication is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(status))
        {
            return PartialView("~/Views/Shared/_EmptyPartialView.cshtml");
        }

        var viewModel = new ChangeApplicationStatusModalViewModel
        {
            JobApplicationId = jobApplication.Id,
            ApplicantName = jobApplication.JobSeeker.User.FullName,
            NewStatus = status
        };

        return PartialView("~/Views/Shared/Modals/_ChangeApplicationStatusModal.cshtml", viewModel);
    }

    [Authorize(Roles = "Admin, Employer")]
    [HttpGet]
    public async Task<IActionResult> UpdateJobApplicationStatus(Guid jobApplicationId, string newStatus)
    {
        var jobApplication = await _jobApplicationsRepository.GetByIdAsync(jobApplicationId);

        if (jobApplication is null)
        {
            return NotFound();
        }

        await _jobApplicationsRepository.UpdateStatusAsync(jobApplicationId, newStatus);

        TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
        {
            Title = "Success",
            Message = $"{jobApplication.JobSeeker.User.FullName}'s application has been set to {newStatus}.",
            Type = "success"
        });

        return RedirectToAction(controllerName: "Jobs", actionName: "ManageJobApplications", routeValues: new { id = jobApplication.JobPostId });
    }

    [Authorize(Roles = "Admin, Employer")]
    public async Task<IActionResult> ToggleJobPostIsActive(Guid jobPostId)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(jobPostId);
        if (jobPost is null)
        {
            return NotFound();
        }

        jobPost.IsActive = !jobPost.IsActive;

        await _jobPostsRepository.UpdateAsync(jobPostId, jobPost);

        TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
        {
            Title = "Success",
            Message = $"Job post is now {(jobPost.IsActive ? "active" : "inactive")}.",
            Type = "success"
        });

        return RedirectToAction(controllerName: "Jobs", actionName: "Details", routeValues: new { id = jobPostId });
    }

    public IActionResult BackToList(string? returnUrl = null)
    {
        Console.WriteLine(returnUrl);

        if (!string.IsNullOrEmpty(returnUrl))
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                var decodedUrl = HttpUtility.UrlDecode(returnUrl);
                return LocalRedirect(decodedUrl);
            }
        }

        return RedirectToAction(controllerName: "Jobs", actionName: "Index");
    }
}
