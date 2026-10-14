using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic EverythingIsAFile = new(
        "everything-is-a-file",
        "Everything is a file",
        "files",
        "The Unix idea that disks, terminals, processes and even randomness can be read and written like files.",
        """
        ## The big idea

        One of the oldest and most useful ideas in Unix, and therefore in Linux, is that **almost everything is a file**. Not just documents and programs, but also disks, keyboards, terminals, network connections, information about running processes and settings inside the kernel. They all show up as names in the directory tree, and you work with them using the same few operations: **open, read, write, close**.

        Why does that matter? Because it means one small set of tools works on everything. `cat` was written to print text files, yet it can also print the kernel's view of your CPU (`cat /proc/cpuinfo`). `dd` copies files, and because a disk is a file, it can also copy a whole disk. A program that writes to a file can write to a terminal or a printer without knowing the difference.

        ## The kinds of files

        When you run `ls -l`, the very first character of each line tells you the file type:

        - `-` a **regular file**: text, images, programs, anything with content.
        - `d` a **directory**: a list of names that point to other files.
        - `l` a **symbolic link**: a pointer to another path.
        - `c` a **character device**: hardware you talk to as a stream of bytes, such as a terminal (`/dev/tty`) or `/dev/null`.
        - `b` a **block device**: hardware that stores data in blocks, such as a disk (`/dev/sda`) or a partition (`/dev/sda1`).
        - `p` a **named pipe** (FIFO): a channel one process writes into and another reads from.
        - `s` a **socket**: an endpoint for communication between processes, like `/run/docker.sock`.

        ```
        ls -l /dev/sda /dev/null /dev/tty
        brw-rw---- 1 root disk 8, 0 Oct 14 09:12 /dev/sda
        crw-rw-rw- 1 root root 1, 3 Oct 14 09:12 /dev/null
        crw-rw-rw- 1 root tty  5, 0 Oct 14 09:12 /dev/tty
        ```

        ## Special files worth knowing

        A few device files are so useful they show up in scripts everywhere:

        - `/dev/null` throws away everything written to it and returns nothing when read. `command > /dev/null` silences output.
        - `/dev/zero` returns an endless stream of zero bytes, handy for creating empty files of a set size.
        - `/dev/random` and `/dev/urandom` return random bytes from the kernel.
        - `/dev/stdin`, `/dev/stdout` and `/dev/stderr` are the current process's standard streams.

        ```
        head -c 16 /dev/urandom | base64      # 16 random bytes as text
        dd if=/dev/zero of=empty.img bs=1M count=100   # a 100 MB file of zeros
        ```

        ## Virtual files: /proc and /sys

        Some directories contain files that do not exist on any disk at all. The kernel creates them on the fly when you read them. `/proc` has a directory for every running process, named after its PID, plus system-wide information like `/proc/meminfo` and `/proc/loadavg`. `/sys` exposes devices and drivers. Writing to some of these files changes the kernel's behavior immediately:

        ```
        cat /proc/loadavg                       # current load averages
        cat /proc/self/status | head            # info about the cat process itself
        echo 1 | sudo tee /proc/sys/net/ipv4/ip_forward   # turn on IP forwarding
        ```

        ## File descriptors

        When a program opens a file, the kernel hands it a small number called a **file descriptor**. Every process starts with three already open: **0** is standard input, **1** standard output and **2** standard error. That is why redirections look like `2>/dev/null`: "send descriptor 2 to the null device". You can see any process's open descriptors in `/proc/<pid>/fd`, and `lsof` lists them for the whole system.

        ## Where the idea stops

        Not everything is a file. Network interfaces like `eth0` have no entry in `/dev`, and you configure them with `ip` instead. Processes are controlled with signals, not by writing to them. Still, the idea goes remarkably far, and once it clicks, many Linux tricks stop looking like magic: they are just reading and writing files that happen to be something more interesting than text.
        """);
}
