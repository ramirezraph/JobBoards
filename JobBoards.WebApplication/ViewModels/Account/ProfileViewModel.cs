using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Account;

public class ProfileViewModel
{
    [Required]
    [DisplayName("Full Name")]
    public string FullName { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    public Guid ResumeId { get; set; }
    public Resume? Resume { get; set; }

    public string? UpdateResultMessage { get; set; }
}