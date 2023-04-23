using AutoMapper;
using JobBoards.Data.Contracts.JobApplication;
using JobBoards.Data.Contracts.JobSeekers;
using JobBoards.Data.Persistence.Repositories.JobApplications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.Api.Controllers;

[Authorize(Roles = "Admin,Employer")]
public class JobApplicationsController : ApiController
{
    private readonly IJobApplicationsRepository _jobApplicationsRepository;
    private readonly IMapper _mapper;

    public JobApplicationsController(IJobApplicationsRepository jobApplicationsRepository, IMapper mapper)
    {
        _jobApplicationsRepository = jobApplicationsRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllJobApplications()
    {
        var jobApplications = await _jobApplicationsRepository.GetAllAsync();
        var jobApplicationsDto = _mapper.Map<List<JobApplicationResponse>>(jobApplications);

        return Ok(jobApplicationsDto);
    }

    [HttpPost("{jobApplicationId}/status")]
    public async Task<IActionResult> UpdateStatus(Guid jobApplicationId, NewStatusRequest request)
    {
        var jobApplication = await _jobApplicationsRepository.GetByIdAsync(jobApplicationId);

        if (jobApplication is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _jobApplicationsRepository.UpdateStatusAsync(jobApplicationId, request.NewStatus);

        return NoContent();
    }
}