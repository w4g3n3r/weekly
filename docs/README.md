# Weekly Documentation

Welcome to the documentation for Weekly, an awesome little time tracking CLI that helps you manage weekly work logs and sync them with Tempo for Jira.

- CLI name: `wk`
- Target framework: .NET 8 (`net8.0`)
- Project: `Weekly.csproj` (assembly name `wk`)

## Contents
- [Solution](./solution.md)
- [Architecture](./architecture.md)
- [Commands](./commands.md)
- [Configuration](./configuration.md)
- [Work Log Format](./worklog-format.md)

## Quick Start
1) Place `wk` on your PATH and open a terminal in your time-logs folder
2) Initialize storage: `wk init -d <path>` (or run in the folder and omit `-d`)
3) Configure tokens:
   - Jira: `wk token add jira you@example.com <jira-api-token>`
   - Tempo: `wk token add tempo you@example.com <tempo-api-token>`
4) Create a log: `wk create` (uses default template and current period)
5) Add time: `wk add m ABC-123 1.5`
6) Review status: `wk status`
7) Push to Tempo: `wk push`

For an introduction and example work log, see `README.md` in the repo.

