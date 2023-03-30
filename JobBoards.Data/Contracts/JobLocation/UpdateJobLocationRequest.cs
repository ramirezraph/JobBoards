using System.ComponentModel.DataAnnotations;

namespace JobBoards.Data.Contracts.JobLocation;

public class UpdateJobLocationRequest
{
    [Required]
    public string City { get; set; }

    [Required]
    public string Country { get; set; }

    public UpdateJobLocationRequest(string city, string country)
    {
        City = city;
        Country = country;
    }
}