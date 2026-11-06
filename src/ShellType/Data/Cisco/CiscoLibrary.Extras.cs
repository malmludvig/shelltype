using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CiscoLibrary
{
    // More than the 50 essentials, so every category is worth practicing on its own at every difficulty.
    private static IEnumerable<(CiscoCategory Category, CommandEntry[] Commands)> ExtraSources()
    {
        yield return (CiscoCategory.Basics, ExtraBasics);
        yield return (CiscoCategory.Interfaces, ExtraInterfaces);
        yield return (CiscoCategory.Routing, ExtraRouting);
        yield return (CiscoCategory.Vlan, ExtraVlan);
        yield return (CiscoCategory.Services, ExtraServices);
    }

    private static readonly CommandEntry[] ExtraBasics =
    [
        C(Exec, "show running-config | include hostname", "Show only the config lines that contain a word", Hard),
        C(Exec, "show running-config | section line", "Show only the config sections for the console and vty lines", Hard),
        C(Exec, "copy running-config tftp://192.168.1.10/r1.cfg", "Back up the active configuration to a TFTP server", Hard),
    ];

    private static readonly CommandEntry[] ExtraInterfaces =
    [
        C(If, "shutdown", "Turn the interface off without removing its configuration", Easy),
        C(If, "duplex full", "Send and receive at the same time on this port", Easy),
        C(If, "speed 100", "Lock the port to 100 Mbps instead of negotiating", Medium),
        C(If, "ipv6 address 2001:db8:1::1/64", "Give the interface an IPv6 address and prefix length", Hard),
        C(Config, "interface range GigabitEthernet0/0 - 1", "Configure several ports at once with the same commands", Hard),
    ];

    private static readonly CommandEntry[] ExtraRouting =
    [
        C(Exec, "show ip protocols", "Show which routing protocols run and which networks they advertise", Easy),
        C(SwConfig, "ip routing", "Turn on routing, which is off by default on a layer 3 switch", Easy),
        C(Ospf, "default-information originate", "Advertise this router's default route to its OSPF neighbors", Medium),
        C(Config, "ip route 192.168.2.0 255.255.255.0 10.0.0.2", "Add a static route: reach 192.168.2.0/24 through the next hop 10.0.0.2", Hard),
    ];

    private static readonly CommandEntry[] ExtraVlan =
    [
        C(SwConfig, "no vlan 10", "Delete VLAN 10; its access ports stop forwarding until they get a new VLAN", Easy),
        C(SwExec, "show interfaces trunk", "List the trunk ports and the VLANs each one carries", Medium),
        C(SwIf, "switchport trunk encapsulation dot1q", "Use 802.1Q tagging on a switch that also supports ISL, before making the port a trunk", Hard),
        C(SwIf, "switchport trunk allowed vlan 10,20,30", "Only carry these VLANs over the trunk", Hard),
        C(SwIf, "switchport trunk native vlan 99", "Send VLAN 99 untagged over the trunk; both ends must agree", Hard),
    ];

    private static readonly CommandEntry[] ExtraServices =
    [
        C(Exec, "show access-lists", "List every ACL with its lines and how many packets each has matched", Easy),
        C(Exec, "show ip nat translations", "List the inside and outside addresses NAT is translating right now", Easy),
        C(Exec, "show ip dhcp binding", "List the addresses the DHCP pool has handed out and to whom", Easy),
        C(If, "ip nat inside", "Mark this interface as facing the private network", Medium),
        C(If, "ip nat outside", "Mark this interface as facing the public network", Medium),
        C(Dhcp, "default-router 192.168.1.1", "Tell DHCP clients which gateway to use", Medium),
        C(Dhcp, "network 192.168.1.0 255.255.255.0", "Set the subnet this DHCP pool hands out addresses from", Hard),
        C(Config, "ip dhcp excluded-address 192.168.1.1 192.168.1.10", "Keep these addresses out of every pool, for example for the gateway", Hard),
        C(Config, "access-list 100 permit tcp any any eq 443", "An extended ACL line that matches https traffic from anywhere to anywhere", Hard),
    ];
}
