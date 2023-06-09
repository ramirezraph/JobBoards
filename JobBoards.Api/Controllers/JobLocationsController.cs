using AutoMapper;
using JobBoards.Data.Contracts.JobLocation;
using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Repositories.JobLocations;
using JobBoards.Data.Persistence.Repositories.JobPosts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.Api.Controllers;

[Authorize(Roles = "Admin,Employer")]
public class JobLocationsController : ApiController
{
    private readonly IJobLocationsRepository _jobLocationsRepository;
    private readonly IJobPostsRepository _jobPostsRepository;
    private readonly IMapper _mapper;

    public JobLocationsController(IJobLocationsRepository jobLocationsRepository, IMapper mapper, IJobPostsRepository jobPostsRepository)
    {
        _jobLocationsRepository = jobLocationsRepository;
        _mapper = mapper;
        _jobPostsRepository = jobPostsRepository;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ListJobLocations()
    {
        List<JobLocation> jobLocations = await _jobLocationsRepository.GetAllAsync();
        return Ok(jobLocations);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobLocationById(Guid id)
    {
        var jobLocation = await _jobLocationsRepository.GetByIdAsync(id);
        if (jobLocation is null)
        {
            return NotFound();
        }

        return Ok(jobLocation);
    }

    [HttpPost]
    public async Task<IActionResult> CreateJobLocation(CreateJobLocationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newJobLocation = JobLocation.CreateNew(request.City, request.Country);
        await _jobLocationsRepository.AddAsync(newJobLocation);

        return CreatedAtAction(nameof(GetJobLocationById), new { id = newJobLocation.Id }, newJobLocation);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobLocation(Guid id)
    {
        var jobLocation = await _jobLocationsRepository.GetByIdAsync(id);
        if (jobLocation is null)
        {
            return NotFound();
        }

        var jobPosts = await _jobPostsRepository.GetAllAsync();
        if (jobPosts.Count(jp => jp.JobLocationId == jobLocation.Id) > 0)
        {
            return Conflict("Unable to delete location. Location is still in used.");
        }

        await _jobLocationsRepository.RemoveAsync(jobLocation);

        return NoContent();
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> UpdateJobLocation(UpdateJobLocationRequest request, Guid id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var jobLocation = await _jobLocationsRepository.GetByIdAsync(id);
        if (jobLocation is null)
        {
            return NotFound();
        }

        var updatedJobLocation = _mapper.Map<JobLocation>(request);

        await _jobLocationsRepository.UpdateAsync(jobLocation.Id, updatedJobLocation);

        return Ok();
    }
}