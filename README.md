# ResumeAI — API, WebRazor UI, and Tests

A small suite consisting of:
- **ResumeAi.Api** — ASP.NET Core Web API for resume analysis and text generation.
- **WebRazor** — ASP.NET Core Razor Pages/MVC front‑end that talks to the API.
- **Tests** — xUnit test projects covering **unit** and **integration** scenarios.

> This `README.md` is intended to live in the **API** project folder (`ResumeAi.Api/`), but it documents the whole solution so new contributors can get productive quickly.

---

## Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download) (7.0 or 6.0 may also work if the solution targets it)
- Visual Studio 2022 **or** VS Code + C# Dev Kit
- (Optional) Git

Check your SDK:
```bash
dotnet --info
```

---

## Solution Layout (typical)

```
/src
  /ResumeAi.Api                 # ASP.NET Core Web API (this folder contains this README)
  /WebRazor                     # Razor Pages or MVC front-end
/tests
  /ResumeAi.Tests.Unit          # Unit tests (xUnit)
  /ResumeAi.Tests.Integration   # Integration tests (xUnit + WebApplicationFactory)
```

> Your exact folder/project names might differ. Adjust commands accordingly.

---

## Quick Start

### Run the API

```bash
cd src/ResumeAi.Api
dotnet restore
dotnet run
# API should listen on http://localhost:5xxx (see console output)
```

Hot reload during development:
```bash
dotnet watch
```

### Run the WebRazor UI

```bash
cd src/WebRazor
dotnet restore
dotnet run
# then open the shown URL
```

If the UI needs the API URL, set an app setting or environment variable (common patterns):
- `ApiBaseUrl` in `appsettings.Development.json`, or
- `ASPNETCORE_URLS` / `API__BASEURL` environment variables.

---

## API Endpoints (current)

These represent the minimal surface used by tests. More endpoints may exist in the codebase.

### Health
```
GET /health  →  200 OK
```

### Upload a TXT resume
```
POST /api/upload
Content-Type: multipart/form-data
Form field: file (text/plain)
Response: { "text": "<full text>" }
```

**cURL**
```bash
curl -F "file=@cv.txt;type=text/plain" http://localhost:5000/api/upload
```

### Analyze a resume vs. job description
```
POST /api/analyze
Body (JSON):
{
  "cvText": "Skills: React, .NET, REST. Experience: Web API.",
  "jobDescription": "Looking for React + .NET developer to build REST APIs."
}
Response (JSON):
{
  "atsScore": 0-100,
  "atsBreakdown": { "keywords": 42, ... },
  "strengths": [ ... ],
  "weaknesses": [ ... ],
  "recommendations": [ ... ],
  "suggestedSummary": "..." 
}
```

**cURL**
```bash
curl -X POST http://localhost:5000/api/analyze   -H "Content-Type: application/json"   -d '{"cvText":"Skills: React, .NET, REST. Experience: Web API.","jobDescription":"Looking for React + .NET developer to build REST APIs."}'
```

### Generate resume text with sections
```
POST /api/generate
Body (JSON):
{
  "cvText": "Skills: React, .NET. Experience: Built REST APIs. Education: BSc.",
  "jobDescription": "Junior .NET/React developer for REST APIs."
}
Response (JSON):
{
  "generatedText": "...
SUMMARY...
EXPERIENCE...
"
}
```

---

## Testing

Both unit and integration tests use **xUnit**.

### Run all tests (solution root)
```bash
dotnet test
```

### Run only Integration Tests
```bash
# project-specific run (adjust path if your tests folder differs)
dotnet test ./tests/ResumeAi.Tests.Integration/ResumeAi.Tests.Integration.csproj

# filter by class
dotnet test --filter "FullyQualifiedName~ResumeAi.Tests.Integration.ApiIntegrationTests"

# filter by single test method
dotnet test --filter "FullyQualifiedName~Analyze_Returns_Score_And_Breakdown"
```

### Run only Unit Tests
```bash
dotnet test ./tests/ResumeAi.Tests.Unit/ResumeAi.Tests.Unit.csproj
# or filter by namespace/class similar to above
```

### Run tests in Visual Studio
1. **Build** the solution.
2. Open **Test > Test Explorer**.
3. Use **Run All**, or right‑click a project/class/test → **Run** / **Debug**.

### What the Integration Tests cover
- `GET /health` returns **200 OK**.
- `POST /api/upload` accepts a `text/plain` file and echoes parsed text.
- `POST /api/analyze` returns an **ATS score**, a **breakdown** (e.g., `keywords`), plus lists for **strengths**, **weaknesses**, **recommendations**, and a **suggestedSummary**.
- `POST /api/generate` returns `generatedText` containing section headers such as **SUMMARY** and **EXPERIENCE**.

The tests are implemented with `WebApplicationFactory<Program>`, so the API is hosted **in‑process**—you do **not** have to start the API manually to run them.

> **Note:** Ensure the API project exposes a partial `Program` class so `WebApplicationFactory<Program>` can locate it:
>
> ```csharp
> // in Program.cs (API)
> public partial class Program { }
> ```

---

## Configuration & Environments

- Local development commonly uses `appsettings.Development.json`.
- Integration tests can run against the default in‑memory configuration. If you need a special profile, set:
  - `ASPNETCORE_ENVIRONMENT=Testing` before `dotnet test`, or
  - Provide a custom `WebApplicationFactory` override.

Example (PowerShell):
```powershell
$env:ASPNETCORE_ENVIRONMENT = "Testing"
dotnet test
```

---

## Troubleshooting

- **Compiler errors like `Task`, `Dictionary<>`, `List<>`, or `MultipartFormDataContent` not found**
  - Add the following usings to the test file:
    ```csharp
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    ```
  - Or enable implicit usings in the `.csproj`:
    ```xml
    <ImplicitUsings>enable</ImplicitUsings>
    ```

- **Tests not discovered**
  - Ensure packages:
    ```xml
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="*" />
    <PackageReference Include="xunit" Version="*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="*" PrivateAssets="all" />
    ```

- **Integration tests fail to build with WebApplicationFactory**
  - Verify your API project is referenced by the test project:
    ```xml
    <ProjectReference Include="..\src\ResumeAi.Api\ResumeAi.Api.csproj" />
    ```

---

## Useful Commands

```bash
# Restore all
dotnet restore

# Build all
dotnet build -c Debug
dotnet build -c Release

# Run API
dotnet run --project ./src/ResumeAi.Api

# Watch API
dotnet watch --project ./src/ResumeAi.Api

# Run all tests
dotnet test -v minimal
```

---

## Contributing

1. Create a feature branch from `main`.
2. Keep PRs small and focused.
3. Add/Update unit tests. Consider adding integration tests for new endpoints.
4. Ensure `dotnet test` passes locally before opening a PR.

---

## License

MIT (or your preferred license). Update this section to match your project.

---

### Appendix: Sample DTOs Used by Tests

```csharp
// upload
private record UploadDto(string text);

// analyze
private record AnalyzeDto(
    int atsScore,
    Dictionary<string, int> atsBreakdown,
    List<string> strengths,
    List<string> weaknesses,
    List<string> recommendations,
    string suggestedSummary);

// generate
private record GenerateDto(string generatedText);
```
