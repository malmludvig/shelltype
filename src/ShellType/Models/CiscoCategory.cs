namespace ShellType.Models;

/// <summary>
/// Broad groups of Cisco IOS commands that can be toggled on and off in a Cisco test.
/// </summary>
public enum CiscoCategory
{
    /// <summary>Moving between modes, saving the config and looking at the device.</summary>
    Basics,

    /// <summary>Addressing ports, turning them on and checking on them.</summary>
    Interfaces,

    /// <summary>Creating VLANs, access ports and trunks.</summary>
    Vlan,

    /// <summary>Static routes and OSPF.</summary>
    Routing,

    /// <summary>Passwords, local users and locking down console and ssh access.</summary>
    Security,

    /// <summary>Access lists, NAT and DHCP.</summary>
    Services,
}
