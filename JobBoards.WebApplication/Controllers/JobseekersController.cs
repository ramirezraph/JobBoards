using Microsoft.AspNetCore.Mvc;

namespace JobBoards.WebApplication.Controllers;

public class JobseekersController : Controller
{
    [HttpGet]
    public IActionResult JobApplications()
    {
        return View();
    }
}