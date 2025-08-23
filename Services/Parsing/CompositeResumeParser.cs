namespace ResumeAi.Api.Services.Parsing
{
    public class CompositeResumeParser : IResumeParser
    {
        public async Task<string> ExtractTextAsync(Stream file, string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (ext == ".txt")
            {
                using var sr = new StreamReader(file);
                return await sr.ReadToEndAsync();
            }
            
            return "[Засега поддържаме само .txt за MVP]";
        }
    }
}
