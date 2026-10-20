using ShellType.Data;
using ShellType.Models;

namespace ShellType.Tests;

public class ClaudeLibraryTests
{
    [Fact]
    public void Every_kind_has_entries_and_keys_are_unique()
    {
        Assert.All(Enum.GetValues<ClaudeKind>(), kind => Assert.Contains(ClaudeLibrary.All, c => c.Kind == kind));
        Assert.Equal(ClaudeLibrary.All.Count, ClaudeLibrary.All.Select(c => c.Key).Distinct().Count());
        Assert.Equal(ClaudeLibrary.All.Count, ClaudeLibrary.All.Select(c => c.Description).Distinct().Count());
    }

    [Fact]
    public void Entries_are_described_and_printable()
    {
        Assert.All(ClaudeLibrary.All, c =>
        {
            Assert.False(string.IsNullOrWhiteSpace(c.Description));
            Assert.All(c.Name, ch => Assert.InRange(ch, ' ', '~'));
            Assert.Equal(c.Name.Trim(), c.Name);
        });
    }

    [Theory]
    [InlineData("/compact [instructions]", "/compact")]
    [InlineData("/loop [interval] [prompt]", "/loop")]
    [InlineData("--worktree, -w [name]", "--worktree, -w")]
    [InlineData("claude attach <id>", "claude attach")]
    [InlineData("claude -p \"query\"", "claude -p \"query\"")]
    public void Key_drops_the_placeholders(string name, string key)
    {
        Assert.Equal(key, ClaudeLibrary.All.Single(c => c.Name == name).Key);
    }

    [Fact]
    public void Commands_and_steps_are_typeable_at_their_prompt()
    {
        var all = ClaudeLibrary.Essentials.Concat(ClaudeLibrary.Operations.SelectMany(o => o.Steps));
        Assert.All(all, c =>
        {
            Assert.All(c.Text, ch => Assert.InRange(ch, ' ', '~'));
            Assert.Equal(c.Text.Trim(), c.Text);
            Assert.DoesNotContain("  ", c.Text);
            Assert.False(string.IsNullOrWhiteSpace(c.Description));
            Assert.Contains(c.Prompt, new[] { ClaudeCommand.ClaudePrompt, CommandEntry.ShellPrompt });
        });
    }

    [Fact]
    public void Typing_list_is_unique_and_leaves_out_keys_and_concepts()
    {
        var typed = ClaudeLibrary.Essentials;
        Assert.Equal(typed.Count, typed.Select(c => c.Text).Distinct().Count());
        Assert.DoesNotContain(typed, c => c.Text == "Shift+Tab" || c.Text == "CLAUDE.md");
        Assert.Contains(typed, c => c.Text == "/compact keep only the database decisions" && c.Prompt == ClaudeCommand.ClaudePrompt);
        Assert.Contains(typed, c => c.Text == "claude --model sonnet" && c.Prompt == CommandEntry.ShellPrompt);
    }

    [Fact]
    public void Flags_and_modes_all_have_an_example_to_type()
    {
        var needExamples = ClaudeLibrary.All.Where(c => c.Kind is ClaudeKind.Flag or ClaudeKind.Mode);
        Assert.All(needExamples, c => Assert.StartsWith("claude ", c.Example));
    }

    [Fact]
    public void Filter_keeps_only_commands_up_to_the_difficulty()
    {
        var easy = ClaudeLibrary.Filter(Difficulty.Easy);

        Assert.NotEmpty(easy);
        Assert.All(easy, c => Assert.Equal(Difficulty.Easy, c.Difficulty));
        Assert.Contains(ClaudeLibrary.Essentials, c => c.Difficulty == Difficulty.Hard);
        Assert.Equal(ClaudeLibrary.Essentials.Count, ClaudeLibrary.Filter(Difficulty.Hard).Count);
    }

    [Fact]
    public void Every_operation_length_has_at_least_three_operations()
    {
        foreach (var length in Enum.GetValues<OperationLength>())
        {
            Assert.True(ClaudeLibrary.Operations.Count(o => o.Length == length) >= 3, $"too few {length} operations");
        }
    }

    [Fact]
    public void Operation_names_are_unique()
    {
        var names = ClaudeLibrary.Operations.Select(o => o.Name).ToList();
        Assert.Equal(names.Count, names.Distinct().Count());
        Assert.All(ClaudeLibrary.Operations, o => Assert.InRange(o.Steps.Count, 2, 10));
    }

    [Fact]
    public void Every_scenario_answers_with_a_real_entry()
    {
        Assert.Equal(24, ClaudeLibrary.Scenarios.Count);
        Assert.All(ClaudeLibrary.Scenarios, s =>
        {
            Assert.NotNull(ClaudeLibrary.Find(s.Answer));
            Assert.False(string.IsNullOrWhiteSpace(s.Why));
        });
        Assert.Equal(24, ClaudeLibrary.Scenarios.Select(s => s.Situation).Distinct().Count());
    }

    [Fact]
    public void Aliases_and_notes_are_parsed()
    {
        var clear = ClaudeLibrary.Find("/clear")!;
        Assert.Equal(["/reset", "/new"], clear.Aliases);
        Assert.Equal("Alias: /reset, /new", clear.Note);
        Assert.Equal("macOS: Option+P", ClaudeLibrary.Find("Alt+P")!.Note);
    }
}
