# Weekly
An awesome little time tracking app.

Weekly is for tracking your time entry over the span of a week, then posting it to Tempo using Jira's issue IDs. It has the following goals:

- **Easy to edit work logs:**
    Weekly stores all of it's data in plain text that is easy to edit with or without tools.
- **Simple time entry:**
    Time entries consist of a task key, and a duration. That's it. 
- **Minimal repetition:**
    Repetitive tasks are automatically logged for you based on user editable templates.

## Installation

1. Copy the latest exe from the releases page and put it somewhere in your system path.
1. Open a command prompt and navigate to the directory where you want to store your work logs. 
    `> cd ~\Documents\Time`
1. Tell Weekly this is the directory you want to use for storing work logs by running the `initialize` command.
    `> wk init`
1. (optional) Review your `config.json` and make any changes. The `initialize` command will tell you where to find it.
1. Update your default template. This file can be found in your template directory. The `initialize` command will tell you where that is. You can use this file to set how many hours you typically work per day, and also have it auto insert repeating time entries into your work log.
1. Add your *Tempo* and *Jira* API tokens to Weekly.
    - You can get a Jira token from your Account's Security page here: https://id.atlassian.com/manage-profile/security
    - You can get a Tempo token from the API Integration page under Settings.
    - Once you have your tokens you can add them via the `wk token add` command. Below are two examples. *Make sure you use the email address for each account as the user name.*
        - `wk token add jira you@example.com your-jira-token`
        - `wk token add tempo you@example.com your-tempo-token` 
    - These tokens are stored in the Windows Credential Manager. They aren't stored in clear text anywhere.

Once you're setup you can use `wk create` to create a new work log for the current period, or `wk add` to add a time entry. Once you have your work log filled out for an entire week, you can use `wk push` to post the entries to Tempo. If you want to see how many hours you have clocked for the week run `wk status`. Help is available via `wk --help`.

## Work Log File Format
Below is an example of what a work log looks like.

```
##wkv1

2025-04-21 2025-04-25
X = 1
x = 0.25

mtwr = 9
f = 4


                      m       t       w       r       f 
#--------------------------------------------------------#
TIME-1                -       -      xx       -       - 
TIME-2                x       x       x       x       - 
TIME-3               xx       -       -       -       - 
TIME-4             0.75    1.25       -      Xx       - 
TIME-6             4.00       -       -       -       - 
TIME-7                *       *       -       -       - 
TIME-8                -       1       -       -       - 
TIME-9                -    0.50       -       -       - 
```
The first line `##wkv1` is the file header. It identifies the file version. This file won't be processed as a work log if that header doesn't exist.

The next line `2025-04-21 2025-04-25` is the work log period. This identifies the days contained within the file.

The next two lines `X = 1` and `x = 0.25` are *aliases*. Aliases are useful if you plan to hand edit work log files. These two aliases set a capital "X" equal to 1 hour, and a lower case "x" equal to 15 minutes. This allows you to easily add time to a task in the file just by adding or removing one of these characters to it's current value for a given day. Aliases can be combined with other aliases and also numerical values. You can define aliases for any single character that doesn't represent a day of the week (umtwrfs).

The next two lines `mtwr = 9` and `f = 4` set the number of hours per weekday. In this example, Monday, Tuesday, Wednesday and Thursday are all expected to have 9 hours, and Friday 4 hours. (If these values don't exist in the work log, the default number of hours set by `HoursPerDay` will be used.) This is useful if you want to use *dynamic* values for tasks. For example, on Monday TIME-7 has a "\*". This means that TIME-7 will take the remainder of the time left in the 9 hour day. If you wanted to split the remainder of the day between two tasks, you can give each of them an asterisk and the time will be split evenly. If you want one task to take up twice as much remaining time, you can give it two asterisks "\*\*" and the other task a single asterisk "\*". This will work for as many tasks, or asterisks you want to divvy up.

The remainder of the file contains the actual work log. The header lists the days of the week in the order that they occur in this time period (u = Sunday, m = Monday, t = Tuesday, w = Wednesday, r = Thursday, f = Friday, and s = Saturday). The maximum number of days that can appear in a work log is 7, and the minimum is 1. The setting `DaysPerFile` determines how many appear in the file. The setting `WeekBeginsOn` determines what the first day will be. These values are used, along with the current date when determining what the current period is.

Each row represents a single Jira issue, and each column the number of hours worked for that day in the current period. Each issue can only have one row. The columns must be separated with at least one space, and any empty columns must have a "-".

Templates have a similar format with a few exceptions:
- Templates must have a TemplateName set: `TemplateName = MyTemplate`.
- Templates should not have a work log period defined.
- Templates require every day to be included in the work log table.
