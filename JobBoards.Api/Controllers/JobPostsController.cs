using AutoMapper;
using JobBoards.Data.Common.Models;
using JobBoards.Data.Contracts.JobPost;
using JobBoards.Data.Entities;
using JobBoards.Data.Identity;
using JobBoards.Data.Persistence.Repositories.JobCategories;
using JobBoards.Data.Persistence.Repositories.JobLocations;
using JobBoards.Data.Persistence.Repositories.JobPosts;
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
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public JobPostsController(IJobPostsRepository jobPostsRepository, IMapper mapper, IJobTypesRepository jobTypesRepository, IJobLocationsRepository jobLocationsRepository, IJobCategoriesRepository jobCategoriesRepository, UserManager<ApplicationUser> userManager)
    {
        _jobPostsRepository = jobPostsRepository;
        _mapper = mapper;
        _jobTypesRepository = jobTypesRepository;
        _jobLocationsRepository = jobLocationsRepository;
        _jobCategoriesRepository = jobCategoriesRepository;
        _userManager = userManager;
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

    [HttpPost]
    public async Task<IActionResult> CreateJobPost(CreateJobPostRequest request)
    {
        var jobLocation = await _jobLocationsRepository.GetByIdAsync(request.JobLocationId);
        if (jobLocation is null)
        {
            ModelState.AddModelError("Job Location", "Job location cannot be found.");
        }

        var jobCategory = await _jobCategoriesRepository.GetByIdAsync(request.JobCategoryId);
        if (jobCategory is null)
        {
            ModelState.AddModelError("Job Category", "Job category cannot be found.");
        }

        var jobType = await _jobTypesRepository.GetByIdAsync(request.JobTypeId);
        if (jobType is null)
        {
            ModelState.AddModelError("Job Type", "Job type cannot be found.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
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
            user.Id);

        await _jobPostsRepository.AddAsync(newjobPost);

        return CreatedAtAction(nameof(GetJobPostById), new { id = newjobPost.Id }, newjobPost);
    }

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

    [HttpPost("{id}")]
    public async Task<IActionResult> UpdateJobPost(UpdateJobPostRequest request, Guid id)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(id);
        if (jobPost is null)
        {
            return NotFound();
        }

        var jobLocation = await _jobLocationsRepository.GetByIdAsync(request.JobLocationId);
        if (jobLocation is null)
        {
            ModelState.AddModelError("Job Location", "Job location cannot be found.");
        }

        var jobCategory = await _jobCategoriesRepository.GetByIdAsync(request.JobCategoryId);
        if (jobCategory is null)
        {
            ModelState.AddModelError("Job Category", "Job category cannot be found.");
        }

        var jobType = await _jobTypesRepository.GetByIdAsync(request.JobTypeId);
        if (jobType is null)
        {
            ModelState.AddModelError("Job Type", "Job type cannot be found.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var updatedJobPost = _mapper.Map<JobPost>(request);

        await _jobPostsRepository.UpdateAsync(jobPost.Id, updatedJobPost);

        return Ok();
    }
}

