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
        new(() => Sources().SelectMany(s => s.Commands).ToList());

    private static readonly Lazy<IReadOnlyList<(CiscoCategory Category, CommandEntry Command)>> Practice =
        new(() => Sources().Concat(ExtraSources()).SelectMany(s => s.Commands.Select(c => (s.Category, c))).ToList());

    /// <summary>All 50 commands, in learning order.</summary>
    public static IReadOnlyList<CommandEntry> Essentials => AllCommands.Value;

    /// <summary>The commands in the given categories up to a difficulty, for time and commands tests. An empty category set means every category.</summary>
    public static IReadOnlyList<CommandEntry> Filter(IReadOnlyCollection<CiscoCategory> categories, Difficulty maxDifficulty) =>
        Practice.Value
            .Where(p => (categories.Count == 0 || categories.Contains(p.Category)) && p.Command.Difficulty <= maxDifficulty)
            .Select(p => p.Command)
            .ToList();

    /// <summary>The category a command belongs to, e.g. <see cref="CiscoCategory.Vlan"/> for <c>switchport access vlan 10</c>.</summary>
    public static CiscoCategory CategoryOf(CommandEntry command) =>
        Practice.Value.First(p => p.Command == command).Category;

    /// <summary>The name a category is shown with, e.g. "acl, nat &amp; dhcp".</summary>
    public static string Label(CiscoCategory category) => category switch
    {
        CiscoCategory.Basics => "basics",
        CiscoCategory.Interfaces => "interfaces",
        CiscoCategory.Vlan => "vlan",
        CiscoCategory.Routing => "routing",
        CiscoCategory.Security => "security",
        _ => "acl, nat & dhcp",
    };

    /// <summary>The name of the IOS mode a prompt belongs to, e.g. "interface configuration" for <c>R1(config-if)#</c>.</summary>
    public static string ModeName(string prompt)
    {
        if (prompt.EndsWith('>'))
        {
            return "user EXEC";
        }

        var open = prompt.IndexOf('(');
        if (open < 0)
        {
            return "privileged EXEC";
        }

        return prompt[(open + 1)..prompt.IndexOf(')')] switch
        {
            "config" => "global configuration",
            "config-if" => "interface configuration",
            "config-subif" => "subinterface configuration",
            "config-line" => "line configuration",
            "config-router" => "router configuration",
            "config-vlan" => "VLAN configuration",
            "dhcp-config" => "DHCP pool configuration",
            var other => other,
        };
    }

    private static IEnumerable<(CiscoCategory Category, CommandEntry[] Commands)> Sources()
    {
        yield return (CiscoCategory.Basics, Modes);
        yield return (CiscoCategory.Basics, Show);
        yield return (CiscoCategory.Interfaces, Interfaces);
        yield return (CiscoCategory.Security, Passwords);
        yield return (CiscoCategory.Security, RemoteAccess);
        yield return (CiscoCategory.Routing, Routing);
        yield return (CiscoCategory.Vlan, Switching);
        yield return (CiscoCategory.Services, Services);
    }

    private static CommandEntry C(string prompt, string text, string description, Difficulty difficulty) =>
        new(text, description, CommandCategory.Networking, difficulty, prompt);
}
