using ShellType.Data;
using ShellType.Models;

namespace ShellType.Tests;

public class CommandLibraryTests
{
    [Fact]
    public void Every_category_has_commands()
    {
        foreach (var category in Enum.GetValues<CommandCategory>())
        {
            Assert.NotEmpty(CommandLibrary.ByCategory(category));
        }
    }

    [Fact]
    public void Command_texts_are_unique()
    {
        var duplicates = CommandLibrary.All
            .GroupBy(c => c.Text)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        Assert.Empty(duplicates);
    }

    [Fact]
    public void Commands_are_typeable_ascii_without_surrounding_whitespace()
    {
        foreach (var command in CommandLibrary.All)
        {
            Assert.All(command.Text, ch => Assert.InRange(ch, ' ', '~'));
            Assert.Equal(command.Text.Trim(), command.Text);
            Assert.DoesNotContain("  ", command.Text);
        }
    }

    [Fact]
    public void Every_command_has_a_description()
    {
        Assert.All(CommandLibrary.All, c => Assert.False(string.IsNullOrWhiteSpace(c.Description)));
    }

    [Fact]
    public void Filter_respects_difficulty_and_categories()
    {
        var result = CommandLibrary.Filter([CommandCategory.Git], Difficulty.Easy);

        Assert.NotEmpty(result);
        Assert.All(result, c =>
        {
            Assert.Equal(CommandCategory.Git, c.Category);
            Assert.Equal(Difficulty.Easy, c.Difficulty);
        });
    }

    [Fact]
    public void Filter_with_no_categories_returns_all_of_that_difficulty()
    {
        var result = CommandLibrary.Filter([], Difficulty.Hard);

        Assert.Equal(CommandLibrary.All.Count, result.Count);
    }

    [Theory]
    [InlineData("grep -rn TODO src/", "grep")]
    [InlineData("sudo apt update", "apt")]
    [InlineData("pwd", "pwd")]
    public void Program_is_first_word_ignoring_sudo(string text, string expected)
    {
        var entry = new CommandEntry(text, "d", CommandCategory.System, Difficulty.Easy);

        Assert.Equal(expected, entry.Program);
    }
}
