using AutoMapper;
using JobBoards.Data.Contracts.JobCategory;
using JobBoards.Data.Contracts.JobLocation;
using JobBoards.Data.Contracts.JobType;
using JobBoards.Data.Entities;

namespace JobBoards.Data.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // Job Category
        CreateMap<JobCategory, CreateJobCategoryRequest>().ReverseMap();
        CreateMap<JobCategory, UpdateJobCategoryRequest>().ReverseMap();

        // Job Location
        CreateMap<JobLocation, CreateJobLocationRequest>().ReverseMap();
        CreateMap<JobLocation, UpdateJobLocationRequest>().ReverseMap();

        CreateMap<JobType, UpdateJobTypeRequest>().ReverseMap();
        CreateMap<JobType, CreateJobTypeRequest>().ReverseMap();
    }
}