namespace ResumeAi.Api.Models.Dtos
{
    public class AnalyzeResponse
    {
        public int AtsScore { get; set; }
        public Dictionary<string, int> AtsBreakdown { get; set; } = new();
        public List<string> Strengths { get; set; } = new();
        public List<string> Weaknesses { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public string SuggestedSummary { get; set; } = "";
    }
}
