using System.ComponentModel.DataAnnotations;

namespace JobBoards.Data.Contracts.JobLocation;

public class CreateJobLocationRequest
{
    [Required]
    public string City { get; set; }

    [Required]
    public string Country { get; set; }

    public CreateJobLocationRequest(string city, string country)
    {
        City = city;
        Country = country;
    }
}