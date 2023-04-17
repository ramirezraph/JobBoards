using Azure.Storage.Blobs;
using JobBoards.Data.Entities;
using JobBoards.Data.Identity;
using JobBoards.Data.Persistence.Repositories.JobApplications;
using JobBoards.Data.Persistence.Repositories.JobPosts;
using JobBoards.Data.Persistence.Repositories.JobSeekers;
using JobBoards.Data.Persistence.Repositories.Resumes;
using JobBoards.WebApplication.ViewModels.Jobseekers;
using JobBoards.WebApplication.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JobBoards.WebApplication.Controllers;

public class JobseekersController : BaseController
{
    private readonly IJobSeekersRepository _jobSeekersRepository;
    private readonly IJobPostsRepository _jobPostsRepository;
    private readonly IResumesRepository _resumesRepository;
    private readonly IJobApplicationsRepository _jobApplicationsRepository;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly UserManager<ApplicationUser> _userManager;
    public JobseekersController(IJobSeekersRepository jobSeekersRepository, BlobServiceClient blobServiceClient, UserManager<ApplicationUser> userManager, IJobPostsRepository jobPostsRepository, IJobApplicationsRepository jobApplicationsRepository, IResumesRepository resumesRepository)
    {
        _jobSeekersRepository = jobSeekersRepository;
        _blobServiceClient = blobServiceClient;
        _userManager = userManager;
        _jobPostsRepository = jobPostsRepository;
        _jobApplicationsRepository = jobApplicationsRepository;
        _resumesRepository = resumesRepository;
    }

    [HttpGet]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> JobApplications()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return RedirectToAction(controllerName: "Account", actionName: "Login");
        }

        var jobSeekerProfile = await _jobSeekersRepository.GetJobSeekerProfileByUserId(user.Id);

        if (jobSeekerProfile is null)
        {
            return BadRequest("No jobseeker profile was found.");
        }

        var viewModel = new JobApplicationsViewModel
        {
            JobApplications = await _jobApplicationsRepository.GetAllByJobSeekerIdAsync(jobSeekerProfile.Id)
        };

        return View(viewModel);
    }

    [Authorize(Roles = "User")]
    public async Task<IActionResult> ApplyNow(Guid postId)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(postId);

        if (jobPost is null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return RedirectToAction(controllerName: "Account", actionName: "Login");
        }

        var jobSeekerProfile = await _jobSeekersRepository.GetJobSeekerProfileByUserId(user.Id);

        if (jobSeekerProfile is null)
        {
            return BadRequest("No jobseeker profile was found.");
        }

        if (jobSeekerProfile.ResumeId == null || jobSeekerProfile.ResumeId == Guid.Empty)
        {
            TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
            {
                Title = "Failed",
                Message = "Unable to apply. Please upload a resume.",
                Type = "danger"
            });

            return RedirectToAction(controllerName: "Account", actionName: "Profile");
        }

        // Check if an application already exists.
        var jobApplication = await _jobApplicationsRepository.GetJobSeekerApplicationToJobPostAsync(jobSeekerProfile.Id, jobPost.Id);
        if (jobApplication is not null)
        {
            // Already applied.
            TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
            {
                Title = "Apply Now",
                Message = "You already have an application for this job.",
                Type = "info"
            });
            return RedirectToAction(controllerName: "Jobs", actionName: "Details", routeValues: new { id = postId });
        }

        var newJobApplication = JobApplication.CreateNew(jobPost.Id, jobSeekerProfile.Id, "Submitted");
        await _jobApplicationsRepository.AddAsync(newJobApplication);

        return RedirectToAction(controllerName: "Jobs", actionName: "Details", routeValues: new { id = postId });
    }

    [Authorize(Roles = "User")]
    public async Task<IActionResult> DisplayWithdrawConfirmationModal(Guid jobApplicationId)
    {
        var jobApplication = await _jobApplicationsRepository.GetByIdAsync(jobApplicationId);

        if (jobApplication is null)
        {
            return NotFound();
        }

        var viewModel = new WithdrawApplicationModalViewModel
        {
            ApplicationId = jobApplication.Id,
            JobSeekerId = jobApplication.JobSeekerId,
            JobPostTitle = jobApplication.JobPost.Title
        };

        return PartialView("~/Views/Shared/Modals/_WithdrawApplicationModal.cshtml", viewModel);
    }

    [Authorize(Roles = "User")]
    public async Task<IActionResult> WithdrawApplication(Guid applicationId, Guid jobseekerId)
    {
        var jobApplication = await _jobApplicationsRepository.GetByIdAsync(applicationId);
        if (jobApplication is null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return RedirectToAction(controllerName: "Account", actionName: "Login");
        }

        var jobSeekerProfile = await _jobSeekersRepository.GetJobSeekerProfileByUserId(user.Id);

        if (jobSeekerProfile is null)
        {
            return BadRequest("No jobseeker profile was found.");
        }

        await _jobApplicationsRepository.WithdrawAsync(jobApplication.Id);

        TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
        {
            Title = "Success",
            Message = "Application withdrawn successfully.",
            Type = "success"
        });

        return RedirectToAction(controllerName: "Jobs", actionName: "Details", routeValues: new { id = jobApplication.JobPostId });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> DownloadResume(string userId)
    {
        // Get the job seeker profile by ID
        var jobSeekerProfile = await _jobSeekersRepository.GetJobSeekerProfileByUserId(userId);

        // Check if the job seeker profile exists
        if (jobSeekerProfile == null)
        {
            return NotFound();
        }

        // Get the resume blob URI from the job seeker profile
        var resumeUri = jobSeekerProfile.Resume.Uri;

        // Get the blob name from the resume blob URI
        var blobName = resumeUri.Segments.Last();

        // Get the blob container client
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient("resumes");

        // Get the blob client for the resume
        var blobClient = blobContainerClient.GetBlobClient(blobName);

        // Download the resume as a stream
        var stream = await blobClient.OpenReadAsync();

        // Get the file name from the resume blob URI
        var fileName = Path.GetFileName(resumeUri.LocalPath);

        // Return the resume file as a FileStreamResult
        return new FileStreamResult(stream, "application/octet-stream")
        {
            FileDownloadName = $"{jobSeekerProfile.User.FullName}-resume{Path.GetExtension(fileName)}"
        };
    }

    [HttpGet]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> RemoveResume(string userId)
    {
        // Get the job seeker profile by ID
        var jobSeekerProfile = await _jobSeekersRepository.GetJobSeekerProfileByUserId(userId);

        // Check if the job seeker profile exists
        if (jobSeekerProfile == null)
        {
            return NotFound();
        }

        // Check if Jobseeker has pending job applications
        var jobApplications = await _jobApplicationsRepository.GetAllByJobSeekerIdAsync(jobSeekerProfile.Id);
        if (jobApplications.Any())
        {
            TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
            {
                Title = "Failed",
                Message = "Unable to remove resume. The resume is in used.",
                Type = "danger"
            });

            return RedirectToAction(controllerName: "Account", actionName: "Profile");
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
            TempData["ShowToast"] = JsonConvert.SerializeObject(new ToastNotification
            {
                Title = "Failed",
                Message = "Resume delete failed. Please try again.",
                Type = "danger"
            });
        }

        return RedirectToAction(controllerName: "Account", actionName: "Profile");
    }
}