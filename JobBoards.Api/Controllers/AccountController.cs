using System.Security.Claims;
using AutoMapper;
using JobBoards.Data.Authentication;
using JobBoards.Data.Contracts.Account;
using JobBoards.Data.Identity;
using JobBoards.Data.Persistence.Repositories.JobSeekers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.Api.Controllers;

public class AccountController : ApiController
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IJobSeekersRepository _jobSeekersRepository;
    public AccountController(IJwtTokenGenerator jwtTokenGenerator, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IMapper mapper, IJobSeekersRepository jobSeekersRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _signInManager = signInManager;
        _userManager = userManager;
        _mapper = mapper;
        _jobSeekersRepository = jobSeekersRepository;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized("Invalid credentials");
        }

        var loginResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!loginResult.Succeeded)
        {
            return Unauthorized("Invalid credentials");
        }

        var dto = _mapper.Map<AuthenticationResponse>(user);
        dto.Token = await _jwtTokenGenerator.GenerateToken(user);

        return Ok(dto);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newUser = new ApplicationUser
        {
            UserName = request.Email,
            FullName = request.FullName,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);
        if (!result.Succeeded)
        {
            return IdentityValidationProblem(result);
        }

        await _userManager.AddToRoleAsync(newUser, "User");
        await _jobSeekersRepository.RegisterUserAsJobSeeker(newUser.Id);

        var dto = _mapper.Map<AuthenticationResponse>(newUser);
        dto.Token = await _jwtTokenGenerator.GenerateToken(newUser);

        return Ok(dto);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId ?? "");
        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(_mapper.Map<GetProfileResponse>(user));
    }

    [HttpPost("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        user.FullName = request.FullName;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return IdentityValidationProblem(result);
        }

        return Ok(_mapper.Map<GetProfileResponse>(user));
    }

    [HttpPost("changepassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId ?? "");
        if (user is null)
        {
            return Unauthorized();
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            return IdentityValidationProblem(changePasswordResult);
        }

        var dto = _mapper.Map<AuthenticationResponse>(user);
        dto.Token = await _jwtTokenGenerator.GenerateToken(user);

        return Ok(dto);
    }

    private IActionResult IdentityValidationProblem(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Code, error.Description);
        }
        return ValidationProblem(ModelState);
    }
}