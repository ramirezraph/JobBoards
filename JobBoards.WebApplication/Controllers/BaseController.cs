using JobBoards.WebApplication.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace JobBoards.WebApplication.Controllers;

public class BaseController : Controller
{
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        base.OnActionExecuted(filterContext);

        // Check if there is a toast message in TempData
        if (TempData.ContainsKey("ShowToast"))
        {
            var toastData = JsonConvert.DeserializeObject<ToastNotification>(TempData["ShowToast"]?.ToString());

            if (toastData is not null)
            {
                if (filterContext.Result is RedirectToActionResult || filterContext.Result is RedirectResult)
                {
                    TempData.Keep("ShowToast");
                }
                else
                {
                    // Assign the toast message to ViewData
                    ViewData["ShowToast"] = toastData;
                    // Remove the toast message from TempData
                    TempData.Remove("ShowToast");
                }

            }
        }
    }
}