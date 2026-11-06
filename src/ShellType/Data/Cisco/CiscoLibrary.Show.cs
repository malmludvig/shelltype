using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CiscoLibrary
{
    private static readonly CommandEntry[] Show =
    [
        C(Exec, "show running-config", "Show the configuration the device is using right now", Easy),
        C(Exec, "show startup-config", "Show the saved configuration that is loaded at boot", Easy),
        C(Exec, "show version", "Show the IOS version, uptime, model, memory and config register", Easy),
        C(Exec, "show cdp neighbors", "List directly connected Cisco devices and which ports link them", Medium),
        C(Exec, "show logging", "Show the log buffer: interface flaps, errors and config changes", Easy),
        C(Exec, "ping 8.8.8.8", "Test reachability; each ! is a reply and each . a timeout", Easy),
        C(Exec, "traceroute 8.8.8.8", "Show every router hop on the path to the destination", Easy),
    ];
}
