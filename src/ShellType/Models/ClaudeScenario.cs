namespace ShellType.Models;

/// <summary>A real situation and the Claude Code command that solves it, for the scenario quiz.</summary>
/// <param name="Answer">The <see cref="ClaudeCommand.Key"/> of the command that solves it.</param>
/// <param name="Why">Shown after answering.</param>
public sealed record ClaudeScenario(string Situation, string Answer, string Why);
