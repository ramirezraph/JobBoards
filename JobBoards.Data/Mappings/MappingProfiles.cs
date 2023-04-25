using AutoMapper;
using JobBoards.Data.Contracts.Account;
using JobBoards.Data.Contracts.JobCategory;
using JobBoards.Data.Contracts.JobLocation;
using JobBoards.Data.Contracts.JobPost;
using JobBoards.Data.Contracts.JobSeekers;
using JobBoards.Data.Contracts.JobType;
using JobBoards.Data.Contracts.Management;
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
        CreateMap<Resume, ResumeResponse>().ReverseMap();

        // Job Post
        CreateMap<JobPost, JobPostResponse>()
            .ForPath(dest => dest.JobLocation.Id, opt => opt.MapFrom(src => src.JobLocationId))
            .ForPath(dest => dest.JobLocation.City, opt => opt.MapFrom(src => src.JobLocation.City))
            .ForPath(dest => dest.JobLocation.County, opt => opt.MapFrom(src => src.JobLocation.Country))
            .ForPath(dest => dest.JobCategory.Id, opt => opt.MapFrom(src => src.JobCategoryId))
            .ForPath(dest => dest.JobCategory.Name, opt => opt.MapFrom(src => src.JobCategory.Name))
            .ForPath(dest => dest.JobCategory.Description, opt => opt.MapFrom(src => src.JobCategory.Description))
            .ForPath(dest => dest.JobType.Id, opt => opt.MapFrom(src => src.JobTypeId))
            .ForPath(dest => dest.JobType.Name, opt => opt.MapFrom(src => src.JobType.Name))
            .ForPath(dest => dest.JobType.Description, opt => opt.MapFrom(src => src.JobType.Description))
            .ForPath(dest => dest.CreatedBy.Id, opt => opt.MapFrom(src => src.CreatedById))
            .ForPath(dest => dest.CreatedBy.Name, opt => opt.MapFrom(src => src.CreatedBy.FullName))
            .ReverseMap();

        CreateMap<JobPost, UpdateJobPostRequest>().ReverseMap();
        CreateMap<JobPost, CreateJobPostRequest>().ReverseMap();

        // Job Application
        CreateMap<JobApplication, JobApplicationResponse>()
            .ForPath(dest => dest.JobPost.Id, opt => opt.MapFrom(src => src.JobPostId))
            .ForPath(dest => dest.JobPost.Location, opt => opt.MapFrom(src => $"{src.JobPost.JobLocation.City}, {src.JobPost.JobLocation.Country}"))
            .ForPath(dest => dest.JobPost.Title, opt => opt.MapFrom(src => src.JobPost.Title))
            .ForPath(dest => dest.JobPost.Category, opt => opt.MapFrom(src => src.JobPost.JobCategory.Name))
            .ForPath(dest => dest.JobPost.Type, opt => opt.MapFrom(src => src.JobPost.JobType.Name))
            .ForPath(dest => dest.JobPost.MinSalary, opt => opt.MapFrom(src => src.JobPost.MinSalary))
            .ForPath(dest => dest.JobPost.MaxSalary, opt => opt.MapFrom(src => src.JobPost.MaxSalary))
            .ForPath(dest => dest.Resume.Id, opt => opt.MapFrom(src => src.JobSeeker.ResumeId))
            .ForPath(dest => dest.Resume.FileName, opt => opt.MapFrom(src => src.JobSeeker.Resume.FileName))
            .ForPath(dest => dest.Resume.Uri, opt => opt.MapFrom(src => src.JobSeeker.Resume.Uri));

        CreateMap<JobApplication, BasicJobApplicationResponse>()
            .ForPath(dest => dest.Resume.Id, opt => opt.MapFrom(src => src.JobSeeker.ResumeId))
            .ForPath(dest => dest.Resume.FileName, opt => opt.MapFrom(src => src.JobSeeker.Resume.FileName))
            .ForPath(dest => dest.Resume.Uri, opt => opt.MapFrom(src => src.JobSeeker.Resume.Uri));

        // Management
        CreateMap<ApplicationUser, UserResponse>().ReverseMap();
    }

}