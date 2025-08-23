
namespace ResumeAi.Api
{
    using Microsoft.OpenApi.Models;
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.WithOrigins("http://localhost:5173") 
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });

            builder.Services.AddScoped<ResumeAi.Api.Services.Parsing.IResumeParser, ResumeAi.Api.Services.Parsing.CompositeResumeParser>();
            builder.Services.AddScoped<ResumeAi.Api.Services.IAtsScorer, ResumeAi.Api.Services.SimpleAtsScorer>();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors();
            app.MapControllers();

            app.MapGet("/health", () => Results.Ok(new { ok = true, time = DateTime.UtcNow }));

            app.Run();
        }
    }
}
