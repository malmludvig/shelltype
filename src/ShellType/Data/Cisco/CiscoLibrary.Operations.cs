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
        // ---- medium: 5 to 7 steps ----
        Op("configure a lan interface", "the lan port needs an address so hosts can use it as their gateway.",
            (Exec, "configure terminal", "Enter global configuration mode"),
            (Config, "interface GigabitEthernet0/0", "Select the LAN port"),
            (If, "description LAN", "Label what the port is for"),
            (If, "ip address 192.168.10.1 255.255.255.0", "Give it the gateway address for the LAN"),
            (If, "no shutdown", "Turn the port on"),
            (If, "end", "Go back to privileged EXEC"),
            (Exec, "show ip interface brief", "Check the address and that it is up/up")),

        Op("create a vlan", "the engineers need their own vlan on port 5.",
            (SwExec, "configure terminal", "Enter global configuration mode"),
            (SwConfig, "vlan 20", "Create VLAN 20"),
            (Vlan, "name ENGINEERING", "Name it so it is easy to recognize"),
            (Vlan, "exit", "Back to global configuration"),
            (SwConfig, "interface FastEthernet0/5", "Select the port the engineer uses"),
            (SwIf, "switchport mode access", "Make it an access port"),
            (SwIf, "switchport access vlan 20", "Put it in the new VLAN")),

        Op("set up a trunk", "connect two switches so both vlans reach the other side.",
            (SwExec, "configure terminal", "Enter global configuration mode"),
            (SwConfig, "interface GigabitEthernet0/1", "Select the uplink to the other switch"),
            (SwIf, "switchport trunk encapsulation dot1q", "Use 802.1Q tagging, needed on switches that also know ISL"),
            (SwIf, "switchport mode trunk", "Make the port a trunk"),
            (SwIf, "switchport trunk allowed vlan 10,20", "Carry only the VLANs that need to cross"),
            (SwIf, "end", "Go back to privileged EXEC"),
            (SwExec, "show interfaces trunk", "Check the trunk is up and which VLANs it carries")),

        Op("secure the console", "anyone with a cable can log in. lock the console port.",
            (Exec, "configure terminal", "Enter global configuration mode"),
            (Config, "line console 0", "Select the console line"),
            (Line, "password Cisco123", "Set the console password"),
            (Line, "login", "Ask for that password when someone connects"),
            (Line, "logging synchronous", "Stop log messages from breaking up what you type"),
            (Line, "exec-timeout 5 0", "Log out idle sessions after five minutes"),
            (Line, "end", "Go back to privileged EXEC")),

        Op("route to a branch office", "the branch network 10.2.0.0/16 sits behind 10.0.0.2. add a route.",
            (Exec, "configure terminal", "Enter global configuration mode"),
            (Config, "ip route 10.2.0.0 255.255.0.0 10.0.0.2", "Send traffic for the branch to the branch router"),
            (Config, "end", "Leave configuration mode"),
            (Exec, "show ip route static", "Check the new S route is installed"),
            (Exec, "ping 10.2.0.1", "Test that the branch answers")),

        Op("block a host with an acl", "192.168.1.66 is attacking the network. block it at the router.",
            (Exec, "configure terminal", "Enter global configuration mode"),
            (Config, "access-list 110 deny ip host 192.168.1.66 any", "Deny all traffic from the attacker"),
            (Config, "access-list 110 permit ip any any", "Allow everything else; ACLs end with an implicit deny"),
            (Config, "interface GigabitEthernet0/0", "Select the LAN port the traffic comes in on"),
            (If, "ip access-group 110 in", "Apply the ACL to incoming traffic"),
            (If, "end", "Go back to privileged EXEC"),
            (Exec, "show access-lists", "Watch the match counters on the deny line")),

        Op("add a local admin", "stop sharing one password. give the admin a personal login.",
            (Exec, "configure terminal", "Enter global configuration mode"),
            (Config, "username admin privilege 15 secret Cisco123", "Create a user who lands straight in privileged EXEC"),
            (Config, "line vty 0 4", "Select the remote login lines"),
            (Line, "login local", "Ask for a username and password from the local users"),
            (Line, "end", "Go back to privileged EXEC"),
            (Exec, "copy running-config startup-config", "Save so the user survives a reboot")),

        Op("give a switch a management ip", "you want to reach the switch remotely. give it an address.",
            (SwExec, "configure terminal", "Enter global configuration mode"),
            (SwConfig, "interface vlan 1", "Select the switch's virtual interface in VLAN 1"),
            (SwIf, "ip address 192.168.1.2 255.255.255.0", "Give the switch an address in the LAN"),
            (SwIf, "no shutdown", "Turn the virtual interface on"),
            (SwIf, "exit", "Back to global configuration"),
            (SwConfig, "ip default-gateway 192.168.1.1", "Let the switch answer from other networks too"),
            (SwConfig, "end", "Leave configuration mode")),

        Op("lock down a switch port", "only the office pc may use port 3. stop anyone plugging in more.",
            (SwExec, "configure terminal", "Enter global configuration mode"),
            (SwConfig, "interface FastEthernet0/3", "Select the port"),
            (SwIf, "switchport mode access", "Port security only works on access ports"),
            (SwIf, "switchport port-security", "Turn on port security"),
            (SwIf, "switchport port-security maximum 2", "Allow at most two MAC addresses"),
            (SwIf, "switchport port-security violation restrict", "Drop and log traffic from extra devices"),
            (SwIf, "end", "Go back to privileged EXEC")),
    ];

    private static Operation Op(
        string name,
        string goal,
        params (string Prompt, string Text, string Description)[] steps) =>
        new(name, goal, steps.Select(s => new CommandEntry(s.Text, s.Description, CommandCategory.Networking, Difficulty.Medium, s.Prompt)).ToList());
}
