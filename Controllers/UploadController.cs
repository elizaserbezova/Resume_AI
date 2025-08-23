using Microsoft.AspNetCore.Mvc;
using ResumeAi.Api.Models.Dtos;
using ResumeAi.Api.Services.Parsing;

namespace ResumeAi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController(IResumeParser parser) : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]                       
    public async Task<IActionResult> Upload([FromForm] UploadRequest request) 
    {
        if (request.File is null || request.File.Length == 0)
            return BadRequest("No file.");

        await using var stream = request.File.OpenReadStream();
        var text = await parser.ExtractTextAsync(stream, request.File.FileName);

        return Ok(new { text });
    }
}