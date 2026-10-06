namespace ShellType.Quiz;

public enum QuestionKind
{
    /// <summary>Shown a command, pick what it does.</summary>
    WhatDoesItDo,

    /// <summary>Shown a command and the error it printed, pick the fix.</summary>
    FixTheError,
}

/// <summary>A multiple-choice question.</summary>
/// <param name="Prompt">The command that is shown (or that was run, for errors).</param>
/// <param name="ErrorOutput">What the terminal printed, for <see cref="QuestionKind.FixTheError"/>.</param>
/// <param name="Options">The choices, already shuffled.</param>
/// <param name="AnswerIndex">Index of the right choice in <paramref name="Options"/>.</param>
/// <param name="Explanation">Shown after answering, to teach why.</param>
public sealed record QuizQuestion(
    QuestionKind Kind,
    string Prompt,
    string? ErrorOutput,
    IReadOnlyList<string> Options,
    int AnswerIndex,
    string Explanation)
{
    public string Answer => Options[AnswerIndex];
}
