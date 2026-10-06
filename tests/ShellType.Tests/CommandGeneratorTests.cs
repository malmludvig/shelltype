using ShellType.Data;
using ShellType.Game;
using ShellType.Models;

namespace ShellType.Tests;

public class CommandGeneratorTests
{
    private static readonly CommandEntry[] Pool =
    [
        new("ls", "list", CommandCategory.Navigation, Difficulty.Easy),
        new("pwd", "print dir", CommandCategory.Navigation, Difficulty.Easy),
        new("cd ..", "up", CommandCategory.Navigation, Difficulty.Easy),
        new("whoami", "me", CommandCategory.Permissions, Difficulty.Easy),
    ];

    [Fact]
    public void Empty_pool_is_rejected()
    {
        Assert.Throws<ArgumentException>(() => new CommandGenerator([]));
    }

    [Fact]
    public void Same_seed_gives_same_sequence()
    {
        var a = new CommandGenerator(CommandLibrary.All, new Random(42)).Take(20).ToList();
        var b = new CommandGenerator(CommandLibrary.All, new Random(42)).Take(20).ToList();

        Assert.Equal(a, b);
    }

    [Fact]
    public void Never_repeats_back_to_back()
    {
        var generator = new CommandGenerator(Pool, new Random(1));
        var sequence = generator.Take(200).ToList();

        for (var i = 1; i < sequence.Count; i++)
        {
            Assert.NotEqual(sequence[i - 1], sequence[i]);
        }
    }

    [Fact]
    public void Single_item_pool_still_works()
    {
        var generator = new CommandGenerator([Pool[0]]);

        Assert.All(generator.Take(5), c => Assert.Equal("ls", c.Text));
    }

    [Fact]
    public void Only_returns_commands_from_the_pool()
    {
        var generator = new CommandGenerator(Pool, new Random(7));

        Assert.All(generator.Take(50), c => Assert.Contains(c, Pool));
    }
}
