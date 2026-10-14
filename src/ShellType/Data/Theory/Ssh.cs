using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Ssh = new(
        "ssh",
        "SSH and keys",
        "networking",
        "How SSH keeps remote logins safe, public and private keys, host keys, the config file, tunnels and hardening the server.",
        """
        ## Remote shells, encrypted

        **SSH** (Secure Shell) is how you log in to and manage remote Linux machines. Everything, your password or key, the commands you type and the output you see, travels through an **encrypted** connection, so nobody on the network can read or tamper with it. On the server, the daemon **sshd** listens on TCP port 22; on your machine, the client is `ssh`.

        ```
        ssh alice@203.0.113.10          # log in
        ssh -p 2222 alice@server        # a non-standard port
        ssh server 'df -h'              # run one command and return
        ```

        ## Host keys: is this the right server?

        The first time you connect to a server, ssh shows its **host key fingerprint** and asks whether to trust it. Your answer is saved in `~/.ssh/known_hosts`. On every later connection ssh checks that the server presents the same key. If it suddenly changes, ssh refuses with a loud **REMOTE HOST IDENTIFICATION HAS CHANGED** warning, because someone might be impersonating the server (a man-in-the-middle attack). Often the real reason is harmless, a reinstalled server with new keys, and `ssh-keygen -R hostname` removes the old entry. But it is worth confirming before you do.

        ## Key-based login

        Passwords can be guessed and are attacked constantly on any server exposed to the internet. **Public-key authentication** is both safer and more convenient. You create a **key pair**:

        - The **private key** stays on your machine and must never be shared. Protect it with a passphrase.
        - The **public key** can be given to anyone. You put it on every server you want to log in to.

        ```
        ssh-keygen -t ed25519 -C "alice@laptop"     # creates ~/.ssh/id_ed25519 and id_ed25519.pub
        ssh-copy-id alice@server                    # appends the public key to the server's authorized_keys
        ssh alice@server                            # now logs in with the key
        ```

        When you connect, the server challenges the client to prove it holds the private key matching a public key in `~/.ssh/authorized_keys`, without the private key ever crossing the network. **ed25519** keys are the modern recommended type; RSA keys should be at least 3072 bits.

        sshd is strict about permissions: `~/.ssh` must be `700` and `authorized_keys` `600`, and your home directory must not be writable by others. Otherwise it silently ignores the key and falls back to asking for a password, which is one of the most common "my key does not work" causes. `ssh -v` on the client and the server's auth log show what is going on.

        ## The ssh agent

        Typing the key passphrase for every connection gets old. The **ssh-agent** keeps decrypted keys in memory for your session: `ssh-add` once, and later connections just work. **Agent forwarding** (`ssh -A`) lets a server use your agent to hop further, but only enable it for servers you trust, since root on that server could use your agent while you are connected.

        ## The client config file

        `~/.ssh/config` saves you from remembering users, ports and keys:

        ```
        Host web
            HostName 203.0.113.10
            User deploy
            Port 2222
            IdentityFile ~/.ssh/id_ed25519

        Host db
            HostName 10.0.0.5
            User admin
            ProxyJump web
        ```

        Now `ssh web` is enough, and `ssh db` connects through `web` as a jump host, reaching a database server that is not exposed to the internet directly. `scp`, `rsync` and `git` all use these settings too.

        ## Copying files and tunnels

        ```
        scp backup.tar.gz web:/backups/
        rsync -avz ./site/ web:/var/www/site/
        ssh -L 5432:localhost:5432 db       # local port 5432 now reaches the database through ssh
        ssh -D 1080 web                     # a SOCKS proxy through the server
        ```

        **Local port forwarding** (`-L`) is especially handy: it gives you secure access to a service that only listens on the server's localhost, without opening it in the firewall.

        ## Hardening the server

        A few settings in `/etc/ssh/sshd_config` make an internet-facing server much safer:

        ```
        PasswordAuthentication no      # keys only
        PermitRootLogin no             # log in as a user, then use sudo
        PubkeyAuthentication yes
        AllowUsers alice deploy        # optionally, only these users
        ```

        Check the file with `sudo sshd -t` and apply with `sudo systemctl reload ssh`. Keep your current session open while testing the new settings in a **second** terminal, so a mistake cannot lock you out. Add **fail2ban** to block addresses that keep failing to log in, and keep the system updated. With keys only, the endless password-guessing bots on the internet simply have nothing to guess.
        """);
}
