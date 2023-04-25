using AutoMapper;
using JobBoards.Data.Contracts.Management;
using JobBoards.Data.Identity;
using JobBoards.Data.Persistence.Faker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.Api.Controllers;

[Authorize(Roles = "Admin")]
public class ManagementController : ApiController
{
    private readonly IFakerService _fakerService;
    private readonly UserManager<ApplicationUser> _userManagaer;
    private readonly IMapper _mapper;

    public ManagementController(IFakerService fakerService, UserManager<ApplicationUser> userManagaer, IMapper mapper)
    {
        _fakerService = fakerService;
        _userManagaer = userManagaer;
        _mapper = mapper;
    }

    [HttpPost("generate-random-users")]
    public async Task<IActionResult> SeedFakeUsers(int size = 1, string role = "User")
    {
        var fakeUsers = await _fakerService.GenerateFakeUsers(size, role);
        var fakeUsersDto = _mapper.Map<List<UserResponse>>(fakeUsers);
        return Ok(fakeUsersDto);
    }

    [HttpGet("get-all-jobseekers")]
    public async Task<IActionResult> GetAllJobseekers()
    {
        var users = await _userManagaer.GetUsersInRoleAsync("User");
        var usersDto = _mapper.Map<List<UserResponse>>(users);
        return Ok(usersDto);
    }

    [HttpGet("get-all-employers")]
    public async Task<IActionResult> GetAllEmployers()
    {
        var users = await _userManagaer.GetUsersInRoleAsync("Employer");
        var usersDto = _mapper.Map<List<UserResponse>>(users);
        return Ok(usersDto);
    }
}