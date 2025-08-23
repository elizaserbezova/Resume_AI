namespace ResumeAi.Api.Models.Dtos
{
    public class UploadRequest
    {
        public string? UploaderName { get; set; }
        public string? Notes { get; set; }
        public IFormFile? File { get; set; }
    }
}
