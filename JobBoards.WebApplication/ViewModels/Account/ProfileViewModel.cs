using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using JobBoards.Data.Entities;

namespace JobBoards.WebApplication.ViewModels.Account;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class FileExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;

    public FileExtensionsAttribute(string[] extensions)
    {
        _extensions = extensions;
    }

    public override bool IsValid(object value)
    {
        if (value is IFormFile file)
        {
            var extension = System.IO.Path.GetExtension(file.FileName);
            return _extensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        return true;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field must be a file of type: {string.Join(", ", _extensions)}";
    }
}
public class ProfileViewModel
{
    public string UserId { get; set; } = default!;

    [Required]
    [DisplayName("Full Name")]
    public string FullName { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [FileExtensions(new[] { ".pdf" })]
    [DisplayName("Resume File")]
    public IFormFile? ResumeFile { get; set; }

    public string? UpdateResultMessage { get; set; }

    public Resume? UserResume { get; set; }
}