using ShellType.Data;
using ShellType.Models;

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

}
