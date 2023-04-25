using JobBoards.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace JobBoards.WebApplication.ViewModels.Management
{
    public class ManageJobLocationsViewModel
    {
        public List<JobLocation> JobLocations { get; set; } = new();
        public JobLocationForm JobLocationsForm { get; set; } = new();

        public class JobLocationForm
        {
            public Guid JobLocationId { get; set; }

            [Required(ErrorMessage = "Please enter a city.")]
            public string City { get; set; } = default!;

            [Required(ErrorMessage = "Please enter a country.")]
            public string Country { get; set; } = default!;

            public JobLocationForm(Guid jobLocationId, string city, string country)
            {
                JobLocationId = jobLocationId;
                City = city;
                Country = country;
            }

            public JobLocationForm(JobLocation jobLocation)
            {
                JobLocationId = jobLocation.Id;
                City = jobLocation.City;
                Country = jobLocation.Country;
            }

            public JobLocationForm()
            {
            }
        }

    }
}
