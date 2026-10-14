using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CiscoLibrary
{
    private static readonly CommandEntry[] Services =
    [
        C(Config, "access-list 10 permit 192.168.1.0 0.0.0.255", "A standard ACL line that matches the 192.168.1.0/24 network", Hard),
        C(If, "ip access-group 10 in", "Apply ACL 10 to traffic coming in on this interface", Medium),
        C(Config, "ip nat inside source list 1 interface GigabitEthernet0/1 overload", "PAT: let every address in ACL 1 share the outside interface's address", Hard),
        C(Config, "ip dhcp pool LAN", "Create a DHCP pool so the router hands out addresses", Medium),
    ];
}
