using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Memory = new(
        "memory",
        "Memory, swap and the OOM killer",
        "processes",
        "Why free memory looks low, what the page cache and swap do, and what happens when memory really runs out.",
        """
        ## Virtual memory

        Every process on Linux believes it has a large, private range of memory addresses all to itself. That is **virtual memory**. The kernel, together with the CPU's memory management unit, maps those virtual addresses onto real physical RAM in chunks called **pages**, usually 4 KB each. Processes cannot see or touch each other's memory, which is a big part of what makes the system stable and secure.

        Virtual memory also means a process can reserve far more than it uses. A program may allocate 2 GB but only touch 100 MB of it; only the touched pages need real RAM. That is why `ps` shows two numbers: **VSZ**, the virtual size it has reserved, and **RSS** (resident set size), the physical memory it actually occupies. RSS is the one that matters for "who is using my RAM?".

        ## "free" memory and the page cache

        New Linux users often panic at `free -h`:

        ```
        free -h
                       total        used        free      shared  buff/cache   available
        Mem:            15Gi       4.1Gi       512Mi       210Mi        10Gi        10Gi
        Swap:          2.0Gi          0B       2.0Gi
        ```

        Only 512 MB free? That is fine. Linux uses otherwise idle RAM as a **page cache**: copies of recently read files kept in memory so the next read is instant. Empty RAM is wasted RAM. The cache is given back immediately when a program needs memory. The number to watch is **available**, which estimates how much memory programs can still get without swapping. Here that is 10 GB.

        ## Swap

        **Swap** is disk space used as overflow for RAM. When memory gets tight, the kernel moves pages that have not been used for a while out to swap, freeing RAM for active work. If those pages are needed again they are read back, which is slow, because even an SSD is far slower than RAM.

        A little swap use is normal and healthy: idle pages of programs that sit around doing nothing get moved out of the way. Trouble starts when the system **constantly** moves pages in and out, called **thrashing**. Everything becomes painfully slow. `vmstat 1` shows it in the `si` and `so` columns (swap in and out per second); they should be near zero most of the time.

        Swap can be a partition or a file. Adding a swap file to a small server is quick (`fallocate`, `mkswap`, `swapon`), and the `vm.swappiness` setting controls how eagerly the kernel swaps.

        ## Running out: the OOM killer

        What happens when RAM and swap are both full and a process asks for more? The kernel cannot make memory out of nothing, so it invokes the **OOM killer** (out of memory). It picks a process, usually the one using the most memory adjusted by a score, and kills it with SIGKILL to save the system.

        From the outside it looks like a program simply vanished. The evidence is in the kernel log:

        ```
        journalctl -k | grep -i "out of memory"
        dmesg -T | grep -i -E "oom|killed process"
        Out of memory: Killed process 7731 (node) total-vm:4120440kB, anon-rss:3011208kB
        ```

        Containers have their own memory limits; a container killed by the OOM killer typically exits with code **137** (128 + 9, SIGKILL).

        You can influence the choice with `oom_score_adj` (systemd: `OOMScoreAdjust=`), protecting important services and sacrificing less important ones. The real fix, though, is to find out why memory ran out: a leak, a too-large cache setting, too many worker processes, or simply too little RAM for the job.

        ## Investigating memory use

        ```
        free -h                               # the overview
        ps aux --sort=-%mem | head            # biggest processes by memory
        top                                   # then press M to sort by memory
        grep -E "VmRSS|VmSwap" /proc/PID/status   # one process's RAM and swap
        cat /proc/meminfo                     # every detail the kernel has
        vmstat 1 5                            # memory and swap activity over time
        ```

        A process whose RSS grows steadily and never shrinks, hour after hour, is the classic sign of a **memory leak**. Restarting it is a workaround; fixing the code is the cure.

        ## Shared memory and libraries

        Adding up the RSS of all processes often gives more than the RAM you have. That is because pages can be **shared**: every process using the C library maps the same physical copy, and forked children share their parent's pages until one of them writes to a page (**copy-on-write**). This sharing is one reason Linux can run so many processes on modest hardware.
        """);
}
