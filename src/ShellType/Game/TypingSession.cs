namespace ShellType.Game;

public enum SessionState
{
    Ready,
    Running,
    Finished,
}

/// <summary>A single key press, kept so we can chart speed and errors over time.</summary>
public readonly record struct Keystroke(TimeSpan At, bool Correct);

/// <summary>
/// The state machine behind one typing test. It knows nothing about the UI:
/// the page feeds it keys and renders whatever it exposes.
/// </summary>
public sealed class TypingSession
{
    private const int Lookahead = 4;

    private readonly List<CommandAttempt> _attempts = [];
    private readonly List<Keystroke> _keystrokes = [];
    private readonly ICommandSource _source;
    private readonly TimeProvider _time;
    private long _startTimestamp;
    private TimeSpan _finishedAt;

    public TypingSession(TestConfig config, ICommandSource source, TimeProvider? time = null)
    {
        Config = config;
        _source = source;
        _time = time ?? TimeProvider.System;

        var initial = config.Mode == TestMode.Commands ? config.CommandCount : Lookahead + 1;
        for (var i = 0; i < initial; i++)
        {
            _attempts.Add(new CommandAttempt(_source.Next()));
        }
    }

    public event Action? Finished;

    public TestConfig Config { get; }
    public SessionState State { get; private set; } = SessionState.Ready;
    public int CurrentIndex { get; private set; }
    public CommandAttempt Current => _attempts[CurrentIndex];
    public IReadOnlyList<CommandAttempt> Attempts => _attempts;
    public IReadOnlyList<Keystroke> Keystrokes => _keystrokes;
    public DateTimeOffset? CompletedAt { get; private set; }

    public TimeSpan Elapsed => State switch
    {
        SessionState.Running => _time.GetElapsedTime(_startTimestamp),
        SessionState.Finished => _finishedAt,
        _ => TimeSpan.Zero,
    };

    /// <summary>Commands that have been fully submitted.</summary>
    public IEnumerable<CommandAttempt> Completed => _attempts.Take(CurrentIndex + (State == SessionState.Finished && Current.IsSubmitted ? 1 : 0));

    /// <summary>Commands still to come, after the current one.</summary>
    public IEnumerable<CommandAttempt> Upcoming => _attempts.Skip(CurrentIndex + 1);

    /// <summary>Feeds a printable character. Returns true if it was the right one.</summary>
    public bool Type(char c)
    {
        if (State == SessionState.Finished)
        {
            return false;
        }

        if (State == SessionState.Ready)
        {
            Start();
        }

        var at = Elapsed;
        var correct = Current.WouldBeCorrect(c);
        if (!Current.Append(c, at))
        {
            return false;
        }

        _keystrokes.Add(new Keystroke(at, correct));

        if (Config.Mode == TestMode.Commands && CurrentIndex == _attempts.Count - 1 && Current.IsCorrect)
        {
            Submit();
        }

        return correct;
    }

    public void Backspace()
    {
        if (State == SessionState.Running)
        {
            Current.Backspace();
        }
    }

    /// <summary>Finishes the current command, like pressing Enter in a terminal.</summary>
    public void Submit()
    {
        if (State != SessionState.Running || Current.TypedLength == 0)
        {
            return;
        }

        var at = Elapsed;
        _keystrokes.Add(new Keystroke(at, Current.IsCorrect));
        Current.Submit(at);

        if (Config.Mode == TestMode.Commands && CurrentIndex == _attempts.Count - 1)
        {
            Finish();
            return;
        }

        CurrentIndex++;
        if (Config.Mode == TestMode.Time)
        {
            while (_attempts.Count - CurrentIndex <= Lookahead)
            {
                _attempts.Add(new CommandAttempt(_source.Next()));
            }
        }
    }

    public void Finish()
    {
        if (State == SessionState.Finished)
        {
            return;
        }

        _finishedAt = State == SessionState.Running ? Elapsed : TimeSpan.Zero;
        State = SessionState.Finished;
        CompletedAt = _time.GetUtcNow();
        Finished?.Invoke();
    }

    private void Start()
    {
        _startTimestamp = _time.GetTimestamp();
        State = SessionState.Running;
    }
}
