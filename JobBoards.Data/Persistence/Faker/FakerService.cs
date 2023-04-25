using System.Security.Cryptography;
using System.Linq;
using Bogus;
using JobBoards.Data.Entities;
using JobBoards.Data.Identity;
using JobBoards.Data.Persistence.Repositories.JobApplications;
using JobBoards.Data.Persistence.Repositories.JobPosts;
using JobBoards.Data.Persistence.Repositories.JobSeekers;
using Microsoft.AspNetCore.Identity;

namespace JobBoards.Data.Persistence.Faker;

public class FakerService : IFakerService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJobSeekersRepository _jobSeekersRepository;
    private readonly IJobApplicationsRepository _jobApplicationsRepository;
    private readonly IJobPostsRepository _jobPostsRepository;

    private readonly Faker<ApplicationUser> _applicationUserFaker;
    private readonly Faker<Resume> _resumeFaker;

    public FakerService(UserManager<ApplicationUser> userManager, IJobSeekersRepository jobSeekersRepository, IJobApplicationsRepository jobApplicationsRepository, IJobPostsRepository jobPostsRepository)
    {
        _userManager = userManager;
        _jobSeekersRepository = jobSeekersRepository;
        _jobApplicationsRepository = jobApplicationsRepository;
        _jobPostsRepository = jobPostsRepository;

        _applicationUserFaker = new Faker<ApplicationUser>()
            .RuleFor(au => au.FullName, f => f.Name.FullName())
            .RuleFor(au => au.Email, (f, au) => f.Internet.Email(au.FullName))
            .RuleFor(au => au.UserName, (f, au) => au.Email);

        _resumeFaker = new Faker<Resume>()
            .RuleFor(r => r.FileName, (f, r) => "test-resume-does-not-exist.pdf")
            .RuleFor(r => r.Uri, f => new Uri(f.Image.PicsumUrl()));
    }

    public async Task<List<ApplicationUser>> GenerateFakeUsers(int size, string role = "User")
    {
        var fakeUsers = _applicationUserFaker.GenerateForever().Take(size);
        var jobPostsIds = _jobPostsRepository.GetAllQueryable().Select(jp => jp.Id);
        var jobApplicationStatuses = new[] { "Submitted", "Interview", "Shortlisted", "Not Suitable", "Withdrawn" };

        foreach (var fakeUser in fakeUsers)
        {
            await _userManager.CreateAsync(fakeUser, "Pass123$");
            await _userManager.AddToRoleAsync(fakeUser, role);

            var jobseekerProfile = await _jobSeekersRepository.RegisterUserAsJobSeeker(fakeUser.Id);

            if (jobseekerProfile is null)
            {
                throw new ArgumentNullException("Failed to register fake user as jobseeker.");
            }

            // Generate fake resume
            var fakeResume = _resumeFaker.Generate();

            await _jobSeekersRepository.UpdateResumeAsync(jobseekerProfile.Id, fakeResume.Uri, fakeResume.FileName);

            // Apply to random job posts
            if (jobPostsIds.Any())
            {
                var newJobApplication = new Faker<JobApplication>()
                                .RuleFor(ja => ja.Id, f => Guid.NewGuid())
                                .RuleFor(ja => ja.JobSeekerId, f => jobseekerProfile.Id)
                                .RuleFor(ja => ja.JobPostId, f => f.PickRandom<Guid>(jobPostsIds))
                                .RuleFor(ja => ja.Status, f => f.PickRandom(jobApplicationStatuses))
                                .RuleFor(ja => ja.CreatedAt, f => DateTime.Now)
                                .RuleFor(ja => ja.UpdatedAt, f => DateTime.Now);

                await _jobApplicationsRepository.AddAsync(newJobApplication.Generate());
            }
        }

        return fakeUsers.ToList();
    }
}