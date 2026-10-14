using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Logging = new(
        "logging",
        "Logs: journald and syslog",
        "system",
        "Where Linux writes down what happened, how to read and filter it with journalctl, and how logs are kept from filling the disk.",
        """
        ## Why logs matter

        When something breaks, the system has usually already told you why, in a log. Services write what they do, the kernel reports hardware problems, sshd records every login attempt, and cron notes every job it starts. Knowing where logs live and how to filter them is the single most useful debugging skill on Linux.

        There are two logging systems you will meet, often running side by side: the **systemd journal** and classic **syslog** text files.

        ## The systemd journal

        On systemd machines, the service **systemd-journald** collects log messages from almost everywhere: the kernel, every service's standard output and standard error, and programs that use the syslog interface. It stores them in a structured binary format, with metadata attached to every line: which unit wrote it, PID, user, priority and an exact timestamp. You read it with `journalctl`:

        ```
        journalctl                        # everything, oldest first, in a pager
        journalctl -e                     # jump to the end
        journalctl -f                     # follow new messages live
        journalctl -u nginx               # one service
        journalctl -u nginx -n 50 --no-pager   # its last 50 lines, printed directly
        journalctl -b                     # this boot only
        journalctl -b -1                  # the previous boot
        journalctl -k                     # kernel messages
        journalctl -p err                 # errors and worse
        journalctl --since "1 hour ago"
        journalctl --since "2026-10-14 08:00" --until "09:00"
        ```

        The real power is in **combining** filters: `journalctl -u myapp -p warning --since today` gives you only warnings and errors from one service today. Add `-o json-pretty` to see all the metadata on each entry, or `--grep` to search the message text.

        ## Priorities

        Every log message has a **priority**, from most to least severe: `emerg` (0), `alert` (1), `crit` (2), `err` (3), `warning` (4), `notice` (5), `info` (6), `debug` (7). Filtering with `-p err` shows levels 0 to 3. When you are looking for why something failed, starting at `err` cuts through the noise.

        ## Persistent or not

        By default some distributions keep the journal only in memory under `/run/log/journal`, which means it is gone after a reboot. If `/var/log/journal` exists, the journal is stored there and survives reboots, so `journalctl -b -1` can show what happened right before a crash. Creating that directory, or setting `Storage=persistent` in `/etc/systemd/journald.conf`, turns it on. `journalctl --disk-usage` shows how much space it uses, and `journalctl --vacuum-time=7d` or `--vacuum-size=500M` trims it.

        ## Classic syslog files

        Before journald there was **syslog**, and on many systems a syslog daemon like **rsyslog** still runs and writes plain text files in `/var/log`:

        - `/var/log/syslog` (Debian, Ubuntu) or `/var/log/messages` (RHEL, Fedora): the general system log.
        - `/var/log/auth.log` or `/var/log/secure`: logins, sudo and ssh activity.
        - `/var/log/kern.log`: kernel messages.
        - `/var/log/dpkg.log` or the dnf logs: package installs.

        Applications often keep their own logs too, such as `/var/log/nginx/access.log` and `error.log`, or `/var/log/postgresql/`. Text logs work with every tool you already know:

        ```
        sudo tail -f /var/log/syslog
        sudo grep "Failed password" /var/log/auth.log | tail
        sudo less +G /var/log/nginx/error.log
        ```

        ## The kernel ring buffer

        The kernel keeps its most recent messages in a memory buffer. `dmesg` prints it, `dmesg -T` with readable times. This is where you find hardware errors, disks being remounted read-only, USB devices appearing and the OOM killer's reports.

        ## Log rotation

        Logs grow forever unless something trims them. **logrotate** runs daily and, according to rules in `/etc/logrotate.conf` and `/etc/logrotate.d/`, renames the current log (`syslog` becomes `syslog.1`), compresses older ones (`syslog.2.gz`) and deletes the oldest. That is why you see numbered and `.gz` files in `/var/log`. Use `zcat`, `zgrep` or `zless` to read the compressed ones.

        ## Writing to the log yourself

        Scripts can log too: `logger "backup finished"` sends a message to syslog and the journal, tagged with your user name. In a systemd service, anything printed to standard output ends up in the journal automatically, so often all a program needs to do is print.

        ## A debugging routine

        When a service misbehaves: run `systemctl status` for the quick view, then `journalctl -u name -n 100` for its own story, then widen out with `journalctl -p err -b` and `dmesg -T` to see whether something else on the system went wrong at the same time. Matching timestamps across logs is often how the real cause shows itself.
        """);
}
