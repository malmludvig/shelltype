using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CommandLibrary
{
    private static readonly CommandEntry[] Networking = Build(CommandCategory.Networking,
        ("ping -c 4 8.8.8.8", "Send four ICMP echo requests", Easy),
        ("curl https://example.com", "Fetch a URL and print the response body", Easy),
        ("curl -I https://example.com", "Fetch only the response headers", Medium),
        ("curl -fsSL https://get.example.sh | bash", "Download and run an install script", Hard),
        ("curl -X POST -H 'Content-Type: application/json' -d '{}' localhost:3000/api", "POST a JSON body to a local API", Hard),
        ("wget https://example.com/file.tar.gz", "Download a file", Easy),
        ("ssh user@server", "Open a remote shell over SSH", Easy),
        ("ssh -p 2222 -i ~/.ssh/id_ed25519 deploy@10.0.0.5", "SSH on a custom port with a specific key", Hard),
        ("ssh-keygen -t ed25519 -C 'me@example.com'", "Generate a new SSH key pair", Hard),
        ("scp backup.sql user@host:/tmp/", "Copy a file to a remote host", Medium),
        ("ip addr show", "Show network interfaces and their addresses", Easy),
        ("ip route", "Show the routing table", Easy),
        ("ss -tulpn", "List listening TCP and UDP sockets with their processes", Medium),
        ("netstat -an | grep LISTEN", "Show listening sockets (older tool)", Medium),
        ("dig example.com +short", "Resolve a domain name", Medium),
        ("nslookup example.com", "Query DNS for a domain", Easy),
        ("traceroute example.com", "Trace the route packets take to a host", Easy),
        ("nc -zv localhost 5432", "Check whether a port is open", Medium),
        ("hostname -I", "Print the IP addresses of this machine", Easy),
        ("sudo ufw allow 443/tcp", "Open HTTPS in the firewall", Medium),
        ("sudo ufw status", "Show the firewall status and rules", Easy),

        // iproute2
        ("ip a", "Show interfaces and addresses (short form)", Easy),
        ("ip -c -br a", "Show addresses in colored, brief form", Medium),
        ("ip l", "Show network links (short form)", Easy),
        ("ip r", "Show the routing table (short form)", Easy),
        ("ip link show", "Show network links", Easy),
        ("ip neigh", "Show the ARP / neighbour table", Easy),
        ("ip route get 8.8.8.8", "Show which route a packet to an address would take", Medium),
        ("ip addr add 10.0.0.1/24 dev eth0", "Assign an address to eth0", Medium),
        ("sudo ip addr add 192.168.1.50/24 dev ens3", "Assign a LAN address to ens3", Hard),
        ("sudo ip addr add 192.168.122.10/24 dev ens4", "Assign a libvirt-network address to ens4", Hard),
        ("ip link set eth0 up", "Bring eth0 up", Easy),
        ("ip link set ens5 down", "Take ens5 down", Easy),
        ("sudo ip link set dev eth0 up", "Bring eth0 up with sudo", Medium),
        ("sudo ip link set dev eth0 down", "Take eth0 down with sudo", Medium),
        ("sudo ip link set dev ens0 down", "Take ens0 down with sudo", Medium),

        // Diagnostics
        ("sudo tcpdump -i eth0", "Capture packets on eth0", Medium),
        ("nmap -F 192.168.1.1", "Fast-scan the most common ports on a host", Medium),
        ("nmap 192.168.1.50", "Scan a host for open ports", Easy),
        ("netstat -tunlp", "List listening TCP and UDP ports with their processes", Medium),
        ("nc -zv -w 3 192.168.1.1 20-30", "Scan a port range with a 3 second timeout", Hard),
        ("nc -l 4444", "Listen for a connection on port 4444", Easy),
        ("nc 192.0.2.10 4444", "Connect to a listener on port 4444", Medium),
        ("mtr google.com", "Continuously trace the route to a host", Easy),

        // Config files
        ("cat /etc/hosts", "Show static hostname mappings", Easy),
        ("cat /etc/network/interfaces", "Show Debian-style interface config", Medium),
        ("cat /etc/netplan/01-netcfg.yaml", "Show the Netplan network config", Medium),
        ("cat /etc/resolv.conf", "Show the DNS resolver config", Easy),
        ("sudo nano /etc/network/interfaces", "Edit Debian-style interface config", Medium),
        ("cat /etc/systemd/network/20-wired.network", "Show a systemd-networkd link config", Hard),
        ("sudo dhclient ens4", "Request a DHCP lease on ens4", Easy),

        // Services
        ("sudo systemctl restart networking", "Restart the networking service", Medium),
        ("sudo systemctl restart NetworkManager", "Restart NetworkManager", Medium),
        ("sudo systemctl restart systemd-resolved", "Restart the systemd DNS resolver", Medium),
        ("sudo systemctl status systemd-networkd", "Check the status of systemd-networkd", Medium),
        ("sudo systemctl restart systemd-networkd", "Restart systemd-networkd", Medium),
        ("sudo systemctl enable systemd-networkd", "Enable systemd-networkd at boot", Medium),

        // systemd-networkd / resolved
        ("networkctl list", "List links managed by systemd-networkd", Easy),
        ("networkctl status", "Show overall systemd-networkd status", Easy),
        ("networkctl status eth0", "Show systemd-networkd status for eth0", Easy),
        ("networkctl reconfigure eth0", "Reapply network config to eth0", Medium),
        ("networkctl reload", "Reload systemd-networkd config files", Easy),
        ("resolvectl status", "Show DNS settings per link", Easy),
        ("resolvectl flush-caches", "Flush the local DNS cache", Medium),

        // NetworkManager
        ("nmcli device status", "List devices and their connection state", Easy),
        ("nmcli device show eth0", "Show details for eth0", Easy),
        ("nmcli device wifi list", "List visible Wi-Fi networks", Easy),
        ("nmcli device wifi connect \"Home-WiFi\" password \"secret123\"", "Connect to a Wi-Fi network", Hard),
        ("nmcli connection show", "List saved connections", Easy),
        ("nmcli connection show --active", "List active connections", Medium),
        ("nmcli general status", "Show overall NetworkManager status", Easy),
        ("nmcli connection up id \"Home-WiFi\"", "Activate a saved connection", Medium),
        ("nmcli connection down id \"Home-WiFi\"", "Deactivate a connection", Medium),
        ("nmcli connection add type ethernet con-name \"Static-Eth\" ifname eth0 ip4 192.168.1.50/24 gw4 192.168.1.1", "Create a static Ethernet connection", Hard),
        ("nmcli connection modify \"Static-Eth\" ipv4.dns \"8.8.8.8 1.1.1.1\"", "Set DNS servers on a connection", Hard),
        ("nmcli connection delete \"Static-Eth\"", "Delete a saved connection", Medium));
}
