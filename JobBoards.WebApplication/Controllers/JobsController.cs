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

namespace JobBoards.WebApplication.Controllers;

public class JobsController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJobPostsRepository _jobPostsRepository;
    private readonly IJobCategoriesRepository _jobCategoriesRepository;
    private readonly IJobLocationsRepository _jobLocationsRepository;
    private readonly IJobTypesRepository _jobTypesRepository;
    private readonly IMapper _mapper;

    public JobsController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJobCategoriesRepository jobCategoriesRepository,
        IMapper mapper,
        IJobPostsRepository jobPostsRepository,
        IJobLocationsRepository jobLocationsRepository,
        IJobTypesRepository jobTypesRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jobCategoriesRepository = jobCategoriesRepository;
        _mapper = mapper;
        _jobPostsRepository = jobPostsRepository;
        _jobLocationsRepository = jobLocationsRepository;
        _jobTypesRepository = jobTypesRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var viewModel = new IndexViewModel
        {
            JobCategories = await _jobCategoriesRepository.GetAllAsync(),
            JobLocations = await _jobLocationsRepository.GetAllAsync(),
            JobTypes = await _jobTypesRepository.GetAllAsync()
        };
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Details()
    {
        var viewModel = new DetailsViewModel
        {
            IsSignedIn = User.Identity?.IsAuthenticated ?? false,
            HasWriteAccess = User.IsInRole("Admin") || User.IsInRole("Employer"),
            WithApplication = false
        };
        return View(viewModel);
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Employer")]
    public async Task<IActionResult> Create()
    {
        var viewModel = new CreateViewModel
        {
            JobCategories = await _jobCategoriesRepository.GetAllAsync(),
            // JobTypes = await _jobTypesRepository.GetAllAsync(),
            // JobLocations = await _jobLocations.GetAllAsync(),
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
            // viewModel.JobTypes = await _jobTypesRepository.GetAllAsync();
            // viewModel.JobLocations = await _jobLocationsRepository.GetAllAsync();

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
            true,
            formValues.JobCategoryId,
            formValues.JobTypeId,
            DateTime.UtcNow.AddYears(1),
            signedInUserId
        );

        await _jobPostsRepository.AddAsync(newJobPost);

        return RedirectToAction(controllerName: "Jobs", actionName: "Index");
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Employer")]
    public IActionResult Update()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Employer")]
    public IActionResult ManageJobApplications()
    {
        return View();
    }
}