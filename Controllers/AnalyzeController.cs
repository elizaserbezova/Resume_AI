using Microsoft.AspNetCore.Mvc;
using ResumeAi.Api.Models.Dtos;
using ResumeAi.Api.Services;

namespace ResumeAi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyzeController : ControllerBase
    {
        private readonly IAtsScorer _scorer;
        public AnalyzeController(IAtsScorer scorer) => _scorer = scorer;

        [HttpPost]
        public ActionResult<AnalyzeResponse> Analyze([FromBody] AnalyzeRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.CvText) || string.IsNullOrWhiteSpace(req.JobDescription))
                return BadRequest("cvText and jobDescription are required.");

            var (score, breakdown) = _scorer.Score(req.CvText, req.JobDescription);

            var resp = new AnalyzeResponse
            {
                AtsScore = score,
                AtsBreakdown = breakdown,
                Strengths = new() { "Има ключови термини", "Има основни секции" },
                Weaknesses = new() { "Липсват измерими резултати", "Обобщението е кратко" },
                Recommendations = new() { "Добави числа/постижения", "Подреди умения по релевантност" },
                SuggestedSummary = "Софтуерен/ИТ профил с интерес към уеб разработки; търси роля с растеж и принос."
            };
            return Ok(resp);
        }
    }
}
