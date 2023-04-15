using JobBoards.WebApplication.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JobBoards.WebApplication.Controllers;

public class BaseController : Controller
{
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        base.OnActionExecuted(filterContext);

        // Check if there is a toast message in TempData
        if (TempData.ContainsKey("ShowToast") && TempData["ShowToast"] is ToastNotification toast)
        {
            // Assign the toast message to ViewData
            ViewData["ShowToast"] = toast;

            // Remove the toast message from TempData
            TempData.Remove("ShowToast");
        }
    }
}