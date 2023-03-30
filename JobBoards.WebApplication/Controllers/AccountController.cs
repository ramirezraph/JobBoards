using JobBoards.Data.Identity;
using JobBoards.WebApplication.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.WebApplication.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(registerViewModel);
        }

        var newUser = new ApplicationUser
        {
            UserName = registerViewModel.Email,
            Email = registerViewModel.Email,
            FullName = registerViewModel.FullName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(newUser, registerViewModel.Password);
        if (!result.Succeeded)
        {
            ViewBag["RegisterFailedMessage"] = "Failed to register user. Please try again.";
            return View(registerViewModel);
        }

        // TODO: Add Role

        // Sign in the user
        await _signInManager.SignInAsync(newUser, isPersistent: false);
        return RedirectToAction(actionName: "Index", controllerName: "Home");
    }

    [HttpGet]
    public IActionResult Profile()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(actionName: "Index", controllerName: "Home");
    }
}