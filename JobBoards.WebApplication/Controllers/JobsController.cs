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

namespace JobBoards.WebApplication.Controllers;

public class JobsController : Controller
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

    public JobsController(
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJobCategoriesRepository jobCategoriesRepository,
        IJobPostsRepository jobPostsRepository,
        IJobLocationsRepository jobLocationsRepository,
        IJobTypesRepository jobTypesRepository,
        IJobApplicationsRepository jobApplicationsRepository,
        IJobSeekersRepository jobSeekersRepository)
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
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? search = null,
        Guid? jobCategoryId = null,
        Guid? jobLocationId = null,
        double? minSalary = null,
        double? maxSalary = null)
    {
        var jobPosts = await _jobPostsRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            jobPosts = jobPosts.Where(jp => jp.Title.ToLower().Contains(search.ToLower()) || jp.Description.ToLower().Contains(search.ToLower())).ToList();
        }

        if (jobCategoryId != null && jobCategoryId != Guid.Empty)
        {
            jobPosts = jobPosts.Where(jp => jp.JobCategoryId == jobCategoryId).ToList();
        }

        if (jobLocationId != null && jobLocationId != Guid.Empty)
        {
            jobPosts = jobPosts.Where(jp => jp.JobLocationId == jobLocationId).ToList();
        }

        if (minSalary != null)
        {
            jobPosts = jobPosts.Where(jp => jp.MinSalary >= minSalary || jp.MaxSalary >= minSalary).ToList();
        }

        if (maxSalary != null)
        {
            jobPosts = jobPosts.Where(jp => jp.MaxSalary <= maxSalary).ToList();
        }

        var viewModel = new IndexViewModel
        {
            JobPosts = jobPosts.OrderByDescending(jp => jp.CreatedAt).ToList(),
            JobCategories = await _jobCategoriesRepository.GetAllAsync(),
            JobLocations = await _jobLocationsRepository.GetAllAsync(),
            JobTypes = await _jobTypesRepository.GetAllAsync(),
            HasWriteAccess = User.IsInRole("Admin") || User.IsInRole("Employer"),
            Filters = new IndexViewModel.FilterForm
            {
                Search = search,
                JobCategoryId = jobCategoryId,
                JobLocationId = jobLocationId,
                MinSalary = minSalary,
                MaxSalary = maxSalary
            }
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult RefineSearchResult(IndexViewModel indexViewModel)
    {
        return RedirectToAction(
            controllerName: "Jobs",
            actionName: "Index",
            routeValues: new
            {
                search = indexViewModel.Filters.Search,
                jobCategoryId = indexViewModel.Filters.JobCategoryId,
                jobLocationId = indexViewModel.Filters.JobLocationId,
                minSalary = indexViewModel.Filters.MinSalary,
                maxSalary = indexViewModel.Filters.MaxSalary
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
    public async Task<IActionResult> Details(Guid id)
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

        return RedirectToAction(controllerName: "Jobs", actionName: "Index");
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

        return RedirectToAction(controllerName: "Jobs", actionName: "Details", routeValues: new { id = formValues.Id });
    }

    [HttpGet]
    [Route("[controller]/Applications/{id:guid}")]
    [Authorize(Roles = "Admin, Employer")]
    public async Task<IActionResult> ManageJobApplications(Guid id, string? search = null, string? status = null)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(id);

        if (jobPost is null)
        {
            return NotFound();
        }

        var jobApplications = await _jobApplicationsRepository.GetAllByPostIdAsync(id);

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

    [HttpGet]
    public async Task<IActionResult> MakeShortlisted(Guid jobApplicationId)
    {
        var jobApplication = await _jobApplicationsRepository.GetByIdAsync(jobApplicationId);

        if (jobApplication is null)
        {
            return NotFound();
        }

        await _jobApplicationsRepository.UpdateStatusAsync(jobApplicationId, "Shortlisted");

        return RedirectToAction(controllerName: "Jobs", actionName: "ManageJobApplications", routeValues: new { id = jobApplication.JobPostId });
    }

    [HttpGet]
    public async Task<IActionResult> MakeInterview(Guid jobApplicationId)
    {
        var jobApplication = await _jobApplicationsRepository.GetByIdAsync(jobApplicationId);

        if (jobApplication is null)
        {
            return NotFound();
        }

        await _jobApplicationsRepository.UpdateStatusAsync(jobApplicationId, "Interview");

        return RedirectToAction(controllerName: "Jobs", actionName: "ManageJobApplications", routeValues: new { id = jobApplication.JobPostId });
    }

    [HttpGet]
    public async Task<IActionResult> MakeNotSuitable(Guid jobApplicationId)
    {
        var jobApplication = await _jobApplicationsRepository.GetByIdAsync(jobApplicationId);

        if (jobApplication is null)
        {
            return NotFound();
        }

        await _jobApplicationsRepository.UpdateStatusAsync(jobApplicationId, "Not Suitable");

        return RedirectToAction(controllerName: "Jobs", actionName: "ManageJobApplications", routeValues: new { id = jobApplication.JobPostId });
    }
}