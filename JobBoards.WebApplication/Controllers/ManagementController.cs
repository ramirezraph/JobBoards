using JobBoards.Data.Persistence.Repositories.JobApplications;
using JobBoards.Data.Persistence.Repositories.JobPosts;
using JobBoards.WebApplication.ViewModels.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.WebApplication.Controllers;

[Authorize(Roles = "Admin")]
public class ManagementController : Controller
{
    private readonly IJobPostsRepository _jobPostsRepository;
    private readonly IJobApplicationsRepository _jobApplicationsRepository;

    public ManagementController(IJobPostsRepository jobPostsRepository, IJobApplicationsRepository jobApplicationsRepository)
    {
        _jobPostsRepository = jobPostsRepository;
        _jobApplicationsRepository = jobApplicationsRepository;
    }

    public async Task<IActionResult> Dashboard()
    {
        var viewModel = new DashboardViewModel
        {
            NumberOfJobApplicationsToday = await _jobApplicationsRepository.GetNumberOfJobApplicationsTodayAsync(),
            TotalNumberOfJobPosts = await _jobPostsRepository.GetCountAsync(),
            TotalNumberOfJobApplications = await _jobApplicationsRepository.GetCountAsync(),
            RecentJobApplications = await _jobApplicationsRepository.GetThreeRecentJobApplicationAsync(),
            RecentJobPosts = await _jobPostsRepository.GetNewListingsAsync()
        };

        return View(viewModel);
    }

    public IActionResult JobApplications()
    {
        return View();
    }
}