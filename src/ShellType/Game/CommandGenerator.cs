using ShellType.Models;

namespace ShellType.Game;

/// <summary>
/// Supplies an endless stream of commands to type.
/// </summary>
public interface ICommandSource
{
    CommandEntry Next();
}

/// <summary>
/// Picks random commands from a pool, avoiding repeats until a good chunk
/// of the pool has been used so short tests feel varied.
/// </summary>
public sealed class CommandGenerator : ICommandSource
{
    private readonly IReadOnlyList<CommandEntry> _pool;
    private readonly Random _random;
    private readonly Queue<CommandEntry> _recent = new();
    private readonly int _memory;

    public CommandGenerator(IReadOnlyList<CommandEntry> pool, Random? random = null)
    {
        if (pool.Count == 0)
        {
            throw new ArgumentException("The command pool cannot be empty.", nameof(pool));
        }

        _pool = pool;
        _random = random ?? Random.Shared;
        _memory = pool.Count / 2;
    }

    public CommandEntry Next()
    {
        CommandEntry pick;
        do
        {
            pick = _pool[_random.Next(_pool.Count)];
        }
        while (_recent.Contains(pick));

        _recent.Enqueue(pick);
        if (_recent.Count > _memory)
        {
            _recent.Dequeue();
        }

        return pick;
    }

    public IEnumerable<CommandEntry> Take(int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return Next();
        }
    }
}
