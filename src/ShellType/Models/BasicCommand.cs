namespace ShellType.Models;

/// <summary>
/// A program on its own, without arguments, and what it is for. Used by the
/// "what is this command?" quiz.
/// </summary>
public sealed record BasicCommand(string Name, string Description, CommandCategory Category);
