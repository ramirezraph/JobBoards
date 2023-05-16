namespace JobBoards.Data.ApiServices;

public enum ApiEndpoint
{
    // Job Posts
    [Endpoint("jobs")]
    GetAllJobPosts,

    [Endpoint("jobs/{id}")]
    GetJobPostById,

    // Job Categories
    [Endpoint("jobcategories")]
    GetAllJobCategories,

    [Endpoint("jobcategories/{id}")]
    GetJobCategoryById,

    [Endpoint("jobcategories")]
    CreateJobCategory,

    [Endpoint("jobcategories/{id}")]
    DeleteJobCategory,

    [Endpoint("jobcategories/{id}")]
    UpdateJobCategory
}

