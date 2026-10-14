using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic FilesystemHierarchy = new(
        "filesystem-hierarchy",
        "The directory structure",
        "files",
        "Why Linux has a single tree that starts at /, and what lives in /etc, /var, /usr and the rest.",
        """
        ## One tree, one root

        Windows gives every disk its own letter: `C:`, `D:` and so on. Linux does it differently. There is exactly **one** directory tree, and it starts at the **root directory**, written as a single slash: `/`. Every file on the system, no matter which disk, USB stick or network share it physically lives on, appears somewhere below `/`. Extra disks are not given letters; they are **mounted** onto a directory in the tree, for example `/mnt/backup` or `/home`.

        The layout of that tree is not random. Most distributions follow the **Filesystem Hierarchy Standard (FHS)**, which says what each top-level directory is for. Once you know the main ones you can find your way around almost any Linux machine, from a Raspberry Pi to a cloud server.

        ## The important directories

        - `/bin` and `/sbin`: essential programs such as `ls`, `cp` and `mount`. `sbin` traditionally holds tools meant for the administrator. On modern systems both are usually links into `/usr`.
        - `/usr`: the bulk of installed software. `/usr/bin` has user programs, `/usr/lib` their libraries and `/usr/share` architecture-independent data like manual pages and icons. Despite the name it has nothing to do with users; think of it as "Unix system resources".
        - `/usr/local`: software you installed yourself, outside the package manager. Keeping it separate means system updates never overwrite it.
        - `/etc`: **configuration**. Almost every system-wide setting is a plain text file in here: `/etc/ssh/sshd_config`, `/etc/fstab`, `/etc/hosts`, `/etc/nginx/`. Backing up `/etc` backs up most of how a machine is set up.
        - `/var`: **variable** data that changes while the system runs. Logs in `/var/log`, package caches in `/var/cache`, mail and print queues in `/var/spool`, databases and service state in `/var/lib`. When a server's disk fills up, `/var` is the first place to look.
        - `/home`: one directory per user, such as `/home/alice`. Your personal files and your dotfiles (`.bashrc`, `.ssh/`, `.config/`) live here. The shortcut `~` means your own home directory.
        - `/root`: the home directory of the root user, kept separate so root can log in even if `/home` is on a disk that failed to mount.
        - `/tmp`: temporary files. Anyone may write here, and the contents are often wiped on reboot.
        - `/boot`: the kernel, the initial ramdisk and the bootloader files needed to start the system.
        - `/dev`: **device files** that represent hardware, like `/dev/sda` for the first disk or `/dev/null`, the bottomless bin.
        - `/proc` and `/sys`: virtual filesystems created by the kernel. They contain no real files on disk; reading them asks the kernel for live information about processes and hardware.
        - `/mnt` and `/media`: mount points. `/mnt` for things the admin mounts by hand, `/media` for removable media mounted automatically.
        - `/opt`: optional, self-contained software packages, often commercial applications that bring everything they need in one folder.
        - `/srv`: data served by this machine, such as websites or FTP files.

        ## Why it is built this way

        The split is about **what changes and who owns it**. Programs in `/usr` change only when you install or upgrade software. Configuration in `/etc` changes when an admin edits it. Data in `/var` changes all the time. Users' files in `/home` belong to the users. Because these concerns are separated, you can put `/var` on a large disk, mount `/usr` read-only, back up `/etc` and `/home` without copying gigabytes of programs, or reinstall the operating system while keeping `/home` untouched.

        ## Finding your way around

        ```
        ls /                 # the top level of the tree
        ls /etc | head       # some configuration files
        du -sh /var/*        # which parts of /var use the most space
        df -h                # which disks are mounted where
        ```

        A useful habit: when you wonder where something lives, ask what kind of thing it is. A program? Look in `/usr/bin`. A setting? `/etc`. A log? `/var/log`. Something a user made? `/home`. That simple question answers most "where is it?" problems on Linux, and the manual page `man hier` describes the whole layout in detail.
        """);
}
