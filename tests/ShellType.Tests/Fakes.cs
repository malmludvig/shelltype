using ShellType.Game;
using ShellType.Models;

namespace ShellType.Tests;

/// <summary>A clock the test controls.</summary>
internal sealed class ManualTimeProvider : TimeProvider
{
    private long _ticks;

    public override long TimestampFrequency => TimeSpan.TicksPerSecond;

    public override long GetTimestamp() => _ticks;

    public override DateTimeOffset GetUtcNow() => new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero).AddTicks(_ticks);

    public void Advance(TimeSpan by) => _ticks += by.Ticks;

    public void Advance(double seconds) => Advance(TimeSpan.FromSeconds(seconds));
}

/// <summary>Hands out a fixed list of commands, cycling when it runs out.</summary>
internal sealed class FixedCommandSource(params string[] texts) : ICommandSource
{
    private int _index;

    public CommandEntry Next() =>
        new(texts[_index++ % texts.Length], "test", CommandCategory.Navigation, Difficulty.Easy);
}
