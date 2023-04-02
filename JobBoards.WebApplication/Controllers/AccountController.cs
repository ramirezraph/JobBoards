using JobBoards.Data.Identity;
using JobBoards.Data.Persistence.Repositories.JobSeekers;
using JobBoards.WebApplication.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.WebApplication.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJobSeekersRepository _jobSeekersRepository;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJobSeekersRepository jobSeekersRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jobSeekersRepository = jobSeekersRepository;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(loginViewModel);
        }

        var loginResult = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, false, false);

        if (!loginResult.Succeeded)
        {
            ViewBag.LoginFailedMessage = "Login failed. Please check your email and password and try again.";
            return View(loginViewModel);
        }

        return RedirectToAction(actionName: "Index", controllerName: "Home");
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
            ViewBag.RegisterFailedMessage = "Failed to register user. Please try again.";
            return View(registerViewModel);
        }

        // Add Role
        await _userManager.AddToRoleAsync(newUser, "User");

        // Register as Jobseeker
        await _jobSeekersRepository.RegisterUserAsJobSeeker(newUser.Id);

        // Sign in the user
        await _signInManager.SignInAsync(newUser, isPersistent: false);
        return RedirectToAction(actionName: "Index", controllerName: "Home");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction(controllerName: "Account", actionName: "Login");
        }
        var viewModel = new ProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email ?? ""
        };

        return View(viewModel);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Profile(ProfileViewModel viewModel)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction(controllerName: "Account", actionName: "Login");
        }

        // user.Email = viewModel.Email;
        user.FullName = viewModel.FullName;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            viewModel.UpdateResultMessage = "Profile update failed. Please try again.";
            return View(viewModel);
        }

        viewModel.UpdateResultMessage = "Profile updated successfully.";
        return View(viewModel);
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
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
