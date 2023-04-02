using System.ComponentModel.DataAnnotations;


namespace JobBoards.Data.Contracts.Resume
{
    public class CreateResumeRequest
    {
        [Required]
        public Guid JobSeekerId { get; set; }
        public Uri Uri { get; set; }
        public string FileName { get; set; }

        public CreateResumeRequest(Guid jobSeekerId, Uri uri, string fileName)
        {
            JobSeekerId = jobSeekerId;
            Uri = uri;
            FileName = fileName;
        }
    }
}