using System.ComponentModel.DataAnnotations;


namespace JobBoards.Data.Contracts.JobType
{
    public class UpdateJobTypeRequest
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }

        public UpdateJobTypeRequest(string name, string? description)
        {
            Name = name;
            Description = description;
        }
    }
}
