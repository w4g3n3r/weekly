# AGENTS.md

Welcome, fellow Codex agents! This file gives you quick, actionable guidance for working in this repository and links to the in-repo documentation.

## Quick Links
- Solution overview: `docs/solution.md`
- Architecture: `docs/architecture.md`
- Commands (CLI usage): `docs/commands.md`
- Configuration: `docs/configuration.md`
- Work log format: `docs/worklog-format.md`
- Docs index: `docs/README.md`

## Project Snapshot
- App type: .NET 8 console app (assembly name: `wk`)
- Solution: `Weekly.sln`
- Project: `Weekly.csproj`
- Entry point: `Program.cs`
- DI wiring: `Program.cs` registers commands, services, and HttpClients
- Core service: `Services/WeeklyService*.cs`
- Infrastructure: `Infrastructure/*` (FileManager, TokenManager, CacheManager, ApiManager)
- Commands: `Commands/*`
- Serializers: `Serializers/*`
- Exceptions: `Exceptions/*`

## Build and Run
- Build: `dotnet build -c Release`
- Run: `dotnet run -- <args>` from the project folder, or run the produced `wk` binary.
- Root command description: "Weekly, an awesome little time keeping app."

## Commands At A Glance
- `wk init [-d <dir>]` — initialize work log/templates and write config
- `wk create [options]` — create a new work log for a period
- `wk add [day] <KEY-123> [hours]` — add time to an issue
- `wk status [-w <name>]` — show table of hours and totals
- `wk push [--force]` — push local entries to Tempo (optionally delete/replace)
- `wk token add|rm|clr` — manage Jira (Basic) and Tempo (Bearer) tokens

See `docs/commands.md` for full options and examples.

## Configuration and Files
- Config path: `%APPDATA%/Weekly/config.json`
- Key settings: week start day, hours/day, days/file, min unit, directories, defaults
- Work logs: `.wk` files in your configured WorkLogDirectory
- Templates: `.template.wk` files in `<WorkLogDirectory>/<TemplateDirectoryName>`
- Format details and examples: `docs/worklog-format.md`

## HTTP, Tokens, and Caching
- Named HttpClients in `Program.cs`:
  - `jira` → `https://rollick.atlassian.net/rest/api/3/` (Basic auth)
  - `tempo` → `https://api.tempo.io/4/` (Bearer auth)
- Token storage: Windows Credential Manager via `Infrastructure/TokenManager.cs`
  - Names are prefixed with `weekly.`; do not hardcode secrets in source
- Lightweight API response cache: `Infrastructure/CacheManager.cs` (files beside config)

## Versioning Notes
- `Weekly.csproj` sets `<Version>0.1.0` while `Directory.build.props` sets `0.1.1`
- Effective version depends on MSBuild import order. If releasing, align these values.

## Coding Conventions and Tips
- Language: C# with nullable + implicit usings enabled
- Keep changes small, focused, and consistent with current style
- Prefer existing abstractions:
  - File IO via `FileManager`
  - API calls via `ApiManager`
  - Orchestration via `WeeklyService`
  - Output/styling/error handling via `BaseCommand` helpers and Spectre.Console
- Do not introduce new dependencies unless necessary for the task
- Prefer minimal, local changes over broad refactors

## Testing and Validation
- There is no test suite in-repo. Validate behavior by:
  - Building the project and running commands with benign inputs
  - Using `wk status` to check rendering and totals against a sample `.wk`
  - Avoid making real network calls during routine CI/sandboxed runs
- If you must touch API paths, consider adding guardrails or toggles to disable network.

## Safety and Secrets
- Never print token values or write them to logs/files
- Use `wk token` commands for management
- Be careful when proposing changes to API endpoints or auth; verify against docs

## Common Agent Tasks
- Documentation: Update `docs/*` and link from `docs/README.md`
- New command: Create a command class in `Commands/`, register in `Program.cs`, and reuse `BaseCommand` utilities
- Format support: Extend serializers under `Serializers/` if the `.wk` format evolves
- Error messages: Prefer throwing `ProgramException` subclasses from `Exceptions/*` for user-friendly output

## AGENTS.md Scope and Precedence
- This file applies to the entire repo. If a more specific `AGENTS.md` is added in a subdirectory, it takes precedence for files in that subtree.
- Direct system/developer/user instructions always override this file.

Happy shipping! If you improve the workflow for future agents, please update this file and the docs.

