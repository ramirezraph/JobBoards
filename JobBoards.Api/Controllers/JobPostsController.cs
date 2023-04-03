using AutoMapper;
using JobBoards.Data.Contracts.JobLocation;
using JobBoards.Data.Contracts.JobPost;
using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Repositories.JobLocations;
using JobBoards.Data.Persistence.Repositories.JobPosts;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.Api.Controllers;

    public class JobPostsController : ApiController
    {
        private readonly IJobPostsRepository _jobPostsRepository;
        private readonly IMapper _mapper;

        public JobPostsController(IJobPostsRepository jobPostsRepository, IMapper mapper)
        {
            _jobPostsRepository = jobPostsRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> ListJobPosts()
        {
            List<JobPost> jobPosts = await _jobPostsRepository.GetAllAsync();
            return Ok(jobPosts);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newjobPost = JobPost.CreateNew(request.Title, request.Description, request.JobLocationId, request.MinSalary, request.MaxSalary, request.IsActive, request.JobCategoryId, request.JobTypeId, request.Expiration, request.CreatedById);
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var jobPost = await _jobPostsRepository.GetByIdAsync(id);
        if (jobPost is null)
        {
            return NotFound();
        }

        var updatedJobPost = _mapper.Map<JobPost>(request);

        await _jobPostsRepository.UpdateAsync(jobPost.Id, updatedJobPost);

        return Ok();
    }
}

