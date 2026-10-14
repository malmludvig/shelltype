using ShellType.Models;

namespace ShellType.Data;

/// <summary>
/// The 50 most common Cisco IOS commands, each shown at the prompt of the mode it is typed in.
/// Each group lives in its own partial file.
/// </summary>
public static partial class CiscoLibrary
{
    // The prompt tells you which mode you are in: > user EXEC, # privileged EXEC, (config)# and below configuration.
    private const string User = "R1>";
    private const string Exec = "R1#";
    private const string Config = "R1(config)#";
    private const string If = "R1(config-if)#";
    private const string Line = "R1(config-line)#";
    private const string Ospf = "R1(config-router)#";
    private const string Dhcp = "R1(dhcp-config)#";
    private const string SwExec = "SW1#";
    private const string SwConfig = "SW1(config)#";
    private const string SwIf = "SW1(config-if)#";
    private const string Vlan = "SW1(config-vlan)#";

    private static readonly Lazy<IReadOnlyList<CommandEntry>> AllCommands =
        new(() => Sources().SelectMany(s => s).ToList());

    /// <summary>All 50 commands, in learning order.</summary>
    public static IReadOnlyList<CommandEntry> Essentials => AllCommands.Value;

    private static IEnumerable<CommandEntry[]> Sources()
    {
        yield return Modes;
        yield return Show;
        yield return Interfaces;
        yield return RemoteAccess;
        yield return Routing;
        yield return Switching;
    }

    private static CommandEntry C(string prompt, string text, string description, Difficulty difficulty) =>
        new(text, description, CommandCategory.Networking, difficulty, prompt);
}
