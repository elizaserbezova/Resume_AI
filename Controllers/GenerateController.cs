using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ResumeAi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class GenerateController : ControllerBase
    {
        [HttpPost]
        public ActionResult<GenerateResponse> Post([FromBody] GenerateRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.CvText) || string.IsNullOrWhiteSpace(req.JobDescription))
                return BadRequest("cvText and jobDescription are required.");

            var generated = BuildGeneratedCv(req.CvText, req.JobDescription);
            return Ok(new GenerateResponse { GeneratedText = generated });
        }

        private static string BuildGeneratedCv(string cvText, string jobDescription)
        {
            var keywords = ExtractKeywords(jobDescription).Take(12).ToList();

            var role = GuessRole(jobDescription);
            var summary =
$@"Results-driven junior {role} with hands-on practice in {string.Join(", ", keywords.Take(5))}.
Focused on clean code, learning fast, and delivering small features end-to-end.";

            string Section(string name) => ExtractSection(cvText, name);

            var skillsBlock = keywords.Count > 0 ? string.Join(", ", keywords) : "C#, .NET, REST, Git";
            var expBlock = Section("experience");
            var projectsBlock = Section("projects");
            var educationBlock = Section("education");

            var result =
$@"SUMMARY
{summary}

SKILLS
{skillsBlock}

EXPERIENCE
{(string.IsNullOrWhiteSpace(expBlock) ? "- Add your recent experience here." : expBlock)}

PROJECTS
{(string.IsNullOrWhiteSpace(projectsBlock) ? "- Add 1–2 relevant projects." : projectsBlock)}

EDUCATION
{(string.IsNullOrWhiteSpace(educationBlock) ? "- Education details." : educationBlock)}
";
            return result.Trim();
        }

        private static IEnumerable<string> ExtractKeywords(string text)
        {
            var tokens = Regex.Matches(text.ToLowerInvariant(), @"[a-z0-9\+\#\.]+")
                              .Select(m => m.Value);
            var stop = new HashSet<string>(new[]
            {
                "the","and","for","with","to","of","a","an","in","on","we","are","is",
                "you","our","as","will","have","has","be","build","developer","junior",
                "looking","requirements","responsibilities","nice","must","plus"
            });
            return tokens.Where(t => t.Length > 1 && !stop.Contains(t))
                         .Distinct()
                         .OrderBy(t => t); 
        }

        private static string ExtractSection(string cv, string sectionName)
        {
            var idx = cv.IndexOf(sectionName, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return string.Empty;

            var rest = cv.Substring(idx);
            var lines = rest.Split('\n');
            var collected = new List<string>();
            foreach (var l in lines.Skip(1)) 
            {
                if (string.IsNullOrWhiteSpace(l)) break;
                collected.Add(l.Trim('\r'));
            }
            return string.Join(Environment.NewLine, collected);
        }

        private static string GuessRole(string jd)
        {
            var s = jd.ToLowerInvariant();
            if (s.Contains("react")) return ".NET/React Developer";
            if (s.Contains(".net") || s.Contains("c#")) return ".NET Developer";
            if (s.Contains("qa")) return "QA Engineer";
            if (s.Contains("data")) return "Data Analyst";
            return "Software Developer";
        }
    }

    public class GenerateRequest
    {
        public string CvText { get; set; } = "";
        public string JobDescription { get; set; } = "";
    }

    public class GenerateResponse
    {
        public string GeneratedText { get; set; } = "";
    }
}
