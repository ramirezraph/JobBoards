using JobBoards.Data.Identity;
using JobBoards.Data.Persistence.Repositories.JobCategories;
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
    private readonly IJobCategoriesRepository _jobCategoriesRepository;
    private readonly IJobLocationsRepository _jobLocationsRepository;
    private readonly IJobTypesRepository _jobTypesRepository;  

    public JobsController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJobCategoriesRepository jobCategoriesRepository,
        IJobLocationsRepository jobLocationsRepository,
        IJobTypesRepository jobTypesRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jobCategoriesRepository = jobCategoriesRepository;
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
    public IActionResult Create()
    {
        return View();
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