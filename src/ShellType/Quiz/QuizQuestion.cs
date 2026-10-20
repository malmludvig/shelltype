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

    /// <summary>Shown what a command does and the mode's prompt, pick the command.</summary>
    WhichCommand,

    /// <summary>Shown a real situation, pick the command that solves it.</summary>
    Scenario,

    /// <summary>Shown what a command does, type the command from memory.</summary>
    TypeIt,
}

/// <summary>A multiple-choice question, or a typed one when <paramref name="Accepted"/> is set.</summary>
/// <param name="Prompt">The command that is shown (or that was run, for errors).</param>
/// <param name="ErrorOutput">What the terminal printed, for <see cref="QuestionKind.FixTheError"/>.</param>
/// <param name="Options">The choices, already shuffled. A typed question has one: the answer.</param>
/// <param name="AnswerIndex">Index of the right choice in <paramref name="Options"/>.</param>
/// <param name="Explanation">Shown after answering, to teach why.</param>
/// <param name="Shell">The prompt shown before the command, like <c>~$</c> or <c>R1(config)#</c>.</param>
/// <param name="Topic">The group the question is about, for the per-topic results, e.g. "slash commands".</param>
/// <param name="Accepted">Every answer that counts as right for a typed question, already normalized.</param>
public sealed record QuizQuestion(
    QuestionKind Kind,
    string Prompt,
    string? ErrorOutput,
    IReadOnlyList<string> Options,
    int AnswerIndex,
    string Explanation,
    string Shell = CommandEntry.ShellPrompt,
    string? Topic = null,
    IReadOnlyList<string>? Accepted = null)
{
    public string Answer => Options[AnswerIndex];

    /// <summary>Answered by typing rather than picking from the options.</summary>
    public bool IsTyped => Accepted is not null;

    /// <summary>The prompt shows a task or situation and asks for the command, rather than showing a command.</summary>
    public bool AsksForCommand => Kind is QuestionKind.WhichCommand or QuestionKind.Scenario or QuestionKind.TypeIt;
}
