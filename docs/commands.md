# Commands

This project uses `System.CommandLine` and `Spectre.Console` to provide a friendly CLI.
Root command description: "Weekly, an awesome little time keeping app."

## init (alias: i)
- Description: Initialize the time keeping directory.
- Options:
  - `--directory`, `-d` string: Directory in which to store files.
- Example:
  - `wk init -d C:\\Users\\you\\Documents\\Time`

## create
- Description: Create a new work log for a selected period.
- Options:
  - `--workLog`, `-w` string: File name of the work log (defaults to `yyyyMMdd`). Use `p` for previous period.
  - `--startDate`, `-s` DateTime: Start date; defaults to current period start.
  - `--endDate`, `-e` DateTime: End date; if omitted, computed using `DaysPerFile`.
  - `--template`, `-t` string: Template to use; defaults to `DefaultTemplateName`.
  - `--force`, `-f` bool: Overwrite if a log with the same name exists.
- Examples:
  - `wk create -s 2025-04-21 -e 2025-04-25 -t default`
  - `wk create -w p` (create worklog for previous period)

## add (alias: a)
- Description: Add a time entry to a work log.
- Options:
  - `--workLog`, `-w` string: Target work log; defaults to current log. Use `p` for previous period.
  - `--template`, `-t` string: Template to use if creating a new log.
- Arguments (positional, flexible order):
  - `day` (optional): One of `s m t w r f u`. Defaults to current day.
  - `issueId` (required): Matches `KEY-123` pattern.
  - `hours` (optional): Number or alias; defaults to `MinimumHourValue` from config.
- Examples:
  - `wk add m ABC-123 1.5`
  - `wk add ABC-123 x` (uses alias `x` = 0.25 hours if configured)
  - `wk add -w p ABC-123 2` (add to previous period worklog)

## status (alias: s)
- Description: Show the status of a work log as a table with per-day totals.
- Options:
  - `--workLog`, `-w` string: Which log to show; defaults to current. Use `p` for previous period.
- Examples:
  - `wk status -w 20250421`
  - `wk status -w p` (show status of previous period)

## push (alias: p)
- Description: Push the current work log’s entries to Tempo.
- Options:
  - `--workLog`, `-w` string: Log to push; defaults to current. Use `p` for previous period.
  - `--force`, `-f` bool: If entries exist remotely for the period, delete them first.
- Behavior:
  - When `--force` is set, existing remote entries are deleted in parallel before pushing.
  - Shows success/failure counts for push/delete operations.
- Examples:
  - `wk push --force`
  - `wk push -w p --force` (push previous period with force)

## token
- Description: Manage API tokens used for Jira (Basic) and Tempo (Bearer).
- Subcommands:
  - `add <token name> <user name> <secret>`: Add/update a token.
  - `rm <token name>`: Remove a stored token.
  - `clr`: Clear all stored tokens created by Weekly.
- Storage: Windows Credential Manager with names prefixed by `weekly.`
- Examples:
  - `wk token add jira you@example.com <jira-api-token>`
  - `wk token add tempo you@example.com <tempo-api-token>`
  - `wk token rm tempo`
  - `wk token clr`

