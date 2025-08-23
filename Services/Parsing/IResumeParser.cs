namespace ResumeAi.Api.Services.Parsing
{
    public interface IResumeParser
    {
        Task<string> ExtractTextAsync(Stream file, string fileName);
    }
}
