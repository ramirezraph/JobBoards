using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using JobBoards.WebApplication.Models;
using JobBoards.WebApplication.ViewModels.Home;
using JobBoards.Data.Persistence.Repositories.JobPosts;

namespace JobBoards.WebApplication.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IJobPostsRepository _jobPostsRepository;

    public HomeController(ILogger<HomeController> logger, IJobPostsRepository jobPostsRepository)
    {
        _logger = logger;
        _jobPostsRepository = jobPostsRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var viewModel = new IndexViewModel
        {
            NewListings = await _jobPostsRepository.GetNewListingsAsync()
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult JobListing()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
