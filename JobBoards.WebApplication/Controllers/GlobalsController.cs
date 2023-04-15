using JobBoards.WebApplication.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.WebApplication.Controllers;

public class GlobalsController : Controller
{
    public IActionResult DisplayToast()
    {
        var viewModel = new ToastViewModel
        {
            Title = "Notification Title",
            Description = "Notification Description"
        };

        return PartialView("~/Views/Shared/Toast/_Toast.cshtml", viewModel);
    }
}