using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CiscoLibrary
{
    private static readonly CommandEntry[] Routing =
    [
        C(Config, "ip route 0.0.0.0 0.0.0.0 203.0.113.1", "Add a default route: send everything without a better match to the ISP", Hard),
        C(Config, "router ospf 1", "Start OSPF process 1 and enter router configuration mode", Easy),
        C(Ospf, "router-id 1.1.1.1", "Give the router a fixed, recognizable OSPF ID", Medium),
        C(Ospf, "network 10.0.0.0 0.0.0.255 area 0", "Run OSPF on interfaces in 10.0.0.0/24 and put them in area 0; note the wildcard mask", Hard),
        C(Ospf, "passive-interface GigabitEthernet0/1", "Advertise this LAN but send no OSPF hellos out of it", Hard),
        C(Exec, "show ip ospf neighbor", "List OSPF neighbors; FULL means the routers have synced their databases", Medium),
    ];
}
