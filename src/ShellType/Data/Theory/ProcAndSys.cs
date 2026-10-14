using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic ProcAndSys = new(
        "proc-and-sys",
        "/proc and /sys",
        "system",
        "The virtual filesystems where the kernel shows its live state: process directories, system information and device settings.",
        """
        ## Files that are not on any disk

        Two directories in the Linux tree contain nothing stored on a disk at all: `/proc` and `/sys`. They are **virtual filesystems**. When you read a file in them, the kernel generates the content on the spot from its current internal state. When you write to some of them, you change kernel settings immediately. They are the clearest example of the "everything is a file" idea, and most system tools, `ps`, `top`, `free`, `lsblk`, are really just friendly readers of these files.

        ## /proc: processes and the system

        **procfs**, mounted at `/proc`, has a directory for every running process, named after its PID. Inside are files describing that process:

        - `/proc/PID/cmdline` the full command line it was started with (arguments separated by zero bytes).
        - `/proc/PID/status` a readable summary: name, state, PPID, user IDs, memory use (`VmRSS`), threads.
        - `/proc/PID/environ` its environment variables.
        - `/proc/PID/cwd` a link to its current working directory.
        - `/proc/PID/exe` a link to the program file it is running.
        - `/proc/PID/fd/` one link per open file descriptor, showing every file and socket it has open.
        - `/proc/PID/limits` its resource limits, like the maximum number of open files.

        ```
        cat /proc/2214/status | grep -E "State|VmRSS|Threads"
        ls -l /proc/2214/fd | wc -l              # how many files it has open
        sudo cat /proc/2214/environ | tr '\0' '\n' # its environment, one per line
        ls -l /proc/2214/cwd                      # where it is running from
        ```

        There is also `/proc/self`, which always points to the process reading it, a neat trick for scripts.

        Besides processes, `/proc` holds system-wide information:

        - `/proc/cpuinfo` details of every CPU core.
        - `/proc/meminfo` detailed memory statistics; `free` reads this.
        - `/proc/loadavg` the load averages `uptime` shows.
        - `/proc/uptime` seconds since boot.
        - `/proc/mounts` currently mounted filesystems.
        - `/proc/cmdline` the parameters the kernel was booted with.
        - `/proc/version` the kernel version and how it was built.

        ## /proc/sys: kernel settings

        Under `/proc/sys` sit the tunable **kernel parameters**. Each one is a small file you can read and, as root, write:

        ```
        cat /proc/sys/net/ipv4/ip_forward              # 0 or 1
        echo 1 | sudo tee /proc/sys/net/ipv4/ip_forward
        cat /proc/sys/fs/file-max                      # system-wide open file limit
        ```

        The `sysctl` command is a front end to exactly these files: `net.ipv4.ip_forward` is `/proc/sys/net/ipv4/ip_forward` with dots instead of slashes. Note that `echo 1 > /proc/...` does not work with plain `sudo echo`, because the redirection happens in your unprivileged shell; that is why `tee` is used.

        ## /sys: devices and drivers

        **sysfs**, mounted at `/sys`, is newer and more structured. It describes the kernel's view of **hardware**: every device, the bus it is attached to, its driver and its settings, one value per file.

        - `/sys/class/net/` one directory per network interface; `/sys/class/net/eth0/statistics/` holds packet and error counters.
        - `/sys/block/` block devices; `/sys/block/sda/queue/scheduler` shows the disk's I/O scheduler.
        - `/sys/class/power_supply/` batteries and chargers on laptops.
        - `/sys/class/thermal/` temperature sensors.
        - `/sys/fs/cgroup/` control groups, which limit the CPU and memory of services and containers.

        ```
        cat /sys/class/net/eth0/address               # the MAC address
        cat /sys/class/net/eth0/operstate             # up or down
        cat /sys/class/power_supply/BAT0/capacity     # battery percentage
        cat /sys/class/thermal/thermal_zone0/temp     # temperature in millidegrees
        ```

        ## Why it is worth knowing

        Most of the time you use higher-level tools, but `/proc` and `/sys` are always there, on every machine, even minimal containers where `ps` or `ip` are not installed. When a tool's output is confusing, you can look at the raw source. And they answer questions tools do not ask directly: which environment did systemd really give my service? What is this process's open-file limit? How many file descriptors does it actually use? Which kernel parameters was this machine booted with? Each answer is one `cat` away.
        """);
}
