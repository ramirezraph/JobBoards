namespace JobBoards.Data.ApiServices;

public enum ApiEndpoint
{
    // Job Category
    [Endpoint("jobs")]
    GetAllJobPosts,
    [Endpoint("jobs/{id}")]
    GetJobPostById,

    // Job Categories
    [Endpoint("jobcategories")]
    GetAllJobCategories,
}

