using System.Security.Claims;
using AutoMapper;
using Azure.Storage.Blobs;
using JobBoards.Data.Contracts.JobSeekers;
using JobBoards.Data.Identity;
using JobBoards.Data.Persistence.Repositories.JobApplications;
using JobBoards.Data.Persistence.Repositories.JobSeekers;
using JobBoards.Data.Persistence.Repositories.Resumes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.Api.Controllers;

[Authorize(Roles = "User")]
public class JobSeekersController : ApiController
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IJobSeekersRepository _jobSeekersRepository;
    private readonly IJobApplicationsRepository _jobApplicationsRepository;
    private readonly IResumesRepository _resumesRepository;

    public JobSeekersController(UserManager<ApplicationUser> userManager, IJobSeekersRepository jobSeekersRepository, IMapper mapper, IJobApplicationsRepository jobApplicationsRepository, BlobServiceClient blobServiceClient, IResumesRepository resumesRepository)
    {
        _userManager = userManager;
        _jobSeekersRepository = jobSeekersRepository;
        _mapper = mapper;
        _jobApplicationsRepository = jobApplicationsRepository;
        _blobServiceClient = blobServiceClient;
        _resumesRepository = resumesRepository;
    }

    [HttpGet("resume")]
    public async Task<IActionResult> GetResume()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var resume = await _jobSeekersRepository.GetResumeByUserIdAsync(userId);
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

        if (jobseekerProfile is null)
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

    [HttpDelete("resume")]
    public async Task<IActionResult> RemoveResume()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var jobSeekerProfile = await _jobSeekersRepository.GetJobSeekerProfileByUserId(userId ?? "");

        if (jobSeekerProfile is null)
        {
            return NotFound("JobSeeker profile not found.");
        }

        // Check if Jobseeker has pending job applications
        var jobApplications = await _jobApplicationsRepository.GetAllByJobSeekerIdAsync(jobSeekerProfile.Id);
        if (jobApplications.Any())
        {
            return Conflict("Unable to remove resume. The resume is in used.");
        }

        // Get the resume blob URI from the job seeker profile
        var resumeUri = jobSeekerProfile.Resume.Uri;

        // Get the blob name from the resume blob URI
        var blobName = resumeUri.Segments.Last();

        // Get the blob container client
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient("resumes");

        // Get the blob client for the resume
        var blobClient = blobContainerClient.GetBlobClient(blobName);

        // Delete the blob client
        var response = await blobClient.DeleteIfExistsAsync();
        if (response.Value)
        {
            // Deletion was successful
            var resume = await _resumesRepository.GetByIdAsync(jobSeekerProfile.ResumeId);
            if (resume is not null)
            {
                await _resumesRepository.RemoveAsync(resume);
            }
        }
        else
        {
            return Problem("Resume delete failed. Please try again.", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }

    [HttpGet("jobapplications")]
    public async Task<IActionResult> GetAllJobApplications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var jobSeekerProfile = await _jobSeekersRepository.GetJobSeekerProfileByUserId(userId);

        if (jobSeekerProfile is null)
        {
            return NotFound("No jobseeker profile was found.");
        }

        var jobApplications = await _jobApplicationsRepository.GetAllByJobSeekerIdAsync(jobSeekerProfile.Id);
        var jobApplicationDto = _mapper.Map<List<JobApplicationResponse>>(jobApplications);

        return Ok(jobApplicationDto);
    }
}