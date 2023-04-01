using JobBoards.Data.Identity;
using JobBoards.WebApplication.ViewModels.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.WebApplication.Controllers;

public class JobsController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public JobsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var viewModel = new IndexViewModel
        {

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