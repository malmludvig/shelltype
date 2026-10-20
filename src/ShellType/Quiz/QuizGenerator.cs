using ShellType.Data;
using ShellType.Models;

namespace ShellType.Quiz;

public enum QuizMode
{
    Mixed,
    Commands,
    Errors,
    Basics,

    /// <summary>The 50 most common Cisco IOS commands, asked both ways round.</summary>
    Cisco,

    /// <summary>Claude Code commands, shortcuts and concepts, asked both ways round.</summary>
    Claude,

    /// <summary>Simple C# LINQ statements, asked both ways round.</summary>
    Linq,

    /// <summary>Real situations, answered with the Claude Code command that solves them.</summary>
    ClaudeScenarios,

    /// <summary>Shown what a Claude Code command does, type it from memory.</summary>
    ClaudeTyped,
}

/// <summary>Builds a shuffled set of quiz questions.</summary>
public static class QuizGenerator
{
    public const int OptionCount = 4;
    public static readonly int[] LengthOptions = [10, 20];

    /// <summary>How long a time attack lasts.</summary>
    public static readonly TimeSpan TimeAttack = TimeSpan.FromSeconds(60);

    /// <summary>Enough questions that nobody runs out during a time attack.</summary>
    public const int TimeAttackQuestions = 200;

