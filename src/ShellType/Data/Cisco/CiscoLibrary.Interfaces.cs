using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CiscoLibrary
{
    private static readonly CommandEntry[] Interfaces =
    [
        C(Config, "interface GigabitEthernet0/0", "Enter interface configuration mode for one port", Medium),
        C(If, "ip address 192.168.1.1 255.255.255.0", "Give the interface an IPv4 address and subnet mask", Hard),
        C(If, "no shutdown", "Turn the interface on; router ports are shut down by default", Easy),
        C(If, "description Link to SW1", "Label the port so the next person knows what it connects to", Medium),
        C(Exec, "show ip interface brief", "One line per interface: IP address and whether it is up/up", Medium),
        C(Exec, "show interfaces GigabitEthernet0/0", "Details for one interface: speed, duplex, errors and counters", Medium),
    ];
}
