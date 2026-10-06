using ShellType.Game;
using ShellType.Models;

namespace ShellType.Tests;

public class ReplaySourceTests
{
    [Fact]
    public void Replays_in_order_then_falls_back()
    {
        CommandEntry[] replay =
        [
            new("pwd", "d", CommandCategory.Navigation, Difficulty.Easy),
            new("ls", "d", CommandCategory.Navigation, Difficulty.Easy),
        ];
        var source = new ReplaySource(replay, new FixedCommandSource("whoami"));

        var texts = Enumerable.Range(0, 4).Select(_ => source.Next().Text).ToList();

        Assert.Equal(["pwd", "ls", "whoami", "whoami"], texts);
    }
}