    public static IReadOnlyList<QuizQuestion> Create(QuizMode mode, int count, Random? random = null)
    {
        random ??= Random.Shared;

        if (mode == QuizMode.Basics)
        {
            return Shuffle(BasicCommands.All, random).Take(count).Select(b => FromBasic(b, random)).ToList();
        }

        if (mode == QuizMode.Cisco)
        {
            // Alternate the two directions: what does this do, and which command does this.
            return Shuffle(CiscoLibrary.Essentials, random)
                .Take(count)
                .Select((c, i) => i % 2 == 0 ? FromCisco(c, random) : WhichCisco(c, random))
                .ToList();
        }

        switch (mode)
        {
            case QuizMode.Claude:
                // Alternate the two directions, like the Cisco quiz.
                return Shuffle(ClaudeLibrary.All, random)
                    .Take(count)
                    .Select((c, i) => i % 2 == 0 ? FromClaude(c, random) : WhichClaude(c, random))
                    .ToList();
            case QuizMode.ClaudeScenarios:
                return Shuffle(ClaudeLibrary.Scenarios, random).Take(count).Select(s => FromScenario(s, random)).ToList();
            case QuizMode.ClaudeTyped:
                return Shuffle(ClaudeLibrary.All, random).Take(count).Select(TypeClaude).ToList();
        }

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

    public static QuizQuestion FromBasic(BasicCommand command, Random random)
    {
        var sameCategory = BasicCommands.All.Where(c => c.Category == command.Category);
        var distractors = Shuffle(sameCategory, random)
            .Concat(Shuffle(BasicCommands.All, random))
            .Select(c => c.Description)
            .Where(d => d != command.Description)
            .Distinct()
            .Take(OptionCount - 1);

        var options = Shuffle(distractors.Append(command.Description), random);
        return new QuizQuestion(
            QuestionKind.WhatIsThisCommand,
            command.Name,
            null,
            options,
            IndexOf(options, command.Description),
            $"{command.Name} is one of the {command.Category.ToString().ToLowerInvariant()} commands. Try {command.Name} --help or man {command.Name} to see what else it can do.");
    }

    /// <summary>Shown a Cisco command at its prompt, pick what it does.</summary>
    public static QuizQuestion FromCisco(CommandEntry command, Random random)
    {
        // Commands typed in the same mode are the easiest to mix up.
        var options = Shuffle(CiscoDistractors(command, random).Select(c => c.Description)
            .Where(d => d != command.Description)
            .Distinct()
            .Take(OptionCount - 1)
            .Append(command.Description), random);
        return new QuizQuestion(
            QuestionKind.WhatDoesItDo,
            command.Text,
            null,
            options,
            IndexOf(options, command.Description),
            $"{command.Text} is typed in {CiscoLibrary.ModeName(command.Prompt)} mode, at the {command.Prompt} prompt.",
            command.Prompt);
    }

    /// <summary>Shown what a Cisco command does and the prompt, pick the command.</summary>
    public static QuizQuestion WhichCisco(CommandEntry command, Random random)
    {
        var options = Shuffle(CiscoDistractors(command, random).Select(c => c.Text)
            .Distinct()
            .Take(OptionCount - 1)
            .Append(command.Text), random);
        return new QuizQuestion(
            QuestionKind.WhichCommand,
            command.Description,
            null,
            options,
            IndexOf(options, command.Text),
            $"{command.Text}, typed in {CiscoLibrary.ModeName(command.Prompt)} mode.",
            command.Prompt);
    }

    private static IEnumerable<CommandEntry> CiscoDistractors(CommandEntry command, Random random) =>
        Shuffle(CiscoLibrary.Essentials.Where(c => c.Prompt == command.Prompt), random)
            .Concat(Shuffle(CiscoLibrary.Essentials, random))
            .Where(c => c != command);

    /// <summary>Shown a LINQ statement, pick what it does.</summary>
    public static QuizQuestion FromLinq(CommandEntry command, Random random)
    {
        var group = LinqLibrary.GroupOf(command);
        var options = Shuffle(LinqDistractors(command, group, random).Select(c => c.Description)
            .Where(d => d != command.Description)
            .Distinct()
            .Take(OptionCount - 1)
            .Append(command.Description), random);
        return new QuizQuestion(
            QuestionKind.WhatDoesItDo,
            command.Text,
            null,
            options,
            IndexOf(options, command.Description),
            $"{command.Text} uses one of the {group} operators.",
            command.Prompt,
            group);
    }

    /// <summary>Shown what a LINQ statement does, pick the statement.</summary>
    public static QuizQuestion WhichLinq(CommandEntry command, Random random)
    {
        var group = LinqLibrary.GroupOf(command);
        var options = Shuffle(LinqDistractors(command, group, random).Select(c => c.Text)
            .Distinct()
            .Take(OptionCount - 1)
            .Append(command.Text), random);
        return new QuizQuestion(
            QuestionKind.WhichCommand,
            command.Description,
            null,
            options,
            IndexOf(options, command.Text),
            $"{command.Text} is one of the {group} operators.",
            command.Prompt,
            group);
    }

    // Statements from the same group, like OrderBy and ThenBy, are the easiest to mix up.
    private static IEnumerable<CommandEntry> LinqDistractors(CommandEntry command, string group, Random random) =>
        Shuffle(LinqLibrary.Essentials.Where(c => LinqLibrary.GroupOf(c) == group), random)
            .Concat(Shuffle(LinqLibrary.Essentials, random))
            .Where(c => c != command);

    /// <summary>Shown a Claude Code command, shortcut or concept, pick what it does.</summary>
    public static QuizQuestion FromClaude(ClaudeCommand command, Random random)
    {
        var options = Shuffle(ClaudeDistractors(command, random).Select(c => c.Description)
            .Take(OptionCount - 1)
            .Append(command.Description), random);
        return new QuizQuestion(
            QuestionKind.WhatDoesItDo,
            command.Name,
            null,
            options,
            IndexOf(options, command.Description),
            ClaudeExplanation(command, $"{command.Key} is one of the {ClaudeKinds.Label(command.Kind)}."),
            command.Prompt,
            ClaudeKinds.Label(command.Kind));
    }

    /// <summary>Shown what a Claude Code command does, pick the command. The prompt hints at its kind.</summary>
    public static QuizQuestion WhichClaude(ClaudeCommand command, Random random)
    {
        var options = Shuffle(ClaudeDistractors(command, random).Select(c => c.Key)
            .Take(OptionCount - 1)
            .Append(command.Key), random);
        return new QuizQuestion(
            QuestionKind.WhichCommand,
            command.Description,
            null,
            options,
            IndexOf(options, command.Key),
            ClaudeExplanation(command, $"{command.Name}, one of the {ClaudeKinds.Label(command.Kind)}."),
            command.Prompt,
            ClaudeKinds.Label(command.Kind));
    }

    /// <summary>Shown a real situation, pick the command that solves it.</summary>
    public static QuizQuestion FromScenario(ClaudeScenario scenario, Random random)
    {
        var command = ClaudeLibrary.Find(scenario.Answer)
            ?? throw new InvalidOperationException($"No Claude Code command {scenario.Answer}.");
        var options = Shuffle(ClaudeDistractors(command, random).Select(c => c.Key)
            .Take(OptionCount - 1)
            .Append(command.Key), random);
        return new QuizQuestion(
            QuestionKind.Scenario,
            scenario.Situation,
            null,
            options,
            IndexOf(options, command.Key),
            $"{command.Key}: {scenario.Why}",
            ClaudeCommand.ClaudePrompt,
            ClaudeKinds.Label(command.Kind));
    }

    /// <summary>Shown what a Claude Code command does, type the command.</summary>
    public static QuizQuestion TypeClaude(ClaudeCommand command)
    {
        var accepted = ClaudeAnswers.Accepted(command);
        var also = accepted.Count > 1 ? $" Accepted: {string.Join(", ", accepted)}." : "";
        return new QuizQuestion(
            QuestionKind.TypeIt,
            command.Description,
            null,
            [command.Key],
            0,
            ClaudeExplanation(command, $"{command.Name}, one of the {ClaudeKinds.Label(command.Kind)}.") + also,
            command.Prompt,
            ClaudeKinds.Label(command.Kind),
            accepted);
    }

    /// <summary>Whether a typed answer is right; Claude Code commands also accept aliases and placeholders.</summary>
    public static bool IsTypedCorrect(string typed, QuizQuestion question) =>
        ClaudeLibrary.Find(question.Answer) is { } command
            ? ClaudeAnswers.IsCorrect(typed, command)
            : question.Accepted?.Contains(ClaudeAnswers.Normalize(typed)) == true;

    private static string ClaudeExplanation(ClaudeCommand command, string text) =>
        command.Note is { } note ? $"{text} {note}." : text;

    // The same kind is the easiest to mix up: slash commands with slash commands, keys with keys.
    private static IEnumerable<ClaudeCommand> ClaudeDistractors(ClaudeCommand command, Random random) =>
        Shuffle(ClaudeLibrary.All.Where(c => c.Kind == command.Kind), random)
            .Concat(Shuffle(ClaudeLibrary.All, random))
            .Where(c => c != command)
            .Distinct();

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
