using System.Security.Claims;
using AutoMapper;
using JobBoards.Data.Common.Models;
using JobBoards.Data.Contracts.JobPost;
using JobBoards.Data.Contracts.JobSeekers;
using JobBoards.Data.Entities;
using JobBoards.Data.Identity;
using JobBoards.Data.Persistence.Repositories.JobApplications;
using JobBoards.Data.Persistence.Repositories.JobCategories;
using JobBoards.Data.Persistence.Repositories.JobLocations;
using JobBoards.Data.Persistence.Repositories.JobPosts;
using JobBoards.Data.Persistence.Repositories.JobSeekers;
using JobBoards.Data.Persistence.Repositories.JobTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.Api.Controllers;

public class JobPostsController : ApiController
{
    private readonly IJobPostsRepository _jobPostsRepository;
    private readonly IJobTypesRepository _jobTypesRepository;
    private readonly IJobLocationsRepository _jobLocationsRepository;
    private readonly IJobCategoriesRepository _jobCategoriesRepository;
    private readonly IJobSeekersRepository _jobSeekersRepository;
    private readonly IJobApplicationsRepository _jobApplicationsRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public JobPostsController(IJobPostsRepository jobPostsRepository, IMapper mapper, IJobTypesRepository jobTypesRepository, IJobLocationsRepository jobLocationsRepository, IJobCategoriesRepository jobCategoriesRepository, UserManager<ApplicationUser> userManager, IJobSeekersRepository jobSeekersRepository, IJobApplicationsRepository jobApplicationsRepository)
    {
        _jobPostsRepository = jobPostsRepository;
        _mapper = mapper;
        _jobTypesRepository = jobTypesRepository;
        _jobLocationsRepository = jobLocationsRepository;
        _jobCategoriesRepository = jobCategoriesRepository;
        _userManager = userManager;
        _jobSeekersRepository = jobSeekersRepository;
        _jobApplicationsRepository = jobApplicationsRepository;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ListJobPosts(
        int? pageNumber,
        int? itemsPerPage,
        string? search = null,
        Guid? jobCategoryId = null,
        Guid? jobLocationId = null,
        double? minSalary = null,
        double? maxSalary = null,
        string? activeJobTypeIds = null)
    {
        IQueryable<JobPost> jobPosts = _jobPostsRepository.GetAllQueryable()
                                            .OrderByDescending(jp => jp.CreatedAt);

        // Filter all Inactive
        if (!User.IsInRole("Admin") && !User.IsInRole("Employer"))
        {
            jobPosts = jobPosts.Where(jp => jp.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            jobPosts = jobPosts.Where(jp => jp.Title.ToLower().Contains(search.ToLower()) || jp.Description.ToLower().Contains(search.ToLower()));
        }

        if (jobCategoryId != null && jobCategoryId != Guid.Empty)
        {
            jobPosts = jobPosts.Where(jp => jp.JobCategoryId == jobCategoryId);
        }

        if (jobLocationId != null && jobLocationId != Guid.Empty)
        {
            jobPosts = jobPosts.Where(jp => jp.JobLocationId == jobLocationId);
        }

        if (minSalary != null)
        {
            jobPosts = jobPosts.Where(jp => jp.MinSalary >= minSalary || jp.MaxSalary >= minSalary);
        }

        if (maxSalary != null)
        {
            jobPosts = jobPosts.Where(jp => jp.MinSalary <= maxSalary || jp.MaxSalary <= maxSalary);
        }

        List<Guid> activeJobTypes = !string.IsNullOrEmpty(activeJobTypeIds) ?
            activeJobTypeIds.Split(',').Select(Guid.Parse).ToList() :
            new List<Guid>();

        if (activeJobTypeIds != null && activeJobTypes.Any())
        {
            jobPosts = jobPosts.Where(jp => activeJobTypes.Contains(jp.JobTypeId));
        }

        var paginatedJobPosts = await PaginatedResult<JobPost>.CreateAsync(jobPosts, pageNumber ?? 1, itemsPerPage ?? 4);

        var paginatedJobPostsDto = new PaginatedResult<JobPostResponse>(
                    items: _mapper.Map<List<JobPostResponse>>(paginatedJobPosts.Items),
                    paginatedJobPosts.CurrentPage,
                    paginatedJobPosts.ItemsPerPage,
                    paginatedJobPosts.TotalPages,
                    paginatedJobPosts.HasPreviousPage,
                    paginatedJobPosts.HasNextPage);

        return Ok(paginatedJobPostsDto);
    }

    [AllowAnonymous]
    [HttpGet("newlistings")]
    public async Task<IActionResult> NewListings()
    {
        var newListings = await _jobPostsRepository.GetNewListingsAsync();

        var newListingsDto = _mapper.Map<List<JobPostResponse>>(newListings);
        return Ok(newListingsDto);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobPostById(Guid id)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(id);
        if (jobPost is null)
        {
            return NotFound();
        }

        var jobPostsDto = _mapper.Map<JobPostResponse>(jobPost);

        return Ok(jobPostsDto);
    }

    [Authorize(Roles = "Admin,Employer")]
    [HttpPost]
    public async Task<IActionResult> CreateJobPost(CreateJobPostRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var jobLocation = await _jobLocationsRepository.GetByIdAsync(request.JobLocationId);
        if (jobLocation is null)
        {
            ModelState.AddModelError(nameof(request.JobLocationId), "No job location was found with the given Id.");
            return ValidationProblem(ModelState);
        }

        var jobCategory = await _jobCategoriesRepository.GetByIdAsync(request.JobCategoryId);
        if (jobCategory is null)
        {
            ModelState.AddModelError(nameof(request.JobCategoryId), "No job category was found with the given Id.");
            return ValidationProblem(ModelState);
        }

        var jobType = await _jobTypesRepository.GetByIdAsync(request.JobTypeId);
        if (jobType is null)
        {
            ModelState.AddModelError(nameof(request.JobTypeId), "No job type was found with the given Id.");
            return ValidationProblem(ModelState);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var newjobPost = JobPost.CreateNew(
            request.Title,
            request.Description,
            request.JobLocationId,
            request.MinSalary,
            request.MaxSalary,
            request.IsActive,
            request.JobCategoryId,
            request.JobTypeId,
            request.Expiration,
            userId);

        await _jobPostsRepository.AddAsync(newjobPost);

        var jobPostsDto = _mapper.Map<JobPostResponse>(newjobPost);

        return CreatedAtAction(nameof(GetJobPostById), new { id = newjobPost.Id }, jobPostsDto);
    }

    [Authorize(Roles = "Admin,Employer")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobLocation(Guid id)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(id);
        if (jobPost is null)
        {
            return NotFound();
        }

        await _jobPostsRepository.RemoveAsync(jobPost);

        return NoContent();
    }

    [Authorize(Roles = "Admin, Employer")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateJobPost(UpdateJobPostRequest request, Guid id)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(id);
        if (jobPost is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var jobLocation = await _jobLocationsRepository.GetByIdAsync(request.JobLocationId);
        if (jobLocation is null)
        {
            ModelState.AddModelError(nameof(request.JobLocationId), "No job location was found with the given Id.");
            return ValidationProblem(ModelState);
        }

        var jobCategory = await _jobCategoriesRepository.GetByIdAsync(request.JobCategoryId);
        if (jobCategory is null)
        {
            ModelState.AddModelError(nameof(request.JobCategoryId), "No job category was found with the given Id.");
            return ValidationProblem(ModelState);
        }

        var jobType = await _jobTypesRepository.GetByIdAsync(request.JobTypeId);
        if (jobType is null)
        {
            ModelState.AddModelError(nameof(request.JobTypeId), "No job type was found with the given Id.");
            return ValidationProblem(ModelState);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var updatedJobPost = _mapper.Map<JobPost>(request);

        await _jobPostsRepository.UpdateAsync(jobPost.Id, updatedJobPost);

        var jobPostDto = _mapper.Map<JobPostResponse>(await _jobPostsRepository.GetByIdAsync(id));

        return Ok(jobPostDto);
    }

    [Authorize(Roles = "User")]
    [HttpPost("{jobPostId}/apply")]
    public async Task<IActionResult> Apply(Guid jobPostId)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(jobPostId);
        if (jobPost is null)
        {
            return NotFound();
        }

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

        if (jobSeekerProfile.ResumeId == null || jobSeekerProfile.ResumeId == Guid.Empty)
        {
            return Conflict("User unable to apply. No resume was found.");
        }

        // Check if an application already exists.
        var jobApplication = await _jobApplicationsRepository.GetJobSeekerApplicationToJobPostAsync(jobSeekerProfile.Id, jobPost.Id);
        if (jobApplication is not null)
        {
            return Conflict("User unable to apply. Application was already submitted.");
        }

        var newJobApplication = JobApplication.CreateNew(jobPost.Id, jobSeekerProfile.Id, "Submitted");
        await _jobApplicationsRepository.AddAsync(newJobApplication);

        var newJobApplicationDto = _mapper.Map<JobApplicationResponse>(newJobApplication);

        return Ok(newJobApplicationDto);
    }

    [Authorize(Roles = "User")]
    [HttpPost("{jobPostId}/withdraw")]
    public async Task<IActionResult> Withdraw(Guid jobPostId)
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

        var jobApplication = await _jobApplicationsRepository.GetJobSeekerApplicationToJobPostAsync(jobSeekerProfile.Id, jobPostId);
        if (jobApplication is null)
        {
            return NotFound("No job application was found.");
        }

        await _jobApplicationsRepository.WithdrawAsync(jobApplication.Id);

        return NoContent();
    }

    [Authorize(Roles = "User")]
    [HttpGet("{jobPostid}/myapplication")]
    public async Task<IActionResult> GetMyApplication(Guid jobPostId)
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

        var jobApplication = await _jobApplicationsRepository.GetJobSeekerApplicationToJobPostAsync(jobSeekerProfile.Id, jobPostId);
        if (jobApplication is null)
        {
            return NotFound("No job application was found.");
        }

        return Ok(_mapper.Map<JobApplicationResponse>(jobApplication));
    }

    [Authorize(Roles = "Admin, Employer")]
    [HttpPost("{jobPostid}/toggleactive")]
    public async Task<IActionResult> ToggleActive(Guid jobPostId)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(jobPostId);
        if (jobPost is null)
        {
            return NotFound();
        }

        jobPost.IsActive = !jobPost.IsActive;

        await _jobPostsRepository.UpdateAsync(jobPostId, jobPost);

        return NoContent();
    }

    [Authorize(Roles = "Admin, Employer")]
    [HttpGet("{jobPostid}/jobapplications")]
    public async Task<IActionResult> GetJobApplicationsByJobPostId(
        Guid jobPostid,
        string? search = null,
        string? status = null)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(jobPostid);

        if (jobPost is null)
        {
            return NotFound();
        }

        var jobApplications = await _jobApplicationsRepository.GetAllByPostIdAsync(jobPostid);
        jobApplications = jobApplications.OrderByDescending(ja => ja.UpdatedAt)
                                    .ThenByDescending(ja => ja.CreatedAt).ToList();

        if (!string.IsNullOrEmpty(search))
        {
            jobApplications = jobApplications.Where(ja => ja.JobSeeker.User.FullName.ToLower().Contains(search.ToLower())).ToList();
        }

        if (!string.IsNullOrEmpty(status))
        {
            jobApplications = jobApplications.Where(a => a.Status.ToLower().Replace(" ", "") == status.ToLower().Replace(" ", "")).ToList();
        }

        var jobApplicationsDto = _mapper.Map<List<BasicJobApplicationResponse>>(jobApplications);

        return Ok(jobApplicationsDto);
    }
}

