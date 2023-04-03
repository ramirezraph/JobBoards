using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Repositories.Common;

namespace JobBoards.Data.Persistence.Repositories.JobApplications;

public interface IJobApplicationsRepository : IRepository<JobApplication>
{
    Task<List<JobApplication>> GetAllByPostIdAsync(Guid postId);
    Task<List<JobApplication>> GetAllByJobSeekerIdAsync(Guid jobSeekerId);
    Task UpdateStatusAsync(Guid id, string newStatus);
    Task<JobApplication?> GetJobSeekerApplicationToJobPostAsync(Guid jobSeekerId, Guid postId);
    Task WithdrawAsync(Guid jobApplicationId);
}