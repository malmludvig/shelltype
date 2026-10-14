using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Dns = new(
        "dns",
        "DNS and name resolution",
        "networking",
        "How a name like example.com becomes an IP address, the record types you will meet, and how to debug it with dig.",
        """
        ## Names for numbers

        Computers connect to IP addresses, but people use names. The **Domain Name System (DNS)** is the worldwide, distributed directory that turns names like `example.com` into addresses like `93.184.215.14`. Almost every connection your machine makes starts with a DNS lookup, so when DNS breaks, it looks like "the internet is down" even though the network itself is fine.

        ## How a lookup works

        Names are hierarchical, read from right to left: `www.example.com.` is `www` inside `example` inside `com` inside the root `.`. No single server knows every name. Instead:

        1. Your machine asks a **resolver**, usually provided by your router, your company or a public service like `1.1.1.1` or `8.8.8.8`.
        2. If the resolver does not have the answer cached, it asks a **root server**, which says "ask the servers for .com".
        3. The `.com` servers say "ask the servers responsible for example.com".
        4. Those **authoritative** name servers give the final answer.
        5. The resolver returns it to you and **caches** it for as long as the record's **TTL** (time to live) allows.

        Caching is why DNS is fast, and also why DNS changes take time to spread: until the TTL runs out, resolvers keep handing out the old answer.

        ## Record types

        A DNS zone holds **records** of different types:

        - **A** maps a name to an IPv4 address.
        - **AAAA** maps a name to an IPv6 address.
        - **CNAME** makes a name an alias for another name (`www` → `example.com`).
        - **MX** tells where email for the domain should be delivered.
        - **TXT** free text, used for verification and email security (SPF, DKIM, DMARC).
        - **NS** lists the authoritative name servers for a domain.
        - **PTR** reverse lookup: from an IP address back to a name.

        ## How Linux resolves a name

        On Linux, programs ask the C library to resolve names, and it follows the order in `/etc/nsswitch.conf`. The usual line is:

        ```
        hosts: files dns
        ```

        which means: check `/etc/hosts` first, then ask DNS. That is why an entry in `/etc/hosts` overrides real DNS, handy for testing a new server before switching the public records.

        Which DNS server to ask is configured in `/etc/resolv.conf`. On many modern systems that file points to `127.0.0.53`, a local caching stub run by **systemd-resolved**, which forwards queries to the real servers it learned from DHCP or your network settings. `resolvectl status` shows those real servers per interface.

        ## Tools for asking DNS

        ```
        dig example.com                  # full answer with details and timing
        dig +short example.com           # just the addresses
        dig example.com MX               # a specific record type
        dig @1.1.1.1 example.com         # ask a specific server
        dig +trace example.com           # follow the lookup from the root down
        dig -x 93.184.215.14             # reverse lookup
        host example.com                 # quick and simple
        getent hosts example.com         # resolve exactly the way programs do, including /etc/hosts
        ```

        The difference between `dig` and `getent` matters: `dig` talks to DNS directly and ignores `/etc/hosts`, while `getent` uses the same path as your applications. If they disagree, look at `/etc/hosts` and nsswitch.

        ## Debugging DNS problems

        A typical routine:

        1. `ping -c 3 1.1.1.1` works but `ping example.com` fails? The network is fine; it is DNS.
        2. `cat /etc/resolv.conf` and `resolvectl status`: which server is being used?
        3. `dig example.com` versus `dig @1.1.1.1 example.com`: if the public resolver answers and yours does not, your configured resolver is the problem.
        4. If a **recently changed** record shows the old value, it is probably cached. Check the TTL in `dig`'s output, and flush local caches with `resolvectl flush-caches`.
        5. If only some names fail, check for typos in records, missing CNAME targets, or an `/etc/hosts` entry overriding things.

        Error messages give hints too. "Name or service not known" and "Could not resolve host" mean the lookup failed. **NXDOMAIN** in `dig` means the name definitely does not exist, while **SERVFAIL** or a timeout points to a broken or unreachable resolver.

        There is an old joke among admins: "It's not DNS. There's no way it's DNS. It was DNS." Checking name resolution early in any network investigation saves a lot of time.
        """);
}
