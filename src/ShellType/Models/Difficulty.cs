namespace ShellType.Models;

/// <summary>
/// How long and symbol-heavy a command is. Easy commands are short one-liners,
/// hard commands contain pipes, quotes, flags and paths.
/// </summary>
public enum Difficulty
{
    Easy = 1,
    Medium = 2,
    Hard = 3,
}
