using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Systemd = new(
        "systemd",
        "systemd and services",
        "system",
        "Units, services and targets: starting, enabling and writing services, and the difference between start and enable.",
        """
        ## What systemd is

        **systemd** is the init system on nearly every major distribution: Debian, Ubuntu, Fedora, RHEL, Arch, openSUSE. As PID 1 it starts everything else at boot, and it keeps managing services while the system runs: it restarts them when they crash, collects their logs, limits their resources and stops them in the right order at shutdown.

        You talk to it mainly through two commands: `systemctl` to control it, and `journalctl` to read the logs it collects.

        ## Units

        Everything systemd manages is a **unit**, described by a small text file. The type is in the file name's suffix:

        - `.service` a program to run, such as `nginx.service` or `ssh.service`.
        - `.socket` a socket that starts a service when someone connects.
        - `.timer` a schedule that starts a service, like cron.
        - `.mount` a mounted filesystem (generated from `/etc/fstab` too).
        - `.target` a group of units that represents a state, like `multi-user.target` or `network-online.target`.

        Unit files shipped by packages live in `/usr/lib/systemd/system/` (or `/lib/systemd/system/`). **Your own** units and overrides go in `/etc/systemd/system/`, which takes precedence. Never edit the files under `/usr/lib`; package updates overwrite them.

        ## start versus enable

        This is the most important distinction to learn:

        - `systemctl start nginx` starts the service **now**. After a reboot it will not be running unless it is also enabled.
        - `systemctl enable nginx` makes it start **automatically at boot**. It does not start it now.
        - `systemctl enable --now nginx` does both.

        The same pair exists in reverse: `stop` and `disable`. And `mask` goes further, making a unit impossible to start at all, even by other units.

        ```
        systemctl status nginx         # running? since when? last log lines
        sudo systemctl restart nginx   # stop and start
        sudo systemctl reload nginx    # re-read config without dropping connections
        systemctl is-active nginx
        systemctl is-enabled nginx
        systemctl --failed             # every unit that failed
        systemctl list-units --type=service
        ```

        ## Reading systemctl status

        `systemctl status` packs a lot into a few lines: whether the unit is **loaded** and enabled, whether it is **active (running)**, **inactive (dead)** or **failed**, the main PID, memory and CPU use, the process tree and the last log lines. When a service fails, its exit code and those log lines are usually the first clue. For the full story, `journalctl -u nginx -n 50` shows the last 50 log lines of that unit.

        ## Writing a service

        Turning your own program into a service takes a few lines:

        ```
        # /etc/systemd/system/myapp.service
        [Unit]
        Description=My web app
        After=network-online.target
        Wants=network-online.target

        [Service]
        User=myapp
        WorkingDirectory=/opt/myapp
        ExecStart=/opt/myapp/bin/server --port 8080
        Environment=NODE_ENV=production
        Restart=on-failure
        RestartSec=5

        [Install]
        WantedBy=multi-user.target
        ```

        Then tell systemd to re-read its files and start it:

        ```
        sudo systemctl daemon-reload
        sudo systemctl enable --now myapp
        journalctl -u myapp -f
        ```

        `[Unit]` describes ordering and dependencies, `[Service]` how to run it and `[Install]` which target pulls it in when enabled. `Restart=on-failure` gives you automatic restarts, and running as a dedicated `User=` follows the principle of least privilege. Note that a service does **not** get your shell environment; variables must be set with `Environment=` or `EnvironmentFile=`, which is a classic reason why something works in your terminal but not as a service.

        ## Changing a shipped unit

        To tweak a unit that came from a package, do not copy and edit it. Use a **drop-in override**:

        ```
        sudo systemctl edit nginx
        ```

        This opens an editor for `/etc/systemd/system/nginx.service.d/override.conf`, where you put only the lines you want to change, for example `[Service]` and `LimitNOFILE=65536`. `systemctl cat nginx` shows the final merged result.

        ## Targets

        Targets replace the old "runlevels". `multi-user.target` is a full system without graphics; `graphical.target` adds a desktop; `rescue.target` is a minimal single-user mode for repairs. `systemctl get-default` shows which target the machine boots into.

        systemd does much more, including logging, timers, network and DNS configuration, time sync and login sessions, but units, `start` versus `enable`, and reading `status` and the journal are what you will use every day.
        """);
}
