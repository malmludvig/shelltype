using ShellType.Data;
using ShellType.Game;
using ShellType.Models;
using ShellType.Quiz;

namespace ShellType.Tests;

public class LinqLibraryTests
{
    private static IEnumerable<CommandEntry> Everything =>
        LinqLibrary.Essentials.Concat(LinqLibrary.Operations.SelectMany(o => o.Steps));

    [Fact]
    public void Statements_and_descriptions_are_unique()
    {
        var count = LinqLibrary.Essentials.Count;
        Assert.True(count >= 50, $"only {count} statements");
        Assert.Equal(count, LinqLibrary.Essentials.Select(c => c.Text).Distinct().Count());
        Assert.Equal(count, LinqLibrary.Essentials.Select(c => c.Description).Distinct().Count());
    }

    [Fact]
    public void Statements_and_steps_are_typeable_at_the_csharp_prompt()
    {
        Assert.All(Everything, c =>
        {
            Assert.All(c.Text, ch => Assert.InRange(ch, ' ', '~'));
            Assert.Equal(c.Text.Trim(), c.Text);
            Assert.DoesNotContain("  ", c.Text);
            Assert.EndsWith(";", c.Text);
            Assert.False(string.IsNullOrWhiteSpace(c.Description));
            Assert.Equal(LinqLibrary.Prompt, c.Prompt);
        });
    }

    [Theory]
    [InlineData("int[] numbers =")]
    [InlineData("int[] other =")]
    [InlineData("string[] words =")]
    [InlineData("string[] lines =")]
    [InlineData("object[] items =")]
    [InlineData("List<Person> people =")]
    [InlineData("List<Order> orders =")]
    public void The_data_shows_every_collection_in_full(string declaration)
    {
        Assert.Contains(declaration, LinqLibrary.Data);
    }

    [Fact]
    public void Every_difficulty_has_statements()
    {
        foreach (var difficulty in Enum.GetValues<Difficulty>())
        {
            Assert.Contains(LinqLibrary.Essentials, c => c.Difficulty == difficulty);
            Assert.All(LinqLibrary.Filter(difficulty), c => Assert.True(c.Difficulty <= difficulty));
        }

        Assert.Equal(LinqLibrary.Essentials.Count, LinqLibrary.Filter(Difficulty.Hard).Count);
    }

    [Fact]
    public void Every_statement_has_a_group()
    {
        Assert.All(LinqLibrary.Essentials, c => Assert.NotEqual("linq", LinqLibrary.GroupOf(c)));
        Assert.Equal("filtering", LinqLibrary.GroupOf(LinqLibrary.Essentials[0]));
    }

    [Fact]
    public void Every_length_has_several_linq_operations()
    {
        foreach (var length in Enum.GetValues<OperationLength>())
        {
            Assert.True(LinqLibrary.Operations.Count(o => o.Length == length) >= 3, $"too few {length} operations");
        }

        var names = LinqLibrary.Operations.Select(o => o.Name).ToList();
        Assert.Equal(names.Count, names.Distinct().Count());
    }

    [Fact]
    public void Linq_quiz_asks_both_ways_round_with_a_topic()
    {
        var questions = QuizGenerator.Create(QuizMode.Linq, 20, new Random(5));

        Assert.Equal(10, questions.Count(q => q.Kind == QuestionKind.WhatDoesItDo));
        Assert.Equal(10, questions.Count(q => q.Kind == QuestionKind.WhichCommand));
        Assert.All(questions, q =>
        {
            var command = q.Kind == QuestionKind.WhichCommand
                ? LinqLibrary.Essentials.Single(c => c.Description == q.Prompt)
                : LinqLibrary.Essentials.Single(c => c.Text == q.Prompt);
            Assert.Equal(LinqLibrary.Prompt, q.Shell);
            Assert.Equal(LinqLibrary.GroupOf(command), q.Topic);
            Assert.Equal(q.Kind == QuestionKind.WhichCommand ? command.Text : command.Description, q.Answer);
        });
    }

    [Fact]
    public void Linq_tests_get_their_own_label()
    {
        Assert.Equal("linq time 30", TestModes.Label(TestMode.Time, 30, Platform.Linq));
    }
}
