# shelltype

A minimalist, [Monkeytype](https://monkeytype.com)-style typing test for **Linux commands**, built with C# and Blazor WebAssembly.

Instead of random words you type real command lines (flags, pipes, paths and quotes included) and learn what each one does along the way.

```
~$ grep -rn TODO src/
   # Recursively search with line numbers
~$ tar -czf backup.tar.gz project/
~$ sudo systemctl restart nginx
```

**Play it:** https://malmludvig.github.io/shelltype/

## Features

- **Two modes:** *time* (15 / 30 / 60 / 120 s) and *commands* (5 / 10 / 25 / 50)
- **200+ commands** in 11 categories: navigation, files, text, search, processes, networking, permissions, packages, archives, system and git
- **Three difficulty levels**, from `pwd` to `awk -F, '{sum += $3} END {print sum}' sales.csv`
- **Learn as you type:** every command shows a short `# comment` explaining it
- **Quiz mode:** *what does this do?* and *fix the error* questions (26 real-world errors like `Permission denied (publickey)`), answered with an fzf-style fuzzy picker, with a speed bonus and combo multiplier
- **Results screen** with WPM, raw WPM, accuracy, consistency, a speed-over-time chart and a per-command breakdown
- **Personal bests & history**, stored locally in your browser
- **Weak-command practice:** shelltype tracks which commands trip you up and drills them
- **8 themes**, caret styles, font sizes, optional key sounds
- **Strict modes:** *stop on error* and *confidence mode* (no backspace)

## Controls

| key | action |
| --- | --- |
| *any character* | type (the timer starts on the first key) |
| <kbd>Enter</kbd> | run the command and move to the next |
| <kbd>Backspace</kbd> | delete a character |
| <kbd>Ctrl</kbd> + <kbd>Backspace</kbd> | delete a word |
| <kbd>Tab</kbd> / <kbd>Esc</kbd> | restart |

In the quiz, type to fuzzy-filter the answers, move with <kbd>↑</kbd> <kbd>↓</kbd> or <kbd>Ctrl</kbd> + <kbd>J</kbd> / <kbd>K</kbd>, answer with <kbd>Enter</kbd>, clear the filter with <kbd>Esc</kbd>.

## How stats are calculated

- **wpm**: characters from correctly typed commands (plus one for each Enter) ÷ 5 ÷ minutes
- **raw**: every character typed ÷ 5 ÷ minutes
- **accuracy**: correct keystrokes ÷ all keystrokes, so corrected mistakes still count
- **consistency**: how steady your per-second speed was (100% = perfectly even)

## Running locally

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet run --project src/ShellType      # http://localhost:5104
dotnet test                             # engine unit tests
```

## Project layout

```
src/ShellType/
  Data/        command catalogue (one file per category) and error scenarios
  Game/        UI-free engine: TypingSession, CommandAttempt, StatsCalculator
  Quiz/        quiz engine: QuizSession, QuizGenerator, FuzzyMatcher
  Services/    settings, history and localStorage persistence
  Components/  TerminalView, ConfigBar, ResultsView, WpmChart, ...
  Pages/       type, quiz, learn, stats, settings, about
  wwwroot/     themes, styles and the small JS keyboard bridge
tests/ShellType.Tests/   xUnit tests for the engine and library
```

The game engine has no Blazor dependencies, so it is fully unit-tested with a fake clock.

## Adding commands

Add a line to the matching `src/ShellType/Data/CommandLibrary.<Category>.cs`:

```csharp
("du -sh *", "Show the size of each item in the current directory", Medium),
```

Tests check that every command is unique, printable ASCII and has a description.

## Deployment

Every push to `main` runs CI and deploys to GitHub Pages via `.github/workflows/deploy.yml`.

## License

[MIT](LICENSE)
