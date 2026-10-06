using ShellType.Quiz;

namespace ShellType.Data;

/// <summary>
/// Real-world terminal errors for the "fix the error" quiz. Wrong fixes are chosen to be
/// tempting: the common mistakes people actually make.
/// </summary>
public static class ErrorScenarios
{
    public static readonly IReadOnlyList<ErrorScenario> All =
    [
        new("./deploy.sh",
            "bash: ./deploy.sh: Permission denied",
            "chmod +x deploy.sh",
            "The script is missing its execute bit. chmod +x lets you run it.",
            ["sudo rm deploy.sh", "chown root deploy.sh", "export PATH=.:$PATH"]),

        new("apt install nginx",
            "E: Could not open lock file /var/lib/dpkg/lock-frontend - open (13: Permission denied)",
            "sudo apt install nginx",
            "Installing packages changes the system, so apt needs root. Never delete or chmod the lock file.",
            ["chmod 777 /var/lib/dpkg", "rm /var/lib/dpkg/lock-frontend", "apt install --force nginx"]),

        new("htop",
            "bash: htop: command not found",
            "sudo apt install htop",
            "The program isn't installed (or isn't on your PATH). Install it with your package manager.",
            ["chmod +x htop", "source htop", "sudo htop"]),

        new("cat config.yml",
            "cat: config.yml: No such file or directory",
            "find . -name config.yml",
            "The file isn't in the current directory. Find where it actually lives first.",
            ["sudo cat config.yml", "chmod +r config.yml", "cat -f config.yml"]),

        new("rmdir build",
            "rmdir: failed to remove 'build': Directory not empty",
            "rm -r build",
            "rmdir only removes empty directories. rm -r removes a directory and its contents.",
            ["rmdir -f build", "sudo rmdir build", "rm build"]),

        new("npm start",
            "Error: listen EADDRINUSE: address already in use :::3000",
            "lsof -i :3000",
            "Something is already listening on port 3000. lsof shows which process, so you can stop it.",
            ["sudo npm start", "npm start --force", "ping localhost:3000"]),

        new("git push",
            "fatal: The current branch feature has no upstream branch.",
            "git push -u origin feature",
            "A new branch doesn't know where to push yet. -u sets origin/feature as its upstream.",
            ["git push --force", "git pull", "git commit --amend"]),

        new("git pull",
            "error: Your local changes to the following files would be overwritten by merge",
            "git stash",
            "Stash your uncommitted changes, pull, then git stash pop to bring them back.",
            ["git push", "git branch -D main", "git rm -r ."]),

        new("ssh deploy@10.0.0.5",
            "deploy@10.0.0.5: Permission denied (publickey).",
            "ssh -i ~/.ssh/deploy_key deploy@10.0.0.5",
            "The server only accepts keys, and SSH didn't offer the right one. Point it at the key with -i.",
            ["sudo ssh deploy@10.0.0.5", "chmod 777 ~/.ssh", "ping 10.0.0.5"]),

        new("ssh -i key.pem ubuntu@host",
            "WARNING: UNPROTECTED PRIVATE KEY FILE! Permissions 0644 for 'key.pem' are too open.",
            "chmod 600 key.pem",
            "SSH refuses private keys that other users can read. 600 means only you can read and write it.",
            ["chmod 777 key.pem", "chown root key.pem", "sudo ssh -i key.pem ubuntu@host"]),

        new("cp big.iso /mnt/usb/",
            "cp: error writing '/mnt/usb/big.iso': No space left on device",
            "df -h",
            "The target filesystem is full. df -h shows free space on every mounted filesystem.",
            ["free -h", "sudo cp big.iso /mnt/usb/", "cp -f big.iso /mnt/usb/"]),

        new("systemctl restart nginx",
            "Failed to restart nginx.service: Interactive authentication required.",
            "sudo systemctl restart nginx",
            "Managing system services requires root privileges.",
            ["systemctl restart nginx --force", "kill -9 nginx", "chmod +x nginx.service"]),

        new("sudo systemctl start nginx",
            "Job for nginx.service failed because the control process exited with error code.",
            "journalctl -u nginx -e",
            "The service crashed on startup. Its logs tell you why: often a typo in the config.",
            ["sudo reboot", "sudo apt remove nginx", "systemctl enable nginx"]),

        new("python3 app.py",
            "ModuleNotFoundError: No module named 'requests'",
            "pip install requests",
            "The Python package isn't installed in this environment. Install it with pip.",
            ["sudo python3 app.py", "chmod +x app.py", "apt install python3"]),

        new("tar -xzf archive.tar",
            "gzip: stdin: not in gzip format",
            "tar -xf archive.tar",
            "The -z flag means gzip, but this .tar isn't compressed. Drop the z.",
            ["gunzip archive.tar", "sudo tar -xzf archive.tar", "tar -czf archive.tar"]),

        new("docker ps",
            "permission denied while trying to connect to the Docker daemon socket",
            "sudo usermod -aG docker $USER",
            "Your user isn't in the docker group. Add it, then log out and back in.",
            ["chmod +x /usr/bin/docker", "sudo rm /var/run/docker.sock", "docker ps --force"]),

        new("./configure",
            "bash: ./configure: /bin/bash^M: bad interpreter: No such file or directory",
            "dos2unix configure",
            "The ^M means the file has Windows line endings (CRLF). dos2unix converts them to Unix ones.",
            ["chmod +x configure", "sudo ./configure", "bash -x configure"]),

        new("echo hello > /etc/motd",
            "bash: /etc/motd: Permission denied",
            "echo hello | sudo tee /etc/motd",
            "Redirection (>) is done by your own shell before sudo runs, so sudo echo doesn't help. tee runs as root and writes the file.",
            ["sudo echo hello > /etc/motd", "chmod +w /etc/motd", "echo hello >> /etc/motd"]),

        new("mkdir projects/app/src",
            "mkdir: cannot create directory 'projects/app/src': No such file or directory",
            "mkdir -p projects/app/src",
            "The parent directories don't exist yet. -p creates them all in one go.",
            ["sudo mkdir projects/app/src", "mkdir -r projects/app/src", "touch projects/app/src"]),

        new("curl https://localhost:8443",
            "curl: (60) SSL certificate problem: self-signed certificate",
            "curl -k https://localhost:8443",
            "-k skips certificate verification. Fine for a local dev server, never for real sites.",
            ["sudo curl https://localhost:8443", "curl -X https://localhost:8443", "curl -L https://localhost:8443"]),

        new("ping github.com",
            "ping: github.com: Temporary failure in name resolution",
            "cat /etc/resolv.conf",
            "DNS isn't working. Check which nameservers the system is configured to use.",
            ["chmod +x /usr/bin/ping", "ping -c 4 github.com", "git clone github.com"]),

        new("kill 4321",
            "bash: kill: (4321) - No such process",
            "pgrep -a myapp",
            "That PID doesn't exist (anymore). Look up the process's current PID first.",
            ["kill -9 4321", "sudo kill 4321", "killall 4321"]),

        new("git commit -m 'wip'",
            "Author identity unknown *** Please tell me who you are.",
            "git config --global user.email 'you@example.com'",
            "Git needs a name and email for commits. Set them once with git config --global.",
            ["git init", "sudo git commit -m 'wip'", "git add ."]),

        new("nano /etc/hosts",
            "[ File '/etc/hosts' is unwritable ]",
            "sudo nano /etc/hosts",
            "System files are owned by root. Open the editor with sudo (or use sudoedit).",
            ["chmod 777 /etc/hosts", "nano -w /etc/hosts", "rm /etc/hosts"]),

        new("sudo umount /mnt/usb",
            "umount: /mnt/usb: target is busy.",
            "lsof +D /mnt/usb",
            "Some process still has files open on the drive (maybe a shell cd'd into it). lsof finds it.",
            ["rm -rf /mnt/usb", "mount /mnt/usb", "umount -r /mnt/usb"]),

        new("git checkout main",
            "error: pathspec 'main' did not match any file(s) known to git",
            "git branch -a",
            "There's no branch called main here. List the branches: it may be called master.",
            ["git init main", "git checkout -f main", "git push origin main"]),
    ];
}
