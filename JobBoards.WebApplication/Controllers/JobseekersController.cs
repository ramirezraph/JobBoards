using Azure.Storage.Blobs;
using JobBoards.Data.Persistence.Repositories.JobSeekers;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.WebApplication.Controllers;

public class JobseekersController : Controller
{
    private readonly IJobSeekersRepository _jobSeekersRepository;
    private readonly BlobServiceClient _blobServiceClient;
    public JobseekersController(IJobSeekersRepository jobSeekersRepository, BlobServiceClient blobServiceClient)
    {
        _jobSeekersRepository = jobSeekersRepository;
        _blobServiceClient = blobServiceClient;
    }

    [HttpGet]
    public IActionResult JobApplications()
    {
        return View();
    }

    [HttpGet]
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
            FileDownloadName = fileName
        };
    }
}