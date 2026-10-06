namespace ShellType.Quiz;

/// <summary>
/// Something that went wrong in a terminal, the command that fixes it, and three
/// plausible-looking wrong fixes.
/// </summary>
public sealed record ErrorScenario(
    string Command,
    string Error,
    string Fix,
    string Explanation,
    string[] WrongFixes);
