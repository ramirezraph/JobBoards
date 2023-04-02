using AutoMapper;
using JobBoards.Data.Contracts.Resume;
using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Repositories.Resumes;
using Microsoft.AspNetCore.Mvc;


namespace JobBoards.Api.Controllers
{
    public class ResumesController : ApiController
    {
        private readonly IResumesRepository _resumesRepository;
        private readonly IMapper _mapper;

        public ResumesController(IResumesRepository resumesRepository, IMapper mapper)
        {
            _resumesRepository = resumesRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> ListResumes()
        {
            List<Resume> resumes = await _resumesRepository.GetAllAsync();
            return Ok(resumes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetResumeById(Guid id)
        {
            var resume = await _resumesRepository.GetByIdAsync(id);
            if (resume is null)
            {
                return NotFound();
            }

            return Ok(resume);
        }

        [HttpPost]
        public async Task<IActionResult> CreateResume(CreateResumeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newResume = Resume.CreateNew(request.JobSeekerId, request.Uri, request.FileName);
            await _resumesRepository.AddAsync(newResume);

            return CreatedAtAction(nameof(GetResumeById), new { id = newResume.Id }, newResume);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResume(Guid id)
        {
            var resume = await _resumesRepository.GetByIdAsync(id);
            if (resume is null)
            {
                return NotFound();
            }

            await _resumesRepository.RemoveAsync(resume);
            return NoContent();
        }
    }
}
