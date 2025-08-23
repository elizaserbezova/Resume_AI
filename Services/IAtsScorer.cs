namespace ResumeAi.Api.Services
{
    public interface IAtsScorer
    {
        (int score, Dictionary<string, int> breakdown) Score(string cvText, string jobDescription);
    }
}
