using ShellType.Data;
using ShellType.Game;
using ShellType.Models;
using ShellType.Quiz;

namespace ShellType.Tests;

public class GitLibraryTests
{
    private static IEnumerable<CommandEntry> Everything =>
        GitLibrary.Essentials.Concat(GitLibrary.Operations.SelectMany(o => o.Steps));

    [Fact]
    public void Commands_and_descriptions_are_unique()
    {
        var count = GitLibrary.Essentials.Count;
        Assert.True(count >= 80, $"only {count} commands");
        Assert.Equal(count, GitLibrary.Essentials.Select(c => c.Text).Distinct().Count());
        Assert.Equal(count, GitLibrary.Essentials.Select(c => c.Description).Distinct().Count());
    }

    [Fact]
    public void Commands_and_steps_are_typeable_git_commands()
    {
        Assert.All(Everything, c =>
        {
            Assert.All(c.Text, ch => Assert.InRange(ch, ' ', '~'));
            Assert.Equal(c.Text.Trim(), c.Text);
            Assert.DoesNotContain("  ", c.Text);
            Assert.StartsWith("git ", c.Text);
            Assert.Equal("git", c.Program);
            Assert.False(string.IsNullOrWhiteSpace(c.Description));
            Assert.Equal(GitLibrary.Prompt, c.Prompt);
            Assert.Equal(CommandCategory.Git, c.Category);
        });
    }

    [Fact]
    public void Every_difficulty_has_commands()
    {
        foreach (var difficulty in Enum.GetValues<Difficulty>())
        {
            Assert.Contains(GitLibrary.Essentials, c => c.Difficulty == difficulty);
            Assert.All(GitLibrary.Filter(difficulty), c => Assert.True(c.Difficulty <= difficulty));
        }

        Assert.Equal(GitLibrary.Essentials.Count, GitLibrary.Filter(Difficulty.Hard).Count);
    }

    [Fact]
    public void Every_command_has_a_group()
    {
        Assert.All(GitLibrary.Essentials, c => Assert.NotEqual("git", GitLibrary.GroupOf(c)));
        Assert.Equal("setup", GitLibrary.GroupOf(GitLibrary.Essentials[0]));
    }

    [Fact]
    public void Every_length_has_several_git_operations()
    {
        foreach (var length in Enum.GetValues<OperationLength>())
        {
            Assert.True(GitLibrary.Operations.Count(o => o.Length == length) >= 3, $"too few {length} operations");
        }

        var names = GitLibrary.Operations.Select(o => o.Name).ToList();
        Assert.Equal(names.Count, names.Distinct().Count());
    }

    [Fact]
    public void Git_quiz_asks_both_ways_round_with_a_topic()
    {
        var questions = QuizGenerator.Create(QuizMode.Git, 20, new Random(5));

        Assert.Equal(10, questions.Count(q => q.Kind == QuestionKind.WhatDoesItDo));
        Assert.Equal(10, questions.Count(q => q.Kind == QuestionKind.WhichCommand));
        Assert.All(questions, q =>
        {
            var command = q.Kind == QuestionKind.WhichCommand
                ? GitLibrary.Essentials.Single(c => c.Description == q.Prompt)
                : GitLibrary.Essentials.Single(c => c.Text == q.Prompt);
            Assert.Equal(GitLibrary.Prompt, q.Shell);
            Assert.Equal(GitLibrary.GroupOf(command), q.Topic);
            Assert.Equal(q.Kind == QuestionKind.WhichCommand ? command.Text : command.Description, q.Answer);
        });
    }

    [Fact]
    public void Git_tests_get_their_own_label()
    {
        Assert.Equal("git time 30", TestModes.Label(TestMode.Time, 30, Platform.Git));
    }
}
