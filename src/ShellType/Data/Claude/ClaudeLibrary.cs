using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

/// <summary>
/// The Claude Code cheat sheet: slash commands, skills, shortcuts, prompt prefixes, the claude cli,
/// its flags, the permission modes and the files and ideas behind them. Each group lives in its own partial file.
/// </summary>
public static partial class ClaudeLibrary
{
    private static readonly Lazy<IReadOnlyList<ClaudeCommand>> AllCommands =
        new(() => Sources().SelectMany(s => s).ToList());

    private static readonly Lazy<IReadOnlyList<CommandEntry>> TypingCommands =
        new(() => AllCommands.Value.Select(ToEntry).OfType<CommandEntry>().ToList());

    /// <summary>Everything on the cheat sheet, grouped by kind.</summary>
    public static IReadOnlyList<ClaudeCommand> All => AllCommands.Value;

    /// <summary>
    /// Every command that can be typed, at the prompt it is typed at: slash commands at Claude's <c>&gt;</c>,
    /// the cli, flags and modes in the terminal. Shortcuts and concepts are not typed, so they are left out.
    /// </summary>
    public static IReadOnlyList<CommandEntry> Essentials => TypingCommands.Value;

    /// <summary>The typeable commands up to a difficulty, for time and commands tests.</summary>
    public static IReadOnlyList<CommandEntry> Filter(Difficulty maxDifficulty) =>
        Essentials.Where(c => c.Difficulty <= maxDifficulty).ToList();

    /// <summary>Finds an entry by its key, like <c>/compact</c> or <c>Shift+Tab</c>.</summary>
    public static ClaudeCommand? Find(string key) => All.FirstOrDefault(c => c.Key == key);

    /// <summary>
    /// The line typed for an entry: its example if it has one, otherwise its bare name for slash commands,
    /// skills and cli commands. Flags, modes and prefixes only make sense with an example.
    /// </summary>
    public static string? TypedText(ClaudeCommand command) =>
        command.Example ?? (command.Kind is ClaudeKind.Slash or ClaudeKind.Skill or ClaudeKind.Cli ? command.Key : null);

    private static CommandEntry? ToEntry(ClaudeCommand command) =>
        TypedText(command) is { } text
            ? new CommandEntry(text, command.Description, CommandCategory.System, Rate(text), command.Prompt)
            : null;

    /// <summary>Short bare commands are easy, quotes, pipes and long lines are hard.</summary>
    private static Difficulty Rate(string text) =>
        text.Contains('"') || text.Contains('|') || text.Length >= 32 ? Hard
        : text.Length <= 12 && !text.Contains(' ') ? Easy
        : Medium;

    private static IEnumerable<ClaudeCommand[]> Sources()
    {
        yield return Basics;
        yield return Setup;
        yield return Background;
        yield return Skills;
        yield return Shortcuts;
        yield return Prefixes;
        yield return Cli;
        yield return Flags;
        yield return Modes;
        yield return Concepts;
    }

    private static ClaudeCommand Cmd(
        ClaudeKind kind, string name, string description, string? aliases, string? note, string? example, string keywords) =>
        new(name,
            description,
            kind,
            aliases?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries) ?? [],
            note ?? (aliases is not null && aliases.StartsWith('/') ? $"Alias: {aliases}" : null),
            example,
            keywords);
}
