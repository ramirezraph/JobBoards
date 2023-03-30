using System.ComponentModel.DataAnnotations;


namespace JobBoards.Data.Contracts.Resume
{
    public class CreateResumeRequest
    {
        [Required]
        public Guid JobSeekerId { get; set; }
        public string DownloadLink { get; set; }

        public CreateResumeRequest(Guid jobSeekerId, string downloadLink)
        {
            JobSeekerId = jobSeekerId;
            DownloadLink = downloadLink;
        }
    }
}