using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CommandLibrary
{
    private static readonly CommandEntry[] System = Build(CommandCategory.System,
        ("uname -a", "Print kernel and system information", Easy),
        ("uptime", "Show how long the system has been running", Easy),
        ("free -h", "Show memory usage in human-readable units", Easy),
        ("lsblk", "List block devices", Easy),
        ("lscpu", "Show CPU information", Easy),
        ("cat /etc/os-release", "Show which distribution you are running", Medium),
        ("sudo systemctl status nginx", "Check the status of a service", Medium),
        ("sudo systemctl restart nginx", "Restart a service", Medium),
        ("sudo systemctl enable --now docker", "Enable a service at boot and start it now", Hard),
        ("journalctl -u nginx -f", "Follow the logs of a systemd unit", Medium),
        ("journalctl --since '1 hour ago'", "Show log entries from the last hour", Hard),
        ("dmesg | tail", "Show recent kernel messages", Medium),
        ("sudo reboot", "Reboot the machine", Easy),
        ("sudo shutdown -h now", "Power off immediately", Medium),
        ("date +%Y-%m-%d", "Print the current date in ISO format", Medium),
        ("crontab -e", "Edit your scheduled cron jobs", Easy),
        ("env | sort", "List environment variables alphabetically", Easy),
        ("export PATH=$HOME/.local/bin:$PATH", "Prepend a directory to your PATH", Hard),
        ("alias ll='ls -alF'", "Define a handy shell alias", Medium),
        ("source ~/.bashrc", "Reload your bash configuration", Easy),
        ("man tar", "Read the manual page for tar", Easy),
        ("sudo mount /dev/sdb1 /mnt/usb", "Mount a USB drive", Medium));
}
