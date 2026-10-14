using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CiscoLibrary
{
    private static readonly CommandEntry[] Switching =
    [
        C(SwConfig, "vlan 10", "Create VLAN 10 and enter VLAN configuration mode", Easy),
        C(Vlan, "name SALES", "Give the VLAN a name that shows up in show vlan", Easy),
        C(SwIf, "switchport mode access", "Make the port an access port that carries a single VLAN", Medium),
        C(SwIf, "switchport access vlan 10", "Put the access port in VLAN 10", Medium),
        C(SwIf, "switchport mode trunk", "Make the port a trunk that carries many VLANs to another switch or router", Medium),
        C(SwExec, "show vlan brief", "List VLANs and the access ports in each one", Easy),
    ];
}
