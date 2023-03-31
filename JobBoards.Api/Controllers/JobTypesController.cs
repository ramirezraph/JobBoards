using AutoMapper;
using JobBoards.Data.Contracts.JobType;
using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Repositories.JobTypes;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.Api.Controllers
{
    public class JobTypesController : ApiController
    {
        private readonly IJobTypesRepository _jobTypesRepository;
        private readonly IMapper _mapper;

        public JobTypesController(IJobTypesRepository jobTypesRepository, IMapper mapper)
        {
            _jobTypesRepository = jobTypesRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> ListJobTypes()
        {
            List<JobType> jobTypes = await _jobTypesRepository.GetAllAsync();
            return Ok(jobTypes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobTypeById(Guid id)
        {
            var jobType = await _jobTypesRepository.GetByIdAsync(id);
            if(jobType is null)
            {
                return NotFound();
            }

            return Ok(jobType);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJobType(CreateJobTypeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newJobType = JobType.CreateNew(request.Name, request.Description);
            await _jobTypesRepository.AddAsync(newJobType);

            return CreatedAtAction(nameof(GetJobTypeById), new { id = newJobType.Id}, newJobType);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobType(Guid id)
        {
            var jobType = await _jobTypesRepository.GetByIdAsync(id);
            if(jobType is null)
            {
                return NotFound();
            }

            await _jobTypesRepository.RemoveAsync(jobType);
            return NoContent();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateJobType(UpdateJobTypeRequest request, Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jobType = await _jobTypesRepository.GetByIdAsync(id);
            if(jobType is null)
            {
                return NotFound();
            }
            var updatedType = _mapper.Map<JobType>(request);

            await _jobTypesRepository.UpdateAsync(jobType.Id, updatedType);

            return Ok();
        }
    }
}
