using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Jobs;

public class DetailsViewModel
{
    public bool IsSignedIn { get; set; } = false;
    public bool HasWriteAccess { get; set; } = false;
    public bool WithApplication { get; set; } = false;
    public JobApplication JobApplication { get; set; } = default!;
    public JobPost JobPost { get; set; } = default!;
}