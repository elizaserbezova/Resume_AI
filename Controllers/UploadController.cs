using Microsoft.AspNetCore.Mvc;
using ResumeAi.Api.Models.Dtos;
using ResumeAi.Api.Services.Parsing;
using System.Text.RegularExpressions;
using System.Text;

namespace ResumeAi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController(IResumeParser parser) : ControllerBase
{
    private static readonly HashSet<string> AllowedExt = new(new[] { ".txt", ".pdf", ".docx" });
    private const long MaxBytes = 5 * 1024 * 1024;
    private const int MaxTextLength = 100_000; 

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxBytes)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload([FromForm] UploadRequest request)
    {
        if (request?.File is null) return BadRequest("No file.");
        if (request.File.Length == 0) return BadRequest("Empty file.");
        if (request.File.Length > MaxBytes) return BadRequest("Max file size is 5 MB.");

        var ext = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        if (!AllowedExt.Contains(ext))
            return BadRequest("Allowed extensions: .txt, .pdf, .docx");

        try
        {
            await using var stream = request.File.OpenReadStream();
            var text = await parser.ExtractTextAsync(stream, request.File.FileName);

            text = Normalize(text);
            if (text.Length > MaxTextLength) text = text[..MaxTextLength];

            return Ok(new { text });
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to read file: {ex.Message}");
        }
    }

    private static string Normalize(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return string.Empty;

        s = Regex.Replace(s, @"[\u0000-\u0008\u000B\u000C\u000E-\u001F]", " ");
       
        s = s.Replace("\r\n", "\n").Replace('\r', '\n');
       
        s = Regex.Replace(s, @"\n{3,}", "\n\n");

        return s.Trim();
    }
}
