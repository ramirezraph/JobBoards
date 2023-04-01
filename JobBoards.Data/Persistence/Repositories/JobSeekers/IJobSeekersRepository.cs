using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Repositories.Common;

namespace JobBoards.Data.Persistence.Repositories.JobSeekers;

public interface IJobSeekersRepository : IRepository<JobSeeker>
{
    Task UpdateResume(Guid id, string resumeDownloadLink);
    Task<JobSeeker?> RegisterUserAsJobSeeker(string userId);
    Task<JobSeeker?> GetJobSeekerProfileByUserId(string userId);
}