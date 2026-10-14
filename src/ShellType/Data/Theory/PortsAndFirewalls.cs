using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic PortsAndFirewalls = new(
        "ports-and-firewalls",
        "Ports, sockets and firewalls",
        "networking",
        "TCP and UDP, what a port and a listening socket are, how to see them with ss, and how Linux firewalls decide what gets in.",
        """
        ## From machines to programs

        An IP address gets a packet to the right **machine**. But a machine runs many networked programs at once: a web server, an ssh server, a database. **Ports** get the packet to the right **program**. A port is a number from 0 to 65535, and a connection is identified by both ends: source address and port, destination address and port.

        ## TCP and UDP

        Two transport protocols carry almost all traffic:

        - **TCP** sets up a connection with a handshake, delivers bytes reliably and in order, and resends anything lost. Web (HTTP/HTTPS), ssh, email and databases use TCP.
        - **UDP** just sends individual packets with no connection and no guarantees. It is fast and simple, used for DNS, video calls, games and VPNs like WireGuard.

        TCP port 53 and UDP port 53 are different things, so always say which protocol you mean.

        ## Well-known ports

        Ports below 1024 are **privileged**: only root may listen on them, which is why web servers start as root and then drop privileges. Some you will see all the time:

        - 22 ssh, 25 smtp (mail), 53 DNS
        - 80 HTTP, 443 HTTPS
        - 3306 MySQL, 5432 PostgreSQL, 6379 Redis, 27017 MongoDB
        - 8080, 3000, 5000 and 8000: common development and application ports

        `/etc/services` lists the traditional names for port numbers.

        ## Sockets and listening

        A program that wants to receive connections opens a **socket**, **binds** it to an address and port, and **listens**. The address it binds to matters a lot:

        - `127.0.0.1:5432` listens only on loopback: reachable from the same machine, invisible from the network.
        - `0.0.0.0:5432` (or `*:5432`, or `[::]:5432` for IPv6) listens on **all** interfaces.

        "It works locally but not from another machine" is very often a service bound to `127.0.0.1`. Only one program can listen on a given address and port at a time; a second one fails with **Address already in use**.

        ## Seeing what is open: ss

        `ss` shows sockets. The options are worth memorizing as one word, **-tulpn**:

        ```
        sudo ss -tulpn
        Netid State  Local Address:Port  Process
        tcp   LISTEN 0.0.0.0:22          users:(("sshd",pid=812,fd=3))
        tcp   LISTEN 127.0.0.1:5432      users:(("postgres",pid=1044,fd=5))
        tcp   LISTEN *:80                users:(("nginx",pid=1203,fd=6))
        ```

        `-t` TCP, `-u` UDP, `-l` listening only, `-p` show the process (needs sudo for other users' processes), `-n` numbers instead of names. Without `-l`, `ss -tn` shows established connections. `sudo lsof -i :8080` answers "who is using port 8080?" from the other direction.

        To test whether a port is reachable from somewhere else, use `nc -zv host port` or `curl -v http://host:port/`. A **connection refused** means the packet arrived but nothing listens there. A **timeout** usually means a firewall silently dropped it.

        ## Firewalls

        The Linux kernel contains a packet filter, **netfilter**, which checks every packet against rules and decides to accept, drop or reject it. Rules are usually organized in **chains**: **INPUT** for packets to this machine, **OUTPUT** for packets from it, and **FORWARD** for packets passing through (routers, containers).

        There are several ways to manage those rules:

        - **iptables**: the classic command, still very common. `sudo iptables -L -n -v --line-numbers` lists rules.
        - **nftables** (`nft`): the modern replacement with a cleaner syntax.
        - **ufw** (Ubuntu): a simple front end. `sudo ufw allow 443/tcp`, `sudo ufw status verbose`.
        - **firewalld** (Fedora, RHEL): zones and services, managed with `firewall-cmd`.

        The usual secure policy is **default deny incoming**: drop everything that is not explicitly allowed, then open only the ports you need, ideally only for the addresses that need them:

        ```
        sudo ufw default deny incoming
        sudo ufw default allow outgoing
        sudo ufw allow OpenSSH
        sudo ufw allow from 10.0.0.0/24 to any port 5432 proto tcp
        sudo ufw enable
        ```

        Always allow ssh **before** enabling a firewall on a remote server, or you lock yourself out. Remember too that cloud providers usually have their own firewall (security groups) in front of the machine, and both must allow the traffic.

        ## A debugging order

        When a service is unreachable: is it running (`systemctl status`)? Is it listening, and on which address (`ss -tlpn`)? Does it answer locally (`curl localhost:port`)? Does the host firewall allow it (`ufw status`, `iptables -L`)? Does a cloud or network firewall allow it? Does the traffic even arrive (`tcpdump -n port 443`)? Each question rules out one layer.
        """);
}
