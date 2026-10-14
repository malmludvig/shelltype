using ShellType.Models;

namespace ShellType.Data;

public static partial class CiscoLibrary
{
    private const string SubIf = "R1(config-subif)#";

    /// <summary>Everyday network tasks as sequences of IOS commands, typed in order in operations mode.</summary>
    public static IReadOnlyList<Operation> Operations { get; } =
    [
        // ---- short: 2 to 4 steps ----
        Op("save the config", "you changed the config. make sure it survives a reboot.",
            (Exec, "show running-config", "Look over the active config before saving it"),
            (Exec, "copy running-config startup-config", "Copy the running config to NVRAM"),
            (Exec, "show startup-config", "Confirm the saved config now has your changes")),

        Op("bring up an interface", "a new link is plugged in but the port is down. turn it on.",
            (Config, "interface GigabitEthernet0/1", "Select the port"),
            (If, "no shutdown", "Router ports start administratively down; turn it on"),
            (If, "end", "Go back to privileged EXEC"),
            (Exec, "show ip interface brief", "Check that the port now says up/up")),

        Op("name a new router", "a fresh router still says Router. give it a real name.",
            ("Router#", "configure terminal", "Enter global configuration mode"),
            ("Router(config)#", "hostname R1", "Set the name; watch the prompt change"),
            (Config, "end", "Leave configuration mode")),

        Op("find a down interface", "users on one port lost the network. find out why.",
            (Exec, "show ip interface brief", "Spot the port that is not up/up"),
            (Exec, "show interfaces GigabitEthernet0/2", "Read its status, errors and last input"),
            (Exec, "show logging | include 0/2", "See when the link went down in the log")),

        Op("add a default route", "the router reaches the lan but not the internet. point it at the isp.",
            (Exec, "configure terminal", "Enter global configuration mode"),
            (Config, "ip route 0.0.0.0 0.0.0.0 203.0.113.1", "Send everything without a better route to the ISP"),
            (Config, "end", "Leave configuration mode"),
            (Exec, "show ip route", "Look for the S* gateway of last resort")),

        Op("protect privileged mode", "anyone at the console can type enable. require a password.",
            (Exec, "configure terminal", "Enter global configuration mode"),
            (Config, "enable secret Cisco123", "Require a hashed password for enable"),
            (Config, "service password-encryption", "Hide the other plain-text passwords too"),
            (Config, "end", "Leave configuration mode")),

        Op("check ospf health", "routes are missing. make sure ospf is up with its neighbors.",
            (Exec, "show ip ospf neighbor", "Every neighbor should be in the FULL state"),
            (Exec, "show ip route ospf", "List only the routes learned from OSPF"),
            (Exec, "show ip protocols", "Check which networks OSPF advertises")),

        Op("map your neighbors", "you are new on the network. find out what is connected.",
            (Exec, "show cdp neighbors", "List the Cisco devices on each port"),
            (Exec, "show cdp neighbors detail", "Get their IP addresses and IOS versions"),
            (Exec, "ping 10.0.0.2", "Check that the neighbor answers")),
    ];

    private static Operation Op(
        string name,
        string goal,
        params (string Prompt, string Text, string Description)[] steps) =>
        new(name, goal, steps.Select(s => new CommandEntry(s.Text, s.Description, CommandCategory.Networking, Difficulty.Medium, s.Prompt)).ToList());
}
