using Azure.Storage.Blobs;
using JobBoards.Data.ApiServices;
using JobBoards.Data.Authentication;
using JobBoards.Data.Identity;
using JobBoards.Data.Persistence.Repositories.JobSeekers;
using JobBoards.Data.Persistence.Repositories.Resumes;
using JobBoards.WebApplication.ViewModels.Account;
using JobBoards.WebApplication.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JobBoards.WebApplication.Controllers;

public class AccountController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IHttpClientService _httpClientService;
    private readonly IJobSeekersRepository _jobSeekersRepository;
    private readonly IResumesRepository _resumesRepository;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJobSeekersRepository jobSeekersRepository, IResumesRepository resumesRepository, BlobServiceClient blobServiceClient, IHttpClientService httpClientService, IJwtTokenGenerator jwtTokenGenerator, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jobSeekersRepository = jobSeekersRepository;
        _resumesRepository = resumesRepository;
        _blobServiceClient = blobServiceClient;
        _httpClientService = httpClientService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _httpContextAccessor = httpContextAccessor;
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
        var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
        if (user is null)
        {
            ViewBag.LoginFailedMessage = "Login failed. Please check your email and password and try again.";
            return View(loginViewModel);
        }

        await _signInManager.CreateUserPrincipalAsync(user);
        var jwt = await _jwtTokenGenerator.GenerateToken(user);
        _httpContextAccessor?.HttpContext?.Session.SetString("JWT", jwt);

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

        var jwt = await _jwtTokenGenerator.GenerateToken(newUser);
        _httpContextAccessor?.HttpContext?.Session.SetString("JWT", jwt);

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
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? ""
        };

        // To display the resume
        var jobSeekerProfile = await _jobSeekersRepository.GetJobSeekerProfileByUserId(user.Id);
        if (jobSeekerProfile is not null)
        {
            if (jobSeekerProfile.ResumeId != Guid.Empty)
            {
                viewModel.UserResume = jobSeekerProfile.Resume;
            }
        }

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

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        if (viewModel.ResumeFile != null)
        {
            var jobSeekerProfile = await _jobSeekersRepository.GetJobSeekerProfileByUserId(user.Id);
            if (jobSeekerProfile is not null)
            {
                // Upload the resume to Azure Storage.
                BlobContainerClient resumesContainer = _blobServiceClient.GetBlobContainerClient("resumes");
                var blobName = jobSeekerProfile.Id.ToString() + Path.GetExtension(viewModel.ResumeFile.FileName);
                BlobClient blobClient = resumesContainer.GetBlobClient(blobName);
                using var stream = viewModel.ResumeFile.OpenReadStream();

                await blobClient.UploadAsync(stream, true);

                // Save the resume to the database.
                await _jobSeekersRepository.UpdateResumeAsync(jobSeekerProfile.Id, blobClient.Uri, viewModel.ResumeFile.FileName);

                viewModel.UserResume = jobSeekerProfile.Resume;
            }
        }
        else
        {
            // To display the resume
            var jobSeekerProfile = await _jobSeekersRepository.GetJobSeekerProfileByUserId(user.Id);
            if (jobSeekerProfile is not null)
            {
                if (jobSeekerProfile.ResumeId != Guid.Empty)
                {
                    viewModel.UserResume = jobSeekerProfile.Resume;
                }
            }
        }

        user.FullName = viewModel.FullName;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
            {
                Title = "Failed",
                Message = "Profile update failed. Please try again",
                Type = "danger"
            });

            return View(viewModel);
        }

        TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
        {
            Title = "Success",
            Message = "Profile updated successfully.",
            Type = "success"
        });

        return View(viewModel);
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // If the password change operation fails due to an incorrect current password, add a custom error message
            if (changePasswordResult.Errors.Any(e => e.Code == "PasswordMismatch"))
            {
                ModelState.AddModelError(string.Empty, "The current password is incorrect.");
            }

            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);

        var jwt = await _jwtTokenGenerator.GenerateToken(user);
        _httpContextAccessor?.HttpContext?.Session.SetString("JWT", jwt);

        TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
        {
            Title = "Success",
            Message = "Password has been changed successfully.",
            Type = "success"
        });

        return RedirectToAction(controllerName: "Account", actionName: "Profile");
    }


    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        _httpContextAccessor?.HttpContext?.Session.Clear();

        return RedirectToAction(actionName: "Index", controllerName: "Home");
    }
}
