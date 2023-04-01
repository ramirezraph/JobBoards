using Microsoft.AspNetCore.Mvc;

namespace JobBoards.WebApplication.Controllers;

public class JobsController : Controller
{

    // ANONYMOUS

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

    // EMPLOYER

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Edit()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ManageDetails()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult Update()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ManageJobApplications()
    {
        return View();
    }
}