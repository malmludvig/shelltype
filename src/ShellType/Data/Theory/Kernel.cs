using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Kernel = new(
        "kernel",
        "The kernel and system calls",
        "system",
        "What the kernel does, the line between kernel space and user space, system calls, modules and kernel parameters.",
        """
        ## Linux is a kernel

        Strictly speaking, **Linux** is only the **kernel**: the core program that runs directly on the hardware. Everything else you use, the shell, `ls`, systemd, your web server, comes from other projects and runs on top of it. A **distribution** like Ubuntu or Fedora bundles the kernel with thousands of those programs into a complete operating system.

        The kernel's job is to share the machine safely between many programs:

        - **Process management**: creating processes, scheduling them onto CPUs, delivering signals.
        - **Memory management**: giving every process its own virtual memory and mapping it onto RAM and swap.
        - **Filesystems**: turning blocks on disks into files and directories.
        - **Device drivers**: talking to disks, network cards, USB devices and graphics hardware.
        - **Networking**: the whole TCP/IP stack, routing and the firewall.
        - **Security**: checking permissions on every access, and isolation features like namespaces and cgroups that containers are built on.

        ## Kernel space and user space

        The CPU has different privilege levels. The kernel runs in **kernel space**, with full access to all memory and hardware. Ordinary programs, even ones running as root, run in **user space**, where they cannot touch hardware or other processes' memory directly. A bug in a user program crashes that program; a bug in the kernel can bring down the whole machine, which is called a **kernel panic**.

        ## System calls

        So how does a user program open a file or send a network packet if it cannot touch the hardware? It asks the kernel, through a **system call** (syscall). There are a few hundred of them, with names like `open`, `read`, `write`, `close`, `fork`, `execve`, `kill`, `socket`, `connect` and `mmap`. Every useful thing a program does ends up as system calls. Library functions like C's `printf` or Python's `open()` are friendly wrappers around them.

        You can watch the system calls a program makes with `strace`. It is one of the best debugging tools there is, because it shows what a program really does regardless of what language it is written in:

        ```
        strace -e trace=openat cat /etc/hostname     # which files does cat open?
        strace -f -p 2214 -e trace=network          # what is this process doing on the network?
        strace -c ls > /dev/null                    # a summary: which syscalls, how many, how long
        ```

        When a program fails with a vague message, `strace` often shows the exact file it could not open or the exact connection that was refused, along with the error code like `ENOENT` (no such file) or `EACCES` (permission denied).

        ## Kernel versions

        `uname -r` prints the running kernel version, for example `6.8.0-45-generic`. Kernels are installed as normal packages, and several versions can sit side by side in `/boot` so you can boot an older one if a new one causes trouble. A new kernel only takes effect after a **reboot**; until then the old one keeps running, which is why security updates for the kernel tell you to restart.

        ## Modules

        Much of the kernel, especially drivers, is built as **loadable modules**: pieces that can be added to and removed from the running kernel without rebooting. They live in `/lib/modules/$(uname -r)/`. Usually the kernel loads the right ones automatically when it detects hardware.

        ```
        lsmod                       # loaded modules
        modinfo e1000e              # details about one module
        sudo modprobe br_netfilter  # load a module and its dependencies
        sudo modprobe -r pcspkr     # unload one
        ```

        Modules to load at boot go in `/etc/modules-load.d/`, and modules to never load (blacklist) in `/etc/modprobe.d/`.

        ## Tuning the kernel: sysctl

        Many kernel behaviors can be adjusted at runtime through **kernel parameters**, exposed as files under `/proc/sys`. The `sysctl` command reads and writes them using dotted names:

        ```
        sysctl net.ipv4.ip_forward              # is IP forwarding on?
        sudo sysctl -w net.ipv4.ip_forward=1    # turn it on now
        sysctl vm.swappiness                    # how eagerly to swap
        sysctl -a | grep somaxconn              # search all parameters
        ```

        Changes made with `sysctl -w` are lost at reboot. To make them permanent, put them in a file in `/etc/sysctl.d/`, such as `/etc/sysctl.d/99-custom.conf`, and apply with `sudo sysctl --system`.

        Boot-time parameters that must be set before the kernel even starts, like `quiet` or `mitigations=off`, are passed by the bootloader instead and can be seen in `/proc/cmdline`.

        Most of the time the kernel is invisible, quietly doing its job. But understanding that every action is a system call, that drivers are modules and that behavior is tunable through `/proc/sys` makes a lot of advanced Linux troubleshooting possible.
        """);
}
