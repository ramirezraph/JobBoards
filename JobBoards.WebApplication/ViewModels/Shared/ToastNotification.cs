namespace JobBoards.WebApplication.ViewModels.Shared;

[Serializable]
public class ToastNotification
{
    public ToastNotification()
    {
    }

    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;

    // success, danger, warning, info
    public string Type { get; set; } = "default";
}