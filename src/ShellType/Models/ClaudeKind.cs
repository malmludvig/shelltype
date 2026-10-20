namespace ShellType.Models;

/// <summary>The kinds of things you use in Claude Code, from slash commands to settings files.</summary>
public enum ClaudeKind
{
    /// <summary>Typed at the prompt after a slash, like <c>/compact</c>.</summary>
    Slash,

    /// <summary>A built-in skill, run like a slash command, like <c>/code-review</c>.</summary>
    Skill,

    /// <summary>A key combination, like <c>Shift+Tab</c>.</summary>
    Shortcut,

    /// <summary>A character at the start of the prompt, like <c>!</c> for shell mode.</summary>
    Prefix,

    /// <summary>A command typed in the terminal, like <c>claude -c</c>.</summary>
    Cli,

    /// <summary>A flag passed when starting claude, like <c>--model</c>.</summary>
    Flag,

    /// <summary>A permission mode, like <c>acceptEdits</c>.</summary>
    Mode,

    /// <summary>A file or idea, like <c>CLAUDE.md</c> or hooks.</summary>
    Concept,
}

public static class ClaudeKinds
{
    /// <summary>Plural name used for headings and chips, e.g. "slash commands".</summary>
    public static string Label(ClaudeKind kind) => kind switch
    {
        ClaudeKind.Slash => "slash commands",
        ClaudeKind.Skill => "skills",
        ClaudeKind.Shortcut => "shortcuts",
        ClaudeKind.Prefix => "prompt prefixes",
        ClaudeKind.Cli => "cli commands",
        ClaudeKind.Flag => "cli flags",
        ClaudeKind.Mode => "permission modes",
        _ => "files & concepts",
    };

    /// <summary>The question asked when you are shown what something does and have to name it.</summary>
    public static string WhichQuestion(ClaudeKind kind) => kind switch
    {
        ClaudeKind.Slash => "which slash command does this?",
        ClaudeKind.Skill => "which skill does this?",
        ClaudeKind.Shortcut => "which shortcut does this?",
        ClaudeKind.Prefix => "which prefix does this?",
        ClaudeKind.Cli => "which cli command does this?",
        ClaudeKind.Flag => "which flag does this?",
        ClaudeKind.Mode => "which permission mode is this?",
        _ => "which file or concept is this?",
    };
}
