using ResumeAi.Api.Utils;
using System.Text.RegularExpressions;

namespace ResumeAi.Api.Services
{
    public class SimpleAtsScorer : IAtsScorer
    {
        private static readonly HashSet<string> Stop = new(StringComparer.OrdinalIgnoreCase)
        { "the","and","for","with","your","our","this","that","като","и","да","на","с","за","от","по" };

        public (int score, Dictionary<string, int> breakdown) Score(string cvText, string jd)
        {
            var cv = TextNormalization.Normalize(cvText);
            var jdNorm = TextNormalization.Normalize(jd);

            var tokens = Regex.Split(jdNorm, @"\W+")
                .Where(t => t.Length > 2)
                .Where(t => !Stop.Contains(t))
                .Distinct()
                .Take(30)
                .ToList();

            int hits = tokens.Count(k => cv.Contains(k));
            int keywordScore = tokens.Count == 0 ? 0 : (int)Math.Round(100.0 * hits / tokens.Count * 0.7);

            int sectionScore = 0;
            foreach (var s in new[] { "experience", "skills", "education", "опит", "умения", "образование" })
                if (cv.Contains(s)) sectionScore += 8; 

            var total = Math.Min(100, keywordScore + sectionScore);
            var breakdown = new Dictionary<string, int>
            {
                ["keywords"] = keywordScore,
                ["sections"] = sectionScore
            };
            return (total, breakdown);
        }
    }
}
