using System.Text.RegularExpressions;
using ShellType.Models;

namespace ShellType.Quiz;

/// <summary>
/// Decides whether a typed answer names a Claude Code command. Case, spaces around <c>+</c> and quotes
/// do not matter, aliases count, and placeholders like <c>"query"</c> may be swapped for anything.
/// </summary>
public static partial class ClaudeAnswers
{
    /// <summary>Lower case, no quotes, single spaces and <c>ctrl + r</c> written as <c>ctrl+r</c>.</summary>
    public static string Normalize(string text) =>
        Spaces().Replace(Plus().Replace(text.Trim().ToLowerInvariant(), "+").Replace("\"", "").Replace("'", ""), " ");

    /// <summary>The names that count: <c>--worktree</c> and <c>-w</c>, aliases, and the macOS or Windows key.</summary>
    public static IReadOnlyList<string> Accepted(ClaudeCommand command)
    {
        var names = command.Key.Split(", ").Concat(command.Aliases).ToList();
        if (command.Note is { } note && OtherKey().Match(note) is { Success: true } key)
        {
            names.Add(key.Groups[1].Value);
        }

        return names.Select(Normalize).Distinct().ToList();
    }

    public static bool IsCorrect(string input, ClaudeCommand command)
    {
        var typed = Normalize(input);
        if (typed.Length == 0)
        {
            return false;
        }

        // A cli command with a placeholder, like claude -p "query": anything may stand in for it.
        if (command.Kind == ClaudeKind.Cli && Placeholder().IsMatch(command.Name))
        {
            var fixedPart = Normalize(Placeholder().Replace(command.Name, ""));
            // ...but not a flag: claude -c is not claude "query".
            if (typed == Normalize(command.Name)
                || (typed.StartsWith(fixedPart + " ", StringComparison.Ordinal) && typed[(fixedPart.Length + 1)..][0] != '-'))
            {
                return true;
            }
        }

        // "claude --model" counts as "--model", and "/model opus" as "/model".
        var withoutClaude = typed.StartsWith("claude -", StringComparison.Ordinal) ? typed[7..] : typed;
        var firstWord = withoutClaude.Split(' ')[0];
        return Accepted(command).Any(a =>
            a == typed
            || a == withoutClaude
            || (a.StartsWith('/') && a == firstWord)
            || (a.StartsWith('-') && a.Split(' ')[0] == firstWord));
    }

    [GeneratedRegex(@"\s*\+\s*")]
    private static partial Regex Plus();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Spaces();

    [GeneratedRegex(@"^(?:macOS|Windows[^:]*|Long form): (.+)$")]
    private static partial Regex OtherKey();

    /// <summary>A quoted or angle-bracketed placeholder at the end, like <c> "query"</c> or <c> &lt;id&gt;</c>.</summary>
    [GeneratedRegex(@"\s*(""[^""]*""|<[^>]*>)\s*$")]
    private static partial Regex Placeholder();
}
