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
        C(Config, "enable secret Cisco123", "Protect privileged EXEC with a hashed password", Medium),
        C(Config, "service password-encryption", "Hide plain-text passwords in the config with weak type 7 encryption", Medium),
        C(Config, "banner motd #Authorized access only#", "Set the message shown to everyone who connects", Hard),
        C(Config, "no ip domain-lookup", "Stop IOS from treating a mistyped command as a hostname to look up", Medium),
    ];
}
