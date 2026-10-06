using ShellType.Game;

namespace ShellType.Tests;

public class StatsCalculatorTests
{
    [Fact]
    public void Wpm_uses_five_chars_per_word()
    {
        // 50 chars = 10 words in 30 seconds = 20 WPM
        Assert.Equal(20, StatsCalculator.Wpm(50, TimeSpan.FromSeconds(30)), precision: 6);
    }

    [Fact]
    public void Wpm_is_zero_for_zero_duration()
    {
        Assert.Equal(0, StatsCalculator.Wpm(50, TimeSpan.Zero));
    }

    [Theory]
    [InlineData(0, 0, 100)]
    [InlineData(9, 10, 90)]
    [InlineData(0, 4, 0)]
    public void Accuracy_is_a_percentage(int correct, int total, double expected)
    {
        Assert.Equal(expected, StatsCalculator.Accuracy(correct, total), precision: 6);
    }

    [Fact]
    public void Perfectly_steady_typing_is_fully_consistent()
    {
        Assert.Equal(100, StatsCalculator.Consistency([60, 60, 60, 60]), precision: 6);
    }

    [Fact]
    public void Erratic_typing_is_less_consistent_than_steady_typing()
    {
        var steady = StatsCalculator.Consistency([58, 62, 60, 61]);
        var erratic = StatsCalculator.Consistency([10, 120, 5, 90]);

        Assert.True(steady > 90);
        Assert.True(erratic < steady);
        Assert.InRange(erratic, 0, 100);
    }

    [Fact]
    public void Samples_bucket_keystrokes_by_second()
    {
        Keystroke[] keys =
        [
            new(TimeSpan.FromSeconds(0.2), true),
            new(TimeSpan.FromSeconds(0.6), false),
            new(TimeSpan.FromSeconds(1.5), true),
        ];

        var samples = StatsCalculator.Samples(keys, TimeSpan.FromSeconds(2));

        Assert.Equal(2, samples.Count);
        Assert.Equal(1, samples[0].Errors);
        Assert.Equal(0, samples[1].Errors);
        Assert.Equal(StatsCalculator.Wpm(2, TimeSpan.FromSeconds(1)), samples[0].Raw, precision: 6);
        Assert.Equal(StatsCalculator.Wpm(2, TimeSpan.FromSeconds(2)), samples[1].Wpm, precision: 6);
    }

    [Fact]
    public void Samples_drop_a_tiny_trailing_second()
    {
        var samples = StatsCalculator.Samples([new(TimeSpan.FromSeconds(0.5), true)], TimeSpan.FromSeconds(2.1));

        Assert.Equal(2, samples.Count);
    }

    [Fact]
    public void Compute_counts_only_correct_commands_toward_wpm()
    {
        var clock = new ManualTimeProvider();
        var session = new TypingSession(
            new TestConfig { Mode = TestMode.Commands, CommandCount = 2 },
            new FixedCommandSource("ls", "pwd"),
            clock);

        foreach (var c in "lx") { session.Type(c); }
        session.Submit();
        clock.Advance(6);
        foreach (var c in "pwd") { session.Type(c); }

        var result = StatsCalculator.Compute(session);

        // Only "pwd" + Enter (4 chars) counts: 0.8 words in 0.1 minutes = 8 WPM
        Assert.Equal(8, result.Wpm, precision: 6);
        Assert.Equal(1, result.IncorrectChars);
        Assert.Equal(0, result.MissedChars);
        Assert.Equal(2, result.Commands.Count);
        Assert.False(result.Commands[0].Correct);
        Assert.True(result.Commands[1].Correct);
        Assert.Equal("commands 2", result.ModeLabel);
    }

    [Fact]
    public void LiveWpm_is_zero_before_the_test_starts()
    {
        var session = new TypingSession(new TestConfig(), new FixedCommandSource("ls"), new ManualTimeProvider());

        Assert.Equal(0, StatsCalculator.LiveWpm(session));
    }

    [Fact]
    public void LiveWpm_tracks_progress_while_running()
    {
        var clock = new ManualTimeProvider();
        var session = new TypingSession(new TestConfig(), new FixedCommandSource("ls -la"), clock);
        foreach (var c in "ls -la") { session.Type(c); }
        clock.Advance(6);

        // 6 clean chars in 6 seconds = 1.2 words in 0.1 min = 12 WPM
        Assert.Equal(12, StatsCalculator.LiveWpm(session), precision: 6);
    }

    [Fact]
    public void Compute_includes_a_clean_partial_command_in_timed_tests()
    {
        var clock = new ManualTimeProvider();
        var session = new TypingSession(new TestConfig { TimeSeconds = 15 }, new FixedCommandSource("git status"), clock);

        foreach (var c in "git st") { session.Type(c); }
        clock.Advance(15);
        session.Tick();

        var result = StatsCalculator.Compute(session);

        Assert.Equal(StatsCalculator.Wpm(6, TimeSpan.FromSeconds(15)), result.Wpm, precision: 6);
        Assert.Equal(0, result.MissedChars);
        Assert.Equal(100, result.Accuracy);
        Assert.False(Assert.Single(result.Commands).Finished);
    }
}
