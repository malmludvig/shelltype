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
        var easy = CiscoLibrary.Filter([], Difficulty.Easy);
        Assert.NotEmpty(easy);
        Assert.All(easy, c => Assert.Equal(Difficulty.Easy, c.Difficulty));
        Assert.Equal(76, CiscoLibrary.Filter([], Difficulty.Hard).Count);
    }

    [Fact]
    public void Every_essential_is_practiced_and_practice_commands_are_valid()
    {
        var all = CiscoLibrary.Filter([], Difficulty.Hard);
        Assert.Equal(all.Count, all.Select(c => c.Text).Distinct().Count());
        Assert.Equal(all.Count, all.Select(c => c.Description).Distinct().Count());
        Assert.All(CiscoLibrary.Essentials, c => Assert.Contains(c, all));
        Assert.All(all, c =>
        {
            Assert.All(c.Text, ch => Assert.InRange(ch, ' ', '~'));
            Assert.DoesNotContain("  ", c.Text);
            Assert.Matches(@"^[A-Za-z0-9]+(\([a-z-]+\))?[>#]$", c.Prompt);
        });
    }

    [Fact]
    public void Every_category_has_commands_at_every_difficulty()
    {
        foreach (var category in Enum.GetValues<CiscoCategory>())
        {
            var commands = CiscoLibrary.Filter([category], Difficulty.Hard);
            Assert.All(commands, c => Assert.Equal(category, CiscoLibrary.CategoryOf(c)));
            foreach (var difficulty in Enum.GetValues<Difficulty>())
            {
                Assert.Contains(commands, c => c.Difficulty == difficulty);
            }

            // Easy tests should not just repeat one command.
            Assert.True(commands.Count(c => c.Difficulty == Difficulty.Easy) >= 3, $"too few easy {category} commands");
        }

        Assert.Equal(CiscoCategory.Vlan, CiscoLibrary.CategoryOf(CiscoLibrary.Essentials.Single(c => c.Text == "vlan 10")));
        Assert.Equal(CiscoCategory.Routing, CiscoLibrary.CategoryOf(CiscoLibrary.Essentials.Single(c => c.Text == "show ip route")));
        Assert.Equal("acl, nat & dhcp", CiscoLibrary.Label(CiscoCategory.Services));
    }

    [Fact]
    public void Category_filter_respects_the_difficulty()
    {
        var easyVlan = CiscoLibrary.Filter([CiscoCategory.Vlan], Difficulty.Easy);
        Assert.NotEmpty(easyVlan);
        Assert.All(easyVlan, c => Assert.Equal(Difficulty.Easy, c.Difficulty));
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
