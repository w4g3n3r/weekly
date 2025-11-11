# Work Log Format

Weekly uses a plain-text format for both work logs (`.wk`) and templates (`.template.wk`). The same serializer handles reading and writing.

## Example
```
##wkv1

2025-04-21 2025-04-25
X = 1
x = 0.25

mtwr = 9
f = 4

                      m       t       w       r       f 
#--------------------------------------------------------#
ABC-123               -       -      xx       -       - 
ABC-456               x       x       x       x       - 
```

## Sections
- Header: `##wkv1` identifies the file format version.
- Period: `YYYY-MM-DD YYYY-MM-DD` start and end dates (omit in templates).
- Aliases: single-char mappings (e.g., `X = 1`, `x = 0.25`) for quick editing.
- Hours per day: e.g., `mtwr = 9` and `f = 4` used for dynamic allocations (`*`).
- Table: one row per issue (`KEY-N`), one column per day, `-` means no hours.

## Templates
- Must include `TemplateName = <Name>`
- Must include all days in the table
- Should not include a period line

For a longer description and guidance, see `README.md` → “Work Log File Format”.

