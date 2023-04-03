using JobBoards.Data.Authentication;
using JobBoards.Data.Contracts.Account;
using JobBoards.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.Api.Controllers;

public class AccountController : ApiController
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    public AccountController(IJwtTokenGenerator jwtTokenGenerator, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var loginResult = await _signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, false, false);
        if (!loginResult.Succeeded)
        {
            return Unauthorized("Invalid credentials");
        }

        var user = await _userManager.FindByEmailAsync(loginRequest.Email);

        if (user is null)
        {
            return NotFound("User not found.");
        }

        return Ok(_jwtTokenGenerator.GenerateToken(user));
    }

}