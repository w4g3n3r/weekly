# Solution Overview

- Solution: `Weekly.sln`
- Visual Studio format: 12.00 (VS 17)
- Projects: 1
  - `Weekly` → `Weekly.csproj`

## Project: Weekly.csproj
- Output type: `Exe`
- Target framework: `net8.0`
- Assembly name: `wk`
- Nullable/implicit usings: enabled
- Version metadata:
  - `Weekly.csproj` sets `<Version>0.1.0</Version>`
  - `Directory.build.props` sets `<Version>0.1.1</Version>`, `<FileVersion>0.1.1.0</FileVersion>`, `<InformationalVersion>0.1.1-alpha</InformationalVersion>`
  - Note: Version precedence follows MSBuild import order; verify effective version via build output (`dotnet build /v:n`).

## Package Dependencies
- Flurl 4.0.0
- Meziantou.Framework.Win32.CredentialManager 1.7.3
- Microsoft.Extensions.Configuration 9.0.3
- Microsoft.Extensions.Configuration.Binder 9.0.3
- Microsoft.Extensions.Configuration.FileExtensions 9.0.3
- Microsoft.Extensions.Configuration.Json 9.0.3
- Microsoft.Extensions.DependencyInjection 9.0.3
- Microsoft.Extensions.Http 9.0.3
- Spectre.Console 0.49.1
- Spectre.Console.Cli 0.49.1
- System.CommandLine 2.0.0-beta4.22272.1
- System.Linq.Async 6.0.1

## Build and Run
- Build: `dotnet build -c Release`
- Run: `dotnet run -- <args>` from the project folder, or run the produced `wk` executable.

## HttpClient Configuration
`Program.cs` registers named HttpClients:
- `jira` → Base `https://rollick.atlassian.net/rest/api/3/` (Basic auth with Jira token)
- `tempo` → Base `https://api.tempo.io/4/` (Bearer auth with Tempo token)

Authentication values are provided by `TokenManager` (Windows Credential Manager).

