namespace ShellType.Models;

/// <summary>Which command line the commands are for.</summary>
public enum Platform
{
    /// <summary>A Linux shell.</summary>
    Linux,

    /// <summary>The Cisco IOS command line on routers and switches.</summary>
    Cisco,

    /// <summary>Claude Code: slash commands at its prompt and the claude cli in the terminal.</summary>
    Claude,

    /// <summary>C# LINQ statements, typed at a C# REPL prompt.</summary>
    Linq,
}
