using AutoMapper;
using JobBoards.Data.Contracts.JobCategory;
using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Repositories.JobCategories;
using JobBoards.Data.Persistence.Repositories.JobPosts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.Api.Controllers;

[Authorize(Roles = "Admin,Employer")]
public class JobCategoriesController : ApiController
{
    private readonly IJobCategoriesRepository _jobCategoriesRepository;
    private readonly IJobPostsRepository _jobPostsRepository;
    private readonly IMapper _mapper;

    public JobCategoriesController(IJobCategoriesRepository jobCategoriesRepository, IMapper mapper, IJobPostsRepository jobPostsRepository)
    {
        _jobCategoriesRepository = jobCategoriesRepository;
        _mapper = mapper;
        _jobPostsRepository = jobPostsRepository;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ListJobCategories()
    {
        List<JobCategory> jobCategories = await _jobCategoriesRepository.GetAllAsync();
        return Ok(jobCategories);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobCategoryById(Guid id)
    {
        var jobCategory = await _jobCategoriesRepository.GetByIdAsync(id);
        if (jobCategory is null)
        {
            return NotFound();
        }

        return Ok(jobCategory);
    }

    [HttpPost]
    public async Task<IActionResult> CreateJobCategory(CreateJobCategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newJobCategory = JobCategory.CreateNew(request.Name, request.Description);
        await _jobCategoriesRepository.AddAsync(newJobCategory);

        return CreatedAtAction(nameof(GetJobCategoryById), new { id = newJobCategory.Id }, newJobCategory);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobCategory(Guid id)
    {
        var jobCategory = await _jobCategoriesRepository.GetByIdAsync(id);
        if (jobCategory is null)
        {
            return NotFound();
        }

        var jobPosts = await _jobPostsRepository.GetAllAsync();
        if (jobPosts.Count(jp => jp.JobCategoryId == jobCategory.Id) > 0)
        {
            return Conflict("Unable to delete category. Category is still in used.");
        }

        await _jobCategoriesRepository.RemoveAsync(jobCategory);

        return NoContent();
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> UpdateJobCategory(UpdateJobCategoryRequest request, Guid id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var jobCategory = await _jobCategoriesRepository.GetByIdAsync(id);
        if (jobCategory is null)
        {
            return NotFound();
        }

        var updatedCategory = _mapper.Map<JobCategory>(request);

        await _jobCategoriesRepository.UpdateAsync(jobCategory.Id, updatedCategory);

        return Ok();
    }
}