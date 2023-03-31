using Microsoft.AspNetCore.Mvc;

namespace JobBoards.WebApplication.Controllers;

public class JobsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Details()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Manage()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ManageDetails()
    {
        return View();
    }
}