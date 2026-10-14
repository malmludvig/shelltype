using ShellType.Models;

namespace ShellType.Quiz;

public enum QuestionKind
{
    /// <summary>Shown a command, pick what it does.</summary>
    WhatDoesItDo,

    /// <summary>Shown a command and the error it printed, pick the fix.</summary>
    FixTheError,

    /// <summary>Shown just a program name like <c>grep</c>, pick what it is for.</summary>
    WhatIsThisCommand,
}

/// <summary>A multiple-choice question.</summary>
/// <param name="Prompt">The command that is shown (or that was run, for errors).</param>
/// <param name="ErrorOutput">What the terminal printed, for <see cref="QuestionKind.FixTheError"/>.</param>
/// <param name="Options">The choices, already shuffled.</param>
/// <param name="AnswerIndex">Index of the right choice in <paramref name="Options"/>.</param>
/// <param name="Explanation">Shown after answering, to teach why.</param>
/// <param name="Shell">The prompt shown before the command, like <c>~$</c> or <c>R1(config)#</c>.</param>
public sealed record QuizQuestion(
    QuestionKind Kind,
    string Prompt,
    string? ErrorOutput,
    IReadOnlyList<string> Options,
    int AnswerIndex,
    string Explanation,
    string Shell = CommandEntry.ShellPrompt)
{
    public string Answer => Options[AnswerIndex];
}
