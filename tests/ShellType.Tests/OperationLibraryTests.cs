using ShellType.Data;
using ShellType.Models;

namespace ShellType.Tests;

public class OperationLibraryTests
{
    [Fact]
    public void Every_length_has_several_operations()
    {
        foreach (var length in Enum.GetValues<OperationLength>())
        {
            Assert.True(OperationLibrary.ByLength(length).Count >= 3, $"too few {length} operations");
        }
    }

    [Fact]
    public void Operations_have_between_3_and_10_steps()
    {
        Assert.All(OperationLibrary.All, o => Assert.InRange(o.Steps.Count, 3, 10));
    }

    [Fact]
    public void Operation_names_are_unique()
    {
        Assert.Equal(OperationLibrary.All.Count, OperationLibrary.All.Select(o => o.Name).Distinct().Count());
    }

    [Fact]
    public void Steps_are_typeable_ascii_with_descriptions()
    {
        foreach (var step in OperationLibrary.All.SelectMany(o => o.Steps))
        {
            Assert.All(step.Text, ch => Assert.InRange(ch, ' ', '~'));
            Assert.Equal(step.Text.Trim(), step.Text);
            Assert.DoesNotContain("  ", step.Text);
            Assert.False(string.IsNullOrWhiteSpace(step.Description));
        }
    }

    [Theory]
    [InlineData(3, OperationLength.Short)]
    [InlineData(4, OperationLength.Short)]
    [InlineData(5, OperationLength.Medium)]
    [InlineData(7, OperationLength.Medium)]
    [InlineData(8, OperationLength.Long)]
    [InlineData(10, OperationLength.Long)]
    public void Length_follows_step_count(int steps, OperationLength expected)
    {
        var entries = Enumerable.Range(0, steps)
            .Select(i => new CommandEntry($"cmd{i}", "d", CommandCategory.System, Difficulty.Easy))
            .ToList();

        Assert.Equal(expected, new Operation("op", "goal", entries).Length);
    }
}
