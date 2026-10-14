using ShellType.Models;
using static ShellType.Models.CommandCategory;

namespace ShellType.Data;

/// <summary>
/// Everyday sysadmin tasks as sequences of commands, typed in order in operations mode.
/// </summary>
public static class OperationLibrary
{
    public static IReadOnlyList<Operation> All { get; } =
    [
        // ---- short: 2 to 4 steps ----
        Op("kill a runaway process", "something is eating the cpu. find it and stop it.", Processes,
            ("top -b -n 1 | head -15", "Snapshot the busiest processes right now"),
            ("pgrep -a python3", "Get the PID and full command line of every python3 process"),
            ("kill 48213", "Ask the process to stop gracefully (SIGTERM)"),
            ("ps -p 48213", "Check that it is really gone")),

        Op("free a busy port", "port 8080 is already in use. find who holds it and stop it.", Networking,
            ("sudo ss -ltnp | grep :8080", "Find the process listening on port 8080"),
            ("ps -fp 3127", "See what PID 3127 actually is before killing it"),
            ("sudo kill 3127", "Stop the process holding the port"),
            ("ss -ltn | grep :8080", "Confirm nothing listens on the port anymore")),

        Op("restart a crashed service", "nginx is down. find out why and bring it back.", CommandCategory.System,
            ("systemctl status nginx", "See the service state and its last log lines"),
            ("sudo nginx -t", "Test the configuration for syntax errors"),
            ("sudo systemctl restart nginx", "Start the service again"),
            ("journalctl -u nginx -n 20 --no-pager", "Read the latest log lines to make sure it stays up")),

        Op("find what fills the disk", "the disk is almost full. find the culprit and clean up.", CommandCategory.System,
            ("df -h", "See which filesystem is running out of space"),
            ("sudo du -sh /var/* | sort -h | tail -5", "List the five biggest directories under /var"),
            ("sudo journalctl --vacuum-size=200M", "Shrink the systemd journal to 200 MB")),

        Op("log in with an ssh key", "stop typing passwords: set up key login to a server.", Networking,
            ("ssh-keygen -t ed25519 -C \"me@laptop\"", "Create a new ed25519 key pair"),
            ("ssh-copy-id deploy@10.0.0.12", "Install the public key on the server"),
            ("ssh deploy@10.0.0.12", "Log in, this time without a password")),

        Op("run a new script", "you downloaded a deploy script. make it runnable and try it.", Permissions,
            ("ls -l deploy.sh", "Check the current permissions"),
            ("chmod +x deploy.sh", "Make the script executable"),
            ("./deploy.sh --dry-run", "Run it without changing anything")),

        Op("switch branch mid-work", "an urgent bug on main, but your work is half done.", Git,
            ("git status", "See what is uncommitted"),
            ("git stash", "Put your unfinished changes aside"),
            ("git switch main", "Go to the main branch"),
            ("git pull", "Get the latest commits")),

        // ---- medium: 5 to 7 steps ----
        Op("open the firewall for a web server", "the site works locally but not from outside. check the firewall.", Networking,
            ("curl -I http://localhost", "Confirm the server answers locally"),
            ("sudo ss -ltnp | grep :80", "Check it listens on all interfaces, not just 127.0.0.1"),
            ("sudo ufw status verbose", "Look at the firewall rules and default policies"),
            ("sudo ufw allow 80/tcp", "Allow plain HTTP"),
            ("sudo ufw allow 443/tcp", "Allow HTTPS"),
            ("sudo ufw reload", "Apply the new rules"),
            ("sudo ufw status numbered", "Verify the rules are in place")),

        Op("disable ssh password login", "edit sshd_config safely so only keys can log in.", CommandCategory.System,
            ("sudo cp /etc/ssh/sshd_config /etc/ssh/sshd_config.bak", "Back up the config before touching it"),
            ("grep -n PasswordAuthentication /etc/ssh/sshd_config", "Find the line to change"),
            ("sudo sed -i 's/^#\\?PasswordAuthentication yes/PasswordAuthentication no/' /etc/ssh/sshd_config", "Turn password login off in place"),
            ("diff /etc/ssh/sshd_config.bak /etc/ssh/sshd_config", "Review exactly what changed"),
            ("sudo sshd -t", "Validate the new config before applying it"),
            ("sudo systemctl reload sshd", "Apply it without dropping current sessions")),

        Op("hunt down a memory hog", "the server is swapping. find the process using all the ram.", Processes,
            ("free -h", "See how much memory and swap is used"),
            ("ps aux --sort=-%mem | head -5", "List the processes using the most memory"),
            ("pidof node", "Get the PID of the node process"),
            ("grep VmRSS /proc/7731/status", "Read how much memory it really holds"),
            ("kill -TERM 7731", "Stop it gracefully"),
            ("sudo systemctl restart api", "Restart the service that owned it"),
            ("vmstat 1 5", "Watch memory and swap settle for five seconds")),

        Op("dig through web server logs", "users report errors. find out what is going on.", Text,
            ("cd /var/log/nginx", "Go to the nginx log directory"),
            ("ls -lh", "See which log files exist and how big they are"),
            ("tail -n 50 error.log", "Read the latest errors"),
            ("grep -c \" 500 \" access.log", "Count requests that failed with status 500"),
            ("awk '{print $1}' access.log | sort | uniq -c | sort -rn | head", "Find the IPs making the most requests"),
            ("sudo tail -f error.log", "Follow new errors as they happen")),

        Op("add a new admin user", "a new colleague needs sudo and ssh access.", Permissions,
            ("sudo adduser alice", "Create the user with a home directory"),
            ("sudo usermod -aG sudo alice", "Add her to the sudo group"),
            ("sudo mkdir -p /home/alice/.ssh", "Create her ssh directory"),
            ("sudo cp authorized_keys /home/alice/.ssh/", "Install her public key"),
            ("sudo chown -R alice:alice /home/alice/.ssh", "Give her ownership of the ssh files"),
            ("sudo chmod 700 /home/alice/.ssh", "Lock the directory down, or sshd ignores it"),
            ("id alice", "Verify her groups")),

        Op("back up a site", "take a backup and ship it to another server.", Archives,
            ("tar -czf site-backup.tar.gz /var/www/site", "Pack the site into a compressed archive"),
            ("ls -lh site-backup.tar.gz", "Check the archive size"),
            ("tar -tzf site-backup.tar.gz | head", "Peek inside to make sure the files are there"),
            ("sha256sum site-backup.tar.gz", "Get a checksum to verify the copy later"),
            ("scp site-backup.tar.gz backup@10.0.0.20:/backups/", "Copy it to the backup server")),

        Op("fix broken dns", "the internet works but names do not resolve.", Networking,
            ("ping -c 3 8.8.8.8", "Check raw connectivity by IP"),
            ("ping -c 3 example.com", "Check whether names resolve"),
            ("cat /etc/resolv.conf", "See which DNS server is configured"),
            ("dig example.com +short", "Ask the configured resolver directly"),
            ("dig @1.1.1.1 example.com +short", "Ask a public resolver to compare"),
            ("resolvectl status", "See per-interface DNS settings"),
            ("sudo systemctl restart systemd-resolved", "Restart the local resolver")),

        Op("ship a hotfix", "branch off main, fix the bug and push it for review.", Git,
            ("git fetch origin", "Get the latest state of the remote"),
            ("git switch -c hotfix/login origin/main", "Create a hotfix branch from main"),
            ("git add src/auth.js", "Stage the fix"),
            ("git commit -m \"Fix login redirect\"", "Commit it with a clear message"),
            ("git push -u origin hotfix/login", "Push the branch and track it")),

        Op("kill a stuck process", "a backup job hangs and ignores you. stop it for real.", Processes,
            ("ps aux | grep [b]ackup", "Find the backup process without matching grep itself"),
            ("pgrep -f backup.sh", "Get just its PID"),
            ("kill 9120", "Ask it nicely first (SIGTERM)"),
            ("ps -o pid,stat,cmd -p 9120", "See if it is still there and in which state"),
            ("kill -9 9120", "Force it to die (SIGKILL)")),

        // ---- long: 8 to 10 steps ----
        Op("open a database port with iptables", "postgres is unreachable from the app servers. walk the firewall rules.", Networking,
            ("sudo systemctl status postgresql", "Make sure the database is actually running"),
            ("sudo ss -ltnp | grep 5432", "Check it listens on the network, not only localhost"),
            ("sudo iptables -L INPUT -n -v --line-numbers", "List the input rules with packet counters"),
            ("sudo iptables -I INPUT 1 -p tcp --dport 5432 -s 10.0.0.0/24 -j ACCEPT", "Allow the app subnet at the top of the chain"),
            ("sudo iptables -L INPUT -n --line-numbers | head -5", "Verify the new rule is first"),
            ("sudo tcpdump -i eth0 -n port 5432 -c 10", "Watch connection attempts arrive"),
            ("sudo iptables-save | sudo tee /etc/iptables/rules.v4", "Save the rules so they survive a reboot"),
            ("sudo netfilter-persistent reload", "Reload the saved rules")),

        Op("investigate a misbehaving process", "the load is high. trace the process, then restart it cleanly.", Processes,
            ("uptime", "Check the load averages"),
            ("top -b -n 1 -o %CPU | head -12", "See the top cpu users"),
            ("ps -eo pid,ppid,user,%cpu,%mem,cmd --sort=-%cpu | head", "List the hungriest processes with their parents"),
            ("pgrep -u www-data -l php", "Find the php workers owned by www-data"),
            ("pstree -p 2210", "See the process tree under the php master"),
            ("sudo lsof -p 2214 | head -20", "List the files and sockets the worker has open"),
            ("sudo strace -p 2214 -f -e trace=network", "Watch which network calls it makes"),
            ("kill -15 2214", "Stop the stuck worker"),
            ("ps -p 2214 || echo gone", "Confirm it exited"),
            ("sudo systemctl restart php8.3-fpm", "Restart the pool to get fresh workers")),

        Op("roll out a new nginx site", "edit a site config, enable it and check it works.", CommandCategory.System,
            ("cd /etc/nginx/sites-available", "Go to the available site configs"),
            ("sudo cp example.conf example.conf.bak", "Back up the config first"),
            ("sudo vim example.conf", "Edit the server block"),
            ("grep -n \"server_name\\|listen\" example.conf", "Double-check the hostnames and ports"),
            ("sudo ln -s /etc/nginx/sites-available/example.conf /etc/nginx/sites-enabled/", "Enable the site"),
            ("sudo nginx -t", "Test the full configuration"),
            ("sudo systemctl reload nginx", "Apply it without downtime"),
            ("curl -I https://example.com", "Check the site answers"),
            ("sudo tail -n 20 /var/log/nginx/error.log", "Make sure no new errors appeared")),

        Op("secure a fresh server", "a brand new vps: update it, firewall it, protect ssh.", Packages,
            ("sudo apt update", "Refresh the package lists"),
            ("sudo apt upgrade -y", "Install all pending updates"),
            ("sudo apt install -y ufw fail2ban unattended-upgrades", "Install the firewall, fail2ban and auto updates"),
            ("sudo ufw default deny incoming", "Block all incoming traffic by default"),
            ("sudo ufw default allow outgoing", "Allow all outgoing traffic"),
            ("sudo ufw allow OpenSSH", "Keep ssh open so you do not lock yourself out"),
            ("sudo ufw enable", "Turn the firewall on"),
            ("sudo systemctl enable --now fail2ban", "Start fail2ban now and on every boot"),
            ("sudo fail2ban-client status sshd", "Check that the ssh jail is active"),
            ("sudo dpkg-reconfigure -plow unattended-upgrades", "Turn on automatic security updates")),

        Op("clean up a full disk", "/ is at 100% and things are failing. free space fast.", CommandCategory.System,
            ("df -h /", "Confirm how full the root filesystem is"),
            ("sudo du -xh / --max-depth=1 2>/dev/null | sort -h | tail", "Find the biggest top-level directories"),
            ("sudo du -sh /var/log/* | sort -h | tail -5", "Find the biggest logs"),
            ("sudo find /var/log -name \"*.gz\" -mtime +30 -delete", "Delete rotated logs older than 30 days"),
            ("sudo journalctl --disk-usage", "See how much the journal takes"),
            ("sudo journalctl --vacuum-time=7d", "Keep only the last week of journal"),
            ("docker system df", "See how much space docker uses"),
            ("docker system prune -f", "Remove stopped containers and dangling images"),
            ("sudo apt autoremove --purge -y", "Remove packages nothing needs anymore"),
            ("df -h /", "Check how much space you won back")),

        Op("rotate a leaked api key", "a key was pushed to github. replace it in the config and restart.", Permissions,
            ("grep -rn \"API_KEY\" /etc/myapp/", "Find every place the key is configured"),
            ("sudo cp /etc/myapp/app.env /etc/myapp/app.env.bak", "Back up the env file"),
            ("openssl rand -hex 32", "Generate a new random key"),
            ("sudo sed -i 's/^API_KEY=.*/API_KEY=4f9c2e7a/' /etc/myapp/app.env", "Swap in the new key"),
            ("sudo chmod 600 /etc/myapp/app.env", "Make the file readable only by its owner"),
            ("sudo chown root:myapp /etc/myapp/app.env", "Set the right owner and group"),
            ("sudo systemctl restart myapp", "Restart the app to load the new key"),
            ("systemctl is-active myapp", "Check that it came back up"),
            ("journalctl -u myapp --since \"5 min ago\"", "Look for auth errors since the restart")),

        Op("find the commit that broke it", "the tests passed in v2.3.0 and fail now. bisect.", Git,
            ("git bisect start", "Start a bisect session"),
            ("git bisect bad", "Mark the current commit as broken"),
            ("git bisect good v2.3.0", "Mark the last known good release"),
            ("npm test", "Run the tests on the commit git picked"),
            ("git bisect good", "This one passes, so the bug is later"),
            ("npm test", "Test the next commit git picked"),
            ("git bisect bad", "This one fails, so the bug is here or earlier"),
            ("git show --stat HEAD", "Inspect the first bad commit"),
            ("git bisect reset", "End the bisect and go back to where you were")),
    ];

    public static IReadOnlyList<Operation> ByLength(OperationLength length) =>
        All.Where(o => o.Length == length).ToList();

    private static Operation Op(
        string name,
        string goal,
        CommandCategory category,
        params (string Text, string Description)[] steps) =>
        new(name, goal, steps.Select(s => new CommandEntry(s.Text, s.Description, category, Difficulty.Medium)).ToList());
}
