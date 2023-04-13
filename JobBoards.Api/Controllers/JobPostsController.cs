using AutoMapper;
using JobBoards.Data.Common.Models;
using JobBoards.Data.Contracts.JobPost;
using JobBoards.Data.Entities;
using JobBoards.Data.Identity;
using JobBoards.Data.Persistence.Repositories.JobCategories;
using JobBoards.Data.Persistence.Repositories.JobLocations;
using JobBoards.Data.Persistence.Repositories.JobPosts;
using JobBoards.Data.Persistence.Repositories.JobTypes;
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

    [HttpGet]
    public async Task<IActionResult> ListJobPosts(int? pageNumber, int? itemsPerPage)
    {
        IQueryable<JobPost> jobPosts = _jobPostsRepository.GetAllQueryable()
                                            .OrderByDescending(jp => jp.CreatedAt);

        var paginatedResult = await PaginatedResult<JobPost>.CreateAsync(jobPosts, pageNumber ?? 1, itemsPerPage ?? 3);

        return Ok(paginatedResult);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobPostById(Guid id)
    {
        var jobPost = await _jobPostsRepository.GetByIdAsync(id);
        if (jobPost is null)
        {
            return NotFound();
        }

        return Ok(jobPost);
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

