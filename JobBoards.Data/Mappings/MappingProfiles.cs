using AutoMapper;
using JobBoards.Data.Contracts.JobCategory;
using JobBoards.Data.Contracts.JobType;
using JobBoards.Data.Entities;

namespace JobBoards.Data.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<JobCategory, CreateJobCategoryRequest>().ReverseMap();
        CreateMap<JobCategory, UpdateJobCategoryRequest>().ReverseMap();

        CreateMap<JobType, UpdateJobTypeRequest>().ReverseMap();
        CreateMap<JobType, CreateJobTypeRequest>().ReverseMap();
    }
}