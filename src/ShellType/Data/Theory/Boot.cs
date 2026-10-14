using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Boot = new(
        "boot",
        "The boot process",
        "system",
        "What happens between pressing the power button and the login prompt: firmware, bootloader, kernel, initramfs and init.",
        """
        ## A relay race

        Starting a Linux machine is a relay race. Each stage is small and only knows enough to find and start the next one. When a system will not boot, knowing the stages tells you where it stopped and what to look at.

        ## 1. Firmware: BIOS or UEFI

        When power comes on, the CPU starts running code stored on the motherboard: the **firmware**. Older machines use **BIOS**; nearly everything made in the last decade uses **UEFI**. The firmware tests the hardware (the POST), finds the disks and then looks for something to boot.

        A BIOS reads the first sector of the disk, the Master Boot Record, and runs the tiny program in it. UEFI is smarter: it understands the FAT-formatted **EFI System Partition** (usually mounted at `/boot/efi`) and runs a boot program file from it, such as `shimx64.efi` or `grubx64.efi`. UEFI also supports **Secure Boot**, which only runs bootloaders and kernels signed with trusted keys.

        ## 2. The bootloader: GRUB

        The bootloader's job is to load the Linux kernel into memory and start it. The most common one is **GRUB**. It shows the boot menu where you can pick a kernel version or an older one if a new update broke something, and it passes **kernel parameters** such as which disk holds the root filesystem (`root=UUID=...`) and options like `quiet splash`.

        GRUB's configuration is generated from `/etc/default/grub` and the scripts in `/etc/grub.d/`; after changing them you run `update-grub` (Debian, Ubuntu) or `grub2-mkconfig -o /boot/grub2/grub.cfg` (Fedora, RHEL). Pressing `e` in the GRUB menu lets you edit the parameters for a single boot, which is the classic way into a rescue shell.

        ## 3. The kernel

        The kernel, a file like `/boot/vmlinuz-6.8.0-45-generic`, now takes over. It decompresses itself, sets up memory management and the CPU, detects hardware and starts its drivers. You can read everything it reported during this phase with `dmesg` or `journalctl -k -b`.

        The kernel has a problem, though: it needs drivers to read the disk where the root filesystem lives, but those drivers may be modules stored **on** that disk. Encrypted disks, LVM and RAID add more chicken-and-egg problems.

        ## 4. The initramfs

        The answer is the **initramfs** (initial RAM filesystem), a small compressed archive like `/boot/initrd.img-6.8.0-45-generic`, loaded into memory by the bootloader alongside the kernel. It contains a minimal environment with exactly the modules and tools needed to find and open the real root filesystem: storage drivers, LVM tools, the program that asks for your disk encryption passphrase.

        Once the real root filesystem is found and mounted, the initramfs hands over to it and frees itself. If the root disk cannot be found, you end up in an initramfs emergency shell, often after a kernel update or a changed disk UUID. Regenerating the initramfs (`update-initramfs -u` or `dracut -f`) fixes many of these problems.

        ## 5. init: PID 1

        The kernel now starts the very first process, **PID 1**, called **init**. On almost all modern distributions that is **systemd**. Everything else that runs on the machine is started by it, directly or indirectly.

        systemd reads its unit files and starts services in parallel, respecting their dependencies, until it reaches the **default target**: usually `multi-user.target` on servers (text login, network services) or `graphical.target` on desktops (a display manager and login screen). Mounting filesystems from `/etc/fstab`, bringing up the network, starting sshd, cron and your web server all happen in this phase.

        ## 6. Login

        Finally a login prompt appears on the console (a `getty` process), a graphical login screen, or simply sshd waiting for connections. Logging in starts your shell, and the race is finished.

        ## Investigating boots

        ```
        systemd-analyze                 # how long firmware, loader, kernel and userspace took
        systemd-analyze blame           # which services were slowest
        systemd-analyze critical-chain  # the chain of units that delayed boot
        journalctl -b                   # everything logged during this boot
        journalctl -b -1 -p err         # errors from the previous boot
        journalctl --list-boots         # all boots the journal remembers
        uname -r                        # which kernel is running now
        ```

        When a machine does not come up, find the last stage that worked. No GRUB menu means a firmware or bootloader problem. A kernel panic or an initramfs shell means the kernel could not get to the root filesystem. Hanging on a "start job" or landing in emergency mode usually means a service or an `/etc/fstab` entry failed, and `journalctl -xb` will say which.
        """);
}
