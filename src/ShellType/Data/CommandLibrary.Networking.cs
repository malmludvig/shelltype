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
        ("sudo ufw allow 443/tcp", "Open HTTPS in the firewall", Medium));
}
