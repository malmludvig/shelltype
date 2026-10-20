namespace ShellType.Models;

/// <summary>
/// One entry on the Claude Code cheat sheet: a slash command, shortcut, cli command, flag,
/// permission mode or concept, with what it does.
/// </summary>
/// <param name="Name">As it is written in the docs, with placeholders, e.g. <c>/compact [instructions]</c>.</param>
/// <param name="Aliases">Other names that do the same thing, e.g. <c>/reset</c> and <c>/new</c> for <c>/clear</c>.</param>
/// <param name="Note">A short extra, like the macOS key or the long form of a flag.</param>
/// <param name="Example">A concrete line to type, e.g. <c>/compact keep only the database decisions</c>.</param>
/// <param name="Keywords">Everyday words that should find it in search, e.g. "undo" for <c>/rewind</c>.</param>
public sealed record ClaudeCommand(
    string Name,
    string Description,
    ClaudeKind Kind,
    IReadOnlyList<string> Aliases,
    string? Note,
    string? Example,
    string Keywords)
{
    public const string ClaudePrompt = ">";

    /// <summary>The name without placeholders: <c>/compact</c> for <c>/compact [instructions]</c>.</summary>
    public string Key
    {
        get
        {
            var name = Name;
            foreach (var marker in new[] { " [", " <" })
            {
                var at = name.IndexOf(marker, StringComparison.Ordinal);
                if (at > 0)
                {
                    name = name[..at];
                }
            }

            return name;
        }
    }

    /// <summary>Where it is typed: the Claude Code prompt, the terminal, or neither for keys and concepts.</summary>
    public string Prompt => Kind switch
    {
        ClaudeKind.Slash or ClaudeKind.Skill or ClaudeKind.Prefix => ClaudePrompt,
        ClaudeKind.Cli or ClaudeKind.Flag or ClaudeKind.Mode => CommandEntry.ShellPrompt,
        ClaudeKind.Shortcut => "keys",
        _ => "#",
    };
}
