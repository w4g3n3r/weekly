# Architecture

Weekly is a single-project .NET console app composed of commands, services, and infrastructure helpers. Dependency Injection wires components together in `Program.cs`.

## Composition Root
- `Program.cs` registers:
  - Configuration singleton (creates or loads `config.json`)
  - Infrastructure: `FileManager`, `TokenManager`, `CacheManager`, `ApiManager`
  - Service: `WeeklyService` (partial class)
  - Serializers: `IWorkLogSerializer` → `DefaultWorkLogSerializer`
  - Commands: `init`, `status`, `add`, `token`, `push`, `create`
  - HttpClients:
    - `jira` → `https://rollick.atlassian.net/rest/api/3/`
    - `tempo` → `https://api.tempo.io/4/`

## Commands (`Commands/*`)
- `BaseCommand`: common rendering and error handling helpers using Spectre.Console
- `InitializeCommand (init|i)`: sets up work log and template directories, writes config
- `CreateCommand`: generates a new work log for a period from a template
- `AddCommand (a)`: adds or creates a work log entry for an issue/day/hours
- `StatusCommand (s)`: displays a tabular view of the current log with totals
- `PushCommand (p)`: pushes local time entries to Tempo, optional delete/replace
- `TokenCommand`: manages Jira/Tempo tokens in Windows Credential Manager

## Services (`Services/*`)
- `WeeklyService` (partial): orchestrates operations across subsystems
  - Work Log operations: open, create, save, list, calculate, template handling
  - API operations: fetch issue details, push/delete/list remote time entries
  - Token operations: add/remove/clear stored tokens

## Infrastructure (`Infrastructure/*`)
- `FileManager`: handles `.wk` files and template pathing/serialization
- `ApiManager`: handles Jira/Tempo HTTP calls using named `HttpClient`
- `TokenManager`: reads/writes credentials via Windows Credential Manager
- `CacheManager`: simple file-backed key/value caching for API responses

## Serializers (`Serializers/*`)
- `IWorkLogSerializer` and `DefaultWorkLogSerializer`: parse and write the `.wk` text format used for work logs and templates.

## Exceptions (`Exceptions/*`)
Domain-specific exceptions normalize error reporting (e.g., `ApiUnauthorizedException`, `WorkLogParseException`, `MissingTokenException`). Commands catch `ProgramException` to present helpful, styled output.

## Resources & Utilities
- `Resources/default.template.wk`: default template content used during init
- `Extensions/NaturalStringComparer`: natural sort for issue keys like `ABC-2` < `ABC-10`

