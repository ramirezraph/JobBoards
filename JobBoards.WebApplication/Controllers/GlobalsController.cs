using JobBoards.WebApplication.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.WebApplication.Controllers;

public class GlobalsController : BaseController
{
    public IActionResult DisplayToast(string title, string message, string type)
    {
        var viewModel = new ToastNotification
        {
            Title = title,
            Message = message,
            Type = type
        };

        return PartialView("~/Views/Shared/Toast/_Toast.cshtml", viewModel);
    }
}