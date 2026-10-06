using ShellType.Game;
using ShellType.Models;

namespace ShellType.Tests;

public class CommandAttemptTests
{
    private static CommandAttempt Attempt(string text = "ls -la") =>
        new(new CommandEntry(text, "d", CommandCategory.Navigation, Difficulty.Easy));

    private static void TypeAll(CommandAttempt attempt, string text)
    {
        foreach (var c in text)
        {
            attempt.Append(c, TimeSpan.Zero);
        }
    }

    [Fact]
    public void Untyped_chars_are_pending()
    {
        var attempt = Attempt();

        Assert.All(Enumerable.Range(0, attempt.Target.Length), i => Assert.Equal(CharState.Pending, attempt.StateAt(i)));
    }

    [Fact]
    public void Tracks_correct_incorrect_and_extra_chars()
    {
        var attempt = Attempt("ls");
        TypeAll(attempt, "lxyz");

        Assert.Equal(CharState.Correct, attempt.StateAt(0));
        Assert.Equal(CharState.Incorrect, attempt.StateAt(1));
        Assert.Equal(CharState.Extra, attempt.StateAt(2));
        Assert.Equal(1, attempt.CorrectChars);
        Assert.Equal(1, attempt.IncorrectChars);
        Assert.Equal(2, attempt.ExtraChars);
        Assert.Equal(4, attempt.DisplayLength);
        Assert.Equal('y', attempt.DisplayCharAt(2));
    }

    [Fact]
    public void Mistakes_are_remembered_after_backspace()
    {
        var attempt = Attempt("ls");
        TypeAll(attempt, "lx");
        attempt.Backspace();
        TypeAll(attempt, "s");

        Assert.True(attempt.IsCorrect);
        Assert.Equal(1, attempt.Mistakes);
    }

    [Fact]
    public void Missed_chars_only_count_once_submitted()
    {
        var attempt = Attempt("ls -la");
        TypeAll(attempt, "ls");

        Assert.Equal(0, attempt.MissedChars);
        attempt.Submit(TimeSpan.FromSeconds(1));
        Assert.Equal(4, attempt.MissedChars);
    }

    [Fact]
    public void Extra_chars_are_capped()
    {
        var attempt = Attempt("ls");
        TypeAll(attempt, new string('x', 50));

        Assert.Equal(2 + CommandAttempt.MaxExtra, attempt.TypedLength);
    }

    [Fact]
    public void Backspace_on_empty_is_a_no_op()
    {
        var attempt = Attempt();
        attempt.Backspace();

        Assert.Equal(0, attempt.TypedLength);
    }

    [Fact]
    public void Wpm_is_measured_from_first_key_to_submit()
    {
        // "pwd" + Enter = 4 chars = 0.8 words, typed in 6 seconds = 8 WPM
        var attempt = Attempt("pwd");
        attempt.Append('p', TimeSpan.FromSeconds(10));
        attempt.Append('w', TimeSpan.FromSeconds(12));
        attempt.Append('d', TimeSpan.FromSeconds(14));
        attempt.Submit(TimeSpan.FromSeconds(16));

        Assert.Equal(8, attempt.Wpm!.Value, precision: 6);
    }

    [Fact]
    public void Wpm_is_null_for_incorrect_commands()
    {
        var attempt = Attempt("pwd");
        TypeAll(attempt, "pwx");
        attempt.Submit(TimeSpan.FromSeconds(3));

        Assert.Null(attempt.Wpm);
    }

    [Fact]
    public void IsOnTrack_detects_divergence()
    {
        var attempt = Attempt("git status");
        TypeAll(attempt, "git s");
        Assert.True(attempt.IsOnTrack);

        TypeAll(attempt, "x");
        Assert.False(attempt.IsOnTrack);
    }
}
