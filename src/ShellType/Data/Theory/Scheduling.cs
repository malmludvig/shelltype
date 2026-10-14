using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Scheduling = new(
        "scheduling",
        "Scheduled jobs: cron and timers",
        "processes",
        "Running things automatically: crontab syntax, the environment cron gives you, at, and systemd timers.",
        """
        ## Letting the machine do it

        Backups at night, log cleanup every week, a report every Monday morning, renewing certificates: much of system administration is doing the same thing on a schedule. Linux has two main ways to do that, the classic **cron** and the newer **systemd timers**.

        ## cron and the crontab

        **cron** is a daemon that wakes up every minute and checks whether any scheduled job is due. Each user has their own job list, the **crontab**, which you edit with:

        ```
        crontab -e        # edit your jobs
        crontab -l        # list them
        sudo crontab -u www-data -l   # another user's jobs
        ```

        Each line has five time fields followed by the command:

        ```
        # min  hour  day-of-month  month  day-of-week  command
          0    3     *             *      *            /home/alice/backup.sh
          */15 *     *             *      *            /usr/local/bin/check-site
          30   7     *             *      1-5          /usr/local/bin/report
          0    0     1             *      *            /usr/local/bin/monthly-cleanup
        ```

        Reading them: the first runs at 03:00 every day. The second runs every 15 minutes (`*/15` means "every 15"). The third at 07:30 Monday to Friday (day-of-week 0 or 7 is Sunday). The fourth at midnight on the first day of each month. `*` means "every", commas give lists (`1,15`) and dashes give ranges (`9-17`). Many crons also understand shortcuts like `@daily`, `@hourly` and `@reboot`.

        System-wide jobs live in `/etc/crontab` and `/etc/cron.d/`, where lines have an extra field for the **user** to run as. There are also the folders `/etc/cron.daily/`, `/etc/cron.weekly/` and so on: drop an executable script in, and it runs on that schedule.

        ## Why cron jobs fail when the script works by hand

        This is the most common cron problem by far, and it almost always comes down to the **environment**. cron runs jobs with a minimal environment:

        - `PATH` is short, often just `/usr/bin:/bin`, so commands in `/usr/local/bin` or `/snap/bin` are "not found". Use full paths, or set `PATH=` at the top of the crontab.
        - Your shell startup files (`.bashrc`, `.profile`) are **not** read, so variables you set there do not exist.
        - The working directory is your home directory, not the script's directory. Use absolute paths or `cd` first.
        - `%` has a special meaning in crontab lines (newline) and must be escaped as `\%`, a classic trap with `date +%F`.
        - There is no terminal, so anything that asks for input hangs or fails.

        Output from a job is mailed to the user if mail is set up, and otherwise often lost. Redirect it to a log so you can see what happened:

        ```
        0 3 * * * /home/alice/backup.sh >> /home/alice/backup.log 2>&1
        ```

        To check whether cron even started your job, look at the log: `grep CRON /var/log/syslog` or `journalctl -u cron`.

        ## at: run once, later

        For a one-time job, `at` is simpler than cron:

        ```
        echo "/usr/local/bin/deploy.sh" | at 23:00
        at now + 30 minutes      # then type commands, end with Ctrl+D
        atq                      # list pending jobs
        atrm 3                   # remove job 3
        ```

        ## systemd timers

        On systemd-based distributions, **timers** are a modern alternative. A timer is a unit file that activates a matching service unit on a schedule:

        ```
        # /etc/systemd/system/backup.timer
        [Timer]
        OnCalendar=*-*-* 03:00:00
        Persistent=true

        [Install]
        WantedBy=timers.target
        ```

        Paired with `backup.service`, which says what to run, you enable it with `systemctl enable --now backup.timer`. Advantages over cron: output goes to the journal automatically (`journalctl -u backup`), jobs can depend on other services, resource limits apply, `Persistent=true` catches up on runs missed while the machine was off, and `systemctl list-timers` shows when everything last ran and will run next.

        ## Good habits

        - Make scheduled scripts safe to run twice, and use a lock (`flock`) so a slow run does not overlap the next one.
        - Log what they do, and alert when they fail; a backup job that silently stopped working months ago is a nasty surprise.
        - Test the exact command with `env -i /bin/sh -c '...'` to mimic cron's bare environment.
        """);
}
