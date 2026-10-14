using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Networking = new(
        "networking",
        "IP addresses, interfaces and routing",
        "networking",
        "How a Linux machine gets on the network: interfaces, IP addresses and subnets, the default gateway and the ip command.",
        """
        ## Interfaces

        A machine talks to networks through **network interfaces**. Each physical network card, Wi-Fi adapter or virtual device is one interface with a name:

        - `lo` the **loopback** interface, a virtual network inside the machine itself, always at `127.0.0.1`.
        - `eth0`, or predictable names like `enp3s0` and `ens18`: wired Ethernet.
        - `wlan0` or `wlp2s0`: Wi-Fi.
        - `docker0`, `br0`, `veth...`, `tun0`, `wg0`: virtual bridges, container links and VPN tunnels.

        The modern tool to inspect and configure all of this is `ip`. The older `ifconfig` and `route` still exist on many systems but are deprecated.

        ```
        ip link                # interfaces and whether they are UP
        ip -br addr            # brief: each interface with its addresses
        ip addr show eth0      # details for one interface
        ip -s link show eth0   # counters: packets, errors, drops
        ```

        ## IP addresses and subnets

        An **IPv4 address** is four numbers from 0 to 255, like `192.168.1.50`. It is always paired with a **prefix length**, written after a slash: `192.168.1.50/24`. The prefix says how many leading bits identify the **network**; the rest identify the **host** within it. `/24` means the first three numbers are the network (`192.168.1.0`) and the last number picks one of up to 254 hosts. Older notation writes the same as a **netmask**: `255.255.255.0`.

        Machines in the **same subnet** talk to each other directly. To reach anything outside the subnet they need a router.

        Some address ranges are special:

        - `127.0.0.0/8` loopback, this machine.
        - `10.0.0.0/8`, `172.16.0.0/12` and `192.168.0.0/16`: **private** ranges used inside homes and companies. They are not routed on the internet; a router translates them to a public address with **NAT**.
        - `169.254.0.0/16` link-local; an address here usually means DHCP failed.

        **IPv6** addresses are much longer, written in hex, like `2001:db8::1/64`. The ideas are the same, and Linux runs both side by side.

        ## How a machine gets its address

        Most machines get their address automatically from a **DHCP** server (usually the router), which also hands out the gateway and DNS servers. Servers often use a **static** address configured in a file. Depending on the distribution that configuration is done by **NetworkManager** (`nmcli`), **systemd-networkd**, Ubuntu's **netplan** (YAML in `/etc/netplan/`) or `/etc/network/interfaces` on older Debian.

        Changes made with `ip addr add` take effect immediately but are lost on reboot; permanent settings belong in those configuration tools.

        ## Routing and the default gateway

        For every packet, the kernel looks up the **routing table** to decide where to send it:

        ```
        ip route
        default via 192.168.1.1 dev eth0
        192.168.1.0/24 dev eth0 proto kernel scope link src 192.168.1.50
        ```

        The second line says the local subnet is reachable directly on `eth0`. The first line is the **default route**: anything else goes to the **gateway** `192.168.1.1`, the router, which passes it on toward the internet. A missing or wrong default route is a classic cause of "the local network works but nothing outside it does". `ip route get 1.1.1.1` shows exactly which route a given destination would use.

        Between IP addresses and actual Ethernet frames sits **ARP**, which finds the hardware (MAC) address belonging to an IP on the local network. `ip neigh` shows what the machine has learned.

        ## Testing connectivity, layer by layer

        When "the network is down", test from the bottom up:

        ```
        ip link                     # 1. is the interface UP, is a cable connected?
        ip -br addr                 # 2. does it have an address?
        ip route                    # 3. is there a default route?
        ping -c 3 192.168.1.1       # 4. can I reach the gateway?
        ping -c 3 1.1.1.1           # 5. can I reach the internet by IP?
        ping -c 3 example.com       # 6. does name resolution work?
        traceroute 1.1.1.1          # where along the way do packets stop?
        ```

        If step 5 works but step 6 fails, the problem is DNS, covered in the next chapter. `mtr` combines ping and traceroute into a live view that is great for spotting where packet loss happens.

        ## Hostnames

        Every machine also has a **hostname**, its own name, shown by `hostname` and set with `hostnamectl set-hostname web01`. The file `/etc/hosts` maps names to addresses locally and is checked before DNS, which makes it handy for testing and for giving names to machines on a small network.
        """);
}
