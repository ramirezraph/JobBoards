using AutoMapper;
using JobBoards.Data.Contracts.Account;
using JobBoards.Data.Contracts.JobCategory;
using JobBoards.Data.Contracts.JobLocation;
using JobBoards.Data.Contracts.JobPost;
using JobBoards.Data.Contracts.JobType;
using JobBoards.Data.Contracts.Resume;
using JobBoards.Data.Entities;
using JobBoards.Data.Identity;

namespace JobBoards.Data.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // Account
        CreateMap<ApplicationUser, AuthenticationResponse>().ReverseMap();
        CreateMap<ApplicationUser, GetProfileResponse>().ReverseMap();

        // Job Category
        CreateMap<JobCategory, CreateJobCategoryRequest>().ReverseMap();
        CreateMap<JobCategory, UpdateJobCategoryRequest>().ReverseMap();

        // Job Location
        CreateMap<JobLocation, CreateJobLocationRequest>().ReverseMap();
        CreateMap<JobLocation, UpdateJobLocationRequest>().ReverseMap();

        // Job Type
        CreateMap<JobType, UpdateJobTypeRequest>().ReverseMap();
        CreateMap<JobType, CreateJobTypeRequest>().ReverseMap();

        // Resume
        CreateMap<Resume, CreateResumeRequest>().ReverseMap();

        // Job Post
        CreateMap<JobPost, UpdateJobPostRequest>().ReverseMap();
        CreateMap<JobPost, CreateJobPostRequest>().ReverseMap();
    }
}