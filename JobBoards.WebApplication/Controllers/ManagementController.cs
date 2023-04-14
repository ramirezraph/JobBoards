using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.WebApplication.Controllers;

[Authorize(Roles = "Admin")]
public class ManagementController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult JobApplications()
    {
        return View();
    }
}