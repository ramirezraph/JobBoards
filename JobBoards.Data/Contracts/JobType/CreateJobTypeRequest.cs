using System.ComponentModel.DataAnnotations;


namespace JobBoards.Data.Contracts.JobType
{
    public class CreateJobTypeRequest
    {
        [Required]
        public string Name { get; set;}
        public string? Description { get; set;}

        public CreateJobTypeRequest(string name, string? description)
        {
            Name = name;
            Description = description;
        }
    }
}
