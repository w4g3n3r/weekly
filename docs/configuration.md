# Configuration

Configuration is stored as JSON at:
- Windows: `%APPDATA%\\Weekly\\config.json`

The application loads `Configuration.Default` on first run and writes a new config file if one does not exist. Subsequent runs load and merge with defaults to ensure backwards compatibility.

## Keys
- `WeekBeginsOn` (string)
  - Day the week begins: `u m t w r f s`
  - Default: `"m"` (Monday)
- `HoursPerDay` (int)
  - Default working hours per day
  - Default: `8`
- `DaysPerFile` (int)
  - Days included per work log file
  - Default: `5`
- `MinimumHourValue` (double)
  - Smallest dynamic value used for calculations
  - Default: `0.25`
- `WorkLogDirectory` (string?)
  - Absolute path where your `.wk` files are stored
  - Used to determine initialization status
- `TemplateDirectoryName` (string)
  - Folder inside `WorkLogDirectory` that contains templates
  - Default: `"Templates"`
- `DefaultTemplateName` (string)
  - Default template name used for new logs
  - Default: `"default"`
- `LogExceptionsToConsole` (bool?)
  - When true, prints inner exceptions for troubleshooting
  - Default: `false`
- `ShowHelpMessages` (bool?)
  - When true, prints help messages alongside errors
  - Default: `true`

## Programmatic Access
- `Configuration.Path`: Full file path to the JSON config file.
- `Configuration.Create()`: Creates and saves a default config file.
- `Configuration.Load()`: Loads config (merging with defaults for missing values).
- `Configuration.TrySave()`: Saves current values to disk.
- `Configuration.IsInitialized()`: True when `WorkLogDirectory` is set and exists.

