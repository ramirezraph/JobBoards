using System.Security.Claims;
using AutoMapper;
using JobBoards.Data.Contracts.JobSeekers;
using JobBoards.Data.Identity;
using JobBoards.Data.Persistence.Repositories.JobSeekers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.Api.Controllers;

public class JobSeekersController : ApiController
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJobSeekersRepository _jobSeekersRepository;

    public JobSeekersController(UserManager<ApplicationUser> userManager, IJobSeekersRepository jobSeekersRepository, IMapper mapper)
    {
        _userManager = userManager;
        _jobSeekersRepository = jobSeekersRepository;
        _mapper = mapper;
    }

    [HttpGet("resume")]
    public async Task<IActionResult> GetResume()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId ?? "");

        Console.WriteLine("JobseekerId = " + user.JobSeekerId);

        if (user == null)
        {
            return Unauthorized();
        }

        var resume = await _jobSeekersRepository.GetResumeByUserIdAsync(user.Id);
        if (resume is null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<ResumeResponse>(resume));
    }

    [HttpPost("resume")]
    public async Task<IActionResult> AddResume(AddResumeRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var jobseekerProfile = await _jobSeekersRepository.GetJobSeekerProfileByUserId(userId ?? "");

        if (jobseekerProfile == null)
        {
            return NotFound("JobSeeker profile not found.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Uri? newUri;
        bool isValidUri = Uri.TryCreate(request.Uri, UriKind.Absolute, out newUri)
                 && (newUri.Scheme == Uri.UriSchemeHttp || newUri.Scheme == Uri.UriSchemeHttps);

        if (newUri is null || !isValidUri)
        {
            ModelState.AddModelError(nameof(request.Uri), "Uri is not valid.");
            return ValidationProblem(ModelState);
        }

        await _jobSeekersRepository.UpdateResumeAsync(jobseekerProfile.Id, newUri, request.FileName);

        var resume = await _jobSeekersRepository.GetResumeByUserIdAsync(jobseekerProfile.UserId);

        return Ok(_mapper.Map<ResumeResponse>(resume));
    }
}