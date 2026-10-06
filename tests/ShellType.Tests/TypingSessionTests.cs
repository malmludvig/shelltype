using ShellType.Game;

namespace ShellType.Tests;

public class TypingSessionTests
{
    private readonly ManualTimeProvider _clock = new();

    private TypingSession Session(TestConfig config, params string[] commands) =>
        new(config, new FixedCommandSource(commands), _clock);

    private static void TypeAll(TypingSession session, string text)
    {
        foreach (var c in text)
        {
            session.Type(c);
        }
    }

    [Fact]
    public void Starts_on_first_keystroke()
    {
        var session = Session(new TestConfig(), "ls");
        Assert.Equal(SessionState.Ready, session.State);

        session.Type('l');

        Assert.Equal(SessionState.Running, session.State);
    }

    [Fact]
    public void Commands_mode_prepares_exact_number_of_commands()
    {
        var session = Session(new TestConfig { Mode = TestMode.Commands, CommandCount = 5 }, "ls");

        Assert.Equal(5, session.Attempts.Count);
    }

    [Fact]
    public void Submit_moves_to_next_command()
    {
        var session = Session(new TestConfig(), "ls", "pwd");
        TypeAll(session, "ls");
        session.Submit();

        Assert.Equal(1, session.CurrentIndex);
        Assert.Equal("pwd", session.Current.Target);
        Assert.True(session.Attempts[0].IsSubmitted);
    }

    [Fact]
    public void Submit_with_empty_input_is_ignored()
    {
        var session = Session(new TestConfig(), "ls", "pwd");
        TypeAll(session, "l");
        session.Backspace();
        session.Submit();

        Assert.Equal(0, session.CurrentIndex);
    }

    [Fact]
    public void Time_mode_keeps_a_lookahead_buffer()
    {
        var session = Session(new TestConfig(), "a", "b", "c");
        for (var i = 0; i < 20; i++)
        {
            TypeAll(session, session.Current.Target);
            session.Submit();
        }

        Assert.True(session.Upcoming.Count() >= 4);
    }

    [Fact]
    public void Commands_mode_finishes_when_last_command_is_typed_correctly()
    {
        var session = Session(new TestConfig { Mode = TestMode.Commands, CommandCount = 2 }, "ls", "pwd");
        TypeAll(session, "ls");
        session.Submit();
        _clock.Advance(3);
        TypeAll(session, "pwd");

        Assert.Equal(SessionState.Finished, session.State);
        Assert.Equal(TimeSpan.FromSeconds(3), session.Elapsed);
        Assert.Equal(2, session.Completed.Count());
    }

    [Fact]
    public void Commands_mode_finishes_when_last_command_is_submitted_wrong()
    {
        var session = Session(new TestConfig { Mode = TestMode.Commands, CommandCount = 1 }, "pwd");
        TypeAll(session, "pwx");
        Assert.Equal(SessionState.Running, session.State);

        session.Submit();

        Assert.Equal(SessionState.Finished, session.State);
    }

    [Fact]
    public void Finished_event_fires_once()
    {
        var session = Session(new TestConfig { Mode = TestMode.Commands, CommandCount = 1 }, "ls");
        var count = 0;
        session.Finished += () => count++;

        TypeAll(session, "ls");
        session.Finish();

        Assert.Equal(1, count);
    }

    [Fact]
    public void Input_is_ignored_after_finish()
    {
        var session = Session(new TestConfig { Mode = TestMode.Commands, CommandCount = 1 }, "ls");
        TypeAll(session, "ls");

        Assert.False(session.Type('x'));
        Assert.Equal("ls", session.Current.Typed);
    }

    [Fact]
    public void Records_every_keystroke_including_enter()
    {
        var session = Session(new TestConfig(), "ls", "pwd");
        TypeAll(session, "lx");
        session.Backspace();
        TypeAll(session, "s");
        session.Submit();

        Assert.Equal(4, session.Keystrokes.Count);
        Assert.Equal(3, session.Keystrokes.Count(k => k.Correct));
    }
}
