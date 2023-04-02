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
    public async Task<IActionResult> Index()
    {
        var viewModel = new IndexViewModel
        {
            JobPosts = await _jobPostsRepository.GetAllAsync(),
            JobCategories = await _jobCategoriesRepository.GetAllAsync(),
            JobLocations = await _jobLocationsRepository.GetAllAsync(),
            JobTypes = await _jobTypesRepository.GetAllAsync(),
            HasWriteAccess = User.IsInRole("Admin") || User.IsInRole("Employer"),
        };

        return View(viewModel);
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
    public async Task<IActionResult> ManageJobApplications(Guid id)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(id);

        if (jobPost is null)
        {
            return NotFound();
        }

        var viewModel = new JobApplicationsViewModel
        {
            JobPost = jobPost,
            JobApplications = await _jobApplicationsRepository.GetAllByPostIdAsync(id)
        };

        return View(viewModel);
    }
}