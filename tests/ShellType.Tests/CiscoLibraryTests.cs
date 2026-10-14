using ShellType.Data;
using ShellType.Models;

namespace ShellType.Tests;

public class CiscoLibraryTests
{
    [Fact]
    public void There_are_exactly_50_unique_essentials()
    {
        Assert.Equal(50, CiscoLibrary.Essentials.Count);
        Assert.Equal(50, CiscoLibrary.Essentials.Select(c => c.Text).Distinct().Count());
    }

    [Fact]
    public void Commands_and_steps_are_typeable_with_an_ios_prompt()
    {
        var all = CiscoLibrary.Essentials.Concat(CiscoLibrary.Operations.SelectMany(o => o.Steps));
        Assert.All(all, c =>
        {
            Assert.All(c.Text, ch => Assert.InRange(ch, ' ', '~'));
            Assert.Equal(c.Text.Trim(), c.Text);
            Assert.DoesNotContain("  ", c.Text);
            Assert.False(string.IsNullOrWhiteSpace(c.Description));
            Assert.Matches(@"^[A-Za-z0-9]+(\([a-z-]+\))?[>#]$", c.Prompt);
        });
    }

    [Fact]
    public void Filter_keeps_only_commands_up_to_the_difficulty()
    {
        var easy = CiscoLibrary.Filter(Difficulty.Easy);
        Assert.NotEmpty(easy);
        Assert.All(easy, c => Assert.Equal(Difficulty.Easy, c.Difficulty));
        Assert.Equal(50, CiscoLibrary.Filter(Difficulty.Hard).Count);
    }

    [Fact]
    public void Every_length_has_several_cisco_operations()
    {
        foreach (var length in Enum.GetValues<OperationLength>())
        {
            Assert.True(CiscoLibrary.Operations.Count(o => o.Length == length) >= 3, $"too few {length} operations");
        }
    }

    [Fact]
    public void Cisco_operation_names_are_unique()
    {
        var names = CiscoLibrary.Operations.Select(o => o.Name).ToList();
        Assert.Equal(names.Count, names.Distinct().Count());
        Assert.All(CiscoLibrary.Operations, o => Assert.InRange(o.Steps.Count, 2, 10));
    }
}
