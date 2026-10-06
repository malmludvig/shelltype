using ShellType.Data;
using ShellType.Models;

namespace ShellType.Quiz;

public enum QuizMode
{
    Mixed,
    Commands,
    Errors,
}

/// <summary>Builds a shuffled set of quiz questions.</summary>
public static class QuizGenerator
{
    public const int OptionCount = 4;
    public static readonly int[] LengthOptions = [10, 20];

    public static IReadOnlyList<QuizQuestion> Create(QuizMode mode, int count, Random? random = null)
    {
        random ??= Random.Shared;

        var (commandCount, errorCount) = mode switch
        {
            QuizMode.Commands => (count, 0),
            QuizMode.Errors => (0, count),
            _ => (count - count / 2, count / 2),
        };
        errorCount = Math.Min(errorCount, ErrorScenarios.All.Count);

        var questions = new List<QuizQuestion>();
        questions.AddRange(Shuffle(CommandLibrary.All, random).Take(commandCount).Select(c => FromCommand(c, random)));
        questions.AddRange(Shuffle(ErrorScenarios.All, random).Take(errorCount).Select(s => FromError(s, random)));
        return Shuffle(questions, random);
    }

    public static QuizQuestion FromCommand(CommandEntry command, Random random)
    {
        // Distractors from the same category are harder, and teach more, than random ones.
        var sameCategory = CommandLibrary.All.Where(c => c.Category == command.Category);
        var distractors = Shuffle(sameCategory, random)
            .Concat(Shuffle(CommandLibrary.All, random))
            .Select(c => c.Description)
            .Where(d => d != command.Description)
            .Distinct()
            .Take(OptionCount - 1);

        var options = Shuffle(distractors.Append(command.Description), random);
        return new QuizQuestion(
            QuestionKind.WhatDoesItDo,
            command.Text,
            null,
            options,
            IndexOf(options, command.Description),
            $"{command.Program} is one of the {command.Category.ToString().ToLowerInvariant()} commands. See them all on the learn page.");
    }

    public static QuizQuestion FromError(ErrorScenario scenario, Random random)
    {
        var options = Shuffle(scenario.WrongFixes.Append(scenario.Fix), random);
        return new QuizQuestion(
            QuestionKind.FixTheError,
            scenario.Command,
            scenario.Error,
            options,
            IndexOf(options, scenario.Fix),
            scenario.Explanation);
    }

    private static int IndexOf(IReadOnlyList<string> options, string value)
    {
        for (var i = 0; i < options.Count; i++)
        {
            if (options[i] == value)
            {
                return i;
            }
        }

        throw new InvalidOperationException("Answer missing from options.");
    }

    private static List<T> Shuffle<T>(IEnumerable<T> items, Random random)
    {
        var list = items.ToList();
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        return list;
    }
}
