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

- **Three modes:** *time* (15 / 30 / 60 / 120 s), *commands* (5 / 10 / 25 / 50) and *operations*
- **Operations:** 86 everyday tasks typed as command sequences, from finding your ip address (2 steps) to securing a fresh server (8-10 steps), each step explaining why it is done
- **Cisco IOS:** switch the platform to practice configuring routers and switches. Every command is shown at its mode's prompt (`R1#`, `R1(config)#`, `R1(config-if)#`), with time and commands tests, 23 operations (ssh, OSPF, DHCP, NAT, VLANs, trunks, ACLs and more) and an *all 50* mode that runs through the 50 most common IOS commands, plus a *cisco ios* quiz that asks both what a command does and which command does a task
- **Claude Code:** switch the platform to learn [Claude Code](https://code.claude.com/docs/en/commands). Slash commands are typed at Claude's `>` prompt and the `claude` cli and flags at `~$`, with time and commands tests, 16 workflow operations (freeing up context, reviewing before a push, running Claude in CI, shipping a feature end to end) and an *all* mode that runs through every typeable command
- **Claude Code cheat sheet:** the *claude* page lists 156 slash commands, skills, shortcuts, prompt prefixes, cli commands, flags, permission modes and files, searchable with everyday words (`undo` finds `/rewind`), with examples, a command of the day, known-marking, per-group progress, flashcards in five Leitner boxes, a day streak and export, import and reset of your progress
- **Claude Code quiz:** *claude code* asks both ways round (what does it do, which one does this), *scenarios* has 24 real situations and *type it* has you type the command from memory, accepting aliases and the macOS keys
- **200+ commands** in 11 categories: navigation, files, text, search, processes, networking, permissions, packages, archives, system and git
- **Three difficulty levels**, from `pwd` to `awk -F, '{sum += $3} END {print sum}' sales.csv`
- **Learn as you type:** every command shows a short `# comment` explaining it
- **Quiz mode:** *what does this do?*, *fix the error* (26 real-world errors like `Permission denied (publickey)`) and *basics* (what is `grep`, `sysctl`, `lsof` for? 250 common commands), answered with an fzf-style fuzzy picker, with a speed bonus and combo multiplier, a 60 second time attack, a retry of the questions you missed and results per topic
- **Mobile friendly quiz:** on a phone the answers become big tap targets, with a next button in thumb reach
- **Theory:** 30 short chapters (about a page each) on how Linux works: filesystem layout, permissions, processes, signals, memory, boot, systemd, logs, the shell, pipes, regex, networking, DNS, firewalls and ssh, with read-progress tracking
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

On a phone, just tap an answer. On a keyboard, type to fuzzy-filter the answers, move with <kbd>↑</kbd> <kbd>↓</kbd> or <kbd>Ctrl</kbd> + <kbd>J</kbd> / <kbd>K</kbd>, answer with <kbd>Enter</kbd>, clear the filter with <kbd>Esc</kbd>.

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
  Data/        command catalogue (one file per category), operations, basic commands, error scenarios and theory chapters
  Data/Cisco/  the Cisco IOS commands and operations
  Data/Claude/ the Claude Code cheat sheet, scenarios and operations
  Game/        UI-free engine: TypingSession, CommandAttempt, StatsCalculator, FlashcardDeck
  Quiz/        quiz engine: QuizSession, QuizGenerator, FuzzyMatcher, ClaudeAnswers
  Theory/      the small Markdown parser theory chapters are written in
  Services/    settings, history, Claude Code progress and localStorage persistence
  Components/  TerminalView, ConfigBar, ResultsView, WpmChart, ...
  Pages/       type, quiz, theory, learn, claude, stats, settings, about
  wwwroot/     themes, styles and the small JS keyboard bridge
tests/ShellType.Tests/   xUnit tests for the engine and library
```

The game engine has no Blazor dependencies, so it is fully unit-tested with a fake clock.

## Adding commands

Add a line to the matching `src/ShellType/Data/CommandLibrary.<Category>.cs`:

```csharp
("du -sh *", "Show the size of each item in the current directory", Medium),
```

Operations live in `src/ShellType/Data/OperationLibrary.cs`. Each one is a name, a goal and 2-10 steps:

```csharp
Op("free a busy port", "port 8080 is already in use. find who holds it and stop it.", Networking,
    ("sudo ss -ltnp | grep :8080", "Find the process listening on port 8080"),
    ("sudo kill 3127", "Stop the process holding the port")),
```

Claude Code entries live in `src/ShellType/Data/Claude/`. Give a slash command an example to type, and everyday words for search:

```csharp
Slash("/compact [instructions]", "Free up context by summarizing the conversation so far, optionally with what to focus on",
    example: "/compact keep only the database decisions", keywords: "full context memory summarize tokens"),
```

Tests check that every command is unique, printable ASCII and has a description.

## Deployment

Every push to `main` runs CI and deploys to GitHub Pages via `.github/workflows/deploy.yml`.

## License

[MIT](LICENSE)
