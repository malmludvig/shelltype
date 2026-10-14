using ShellType.Game;
using ShellType.Models;
using ShellType.Services;

namespace ShellType.Tests;

public class TestModeTests
{
    [Theory]
    [InlineData(TestMode.Time, 30, Platform.Linux, "time 30")]
    [InlineData(TestMode.Time, 30, Platform.Cisco, "cisco time 30")]
    [InlineData(TestMode.Commands, 10, Platform.Cisco, "cisco commands 10")]
    [InlineData(TestMode.Essentials, 50, Platform.Cisco, "cisco all 50")]
    [InlineData(TestMode.Operations, 2, Platform.Cisco, "cisco operations medium")]
    public void Labels_name_the_mode_and_the_platform(TestMode mode, int value, Platform platform, string expected)
    {
        Assert.Equal(expected, TestModes.Label(mode, value, platform));
    }

    [Fact]
    public void Fixed_lists_have_no_difficulty_label()
    {
        Assert.Equal("", TestModes.DifficultyLabel(TestMode.Essentials, Difficulty.Hard));
        Assert.Equal("hard", TestModes.DifficultyLabel(TestMode.Time, Difficulty.Hard));
    }

    [Fact]
    public void Essentials_falls_back_to_commands_on_linux()
    {
        var settings = new UserSettings { Platform = Platform.Linux, Mode = TestMode.Essentials };
        Assert.Equal(TestMode.Commands, settings.ToTestConfig().Mode);

        settings.Platform = Platform.Cisco;
        Assert.Equal(TestMode.Essentials, settings.ToTestConfig().Mode);
        Assert.Equal(Platform.Cisco, settings.ToTestConfig().Platform);
    }
}
