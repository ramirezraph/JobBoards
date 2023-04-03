using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Home;

public class IndexViewModel
{
    public List<JobPost> NewListings { get; set; } = new();
    public string? SearchJob { get; set; }
}