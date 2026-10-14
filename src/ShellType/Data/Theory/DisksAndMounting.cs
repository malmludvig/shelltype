using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic DisksAndMounting = new(
        "disks-and-mounting",
        "Disks, partitions and mounting",
        "files",
        "From a raw disk to files you can use: block devices, partitions, filesystems, mount points and fstab.",
        """
        ## From hardware to files

        Before you can save a file on a new disk, several layers have to be in place. Understanding them makes tools like `lsblk`, `mkfs`, `mount` and `/etc/fstab` much less mysterious.

        ## 1. The block device

        The kernel represents each disk as a **block device** file in `/dev`. SATA and USB disks are usually `/dev/sda`, `/dev/sdb` and so on; NVMe drives look like `/dev/nvme0n1`; virtual machine disks are often `/dev/vda`. Reading `/dev/sda` directly gives you the raw bytes of the whole disk, which is exactly what tools like `dd` do when they write an image to a USB stick.

        ## 2. Partitions

        A disk is normally divided into **partitions**, separate regions that each hold their own filesystem. A **partition table** at the start of the disk describes them. Modern systems use **GPT**; older ones use **MBR**. Partitions get their own device names: `/dev/sda1`, `/dev/sda2`, or `/dev/nvme0n1p1` for NVMe. Tools like `fdisk`, `parted` and `gdisk` edit the partition table.

        Many systems add a layer here: **LVM** (Logical Volume Manager) groups partitions into a pool and carves flexible "logical volumes" out of it that can be resized later, and **LUKS** encrypts a partition so its contents are unreadable without a passphrase.

        ## 3. The filesystem

        A partition is just a range of bytes until you put a **filesystem** on it. The filesystem decides how files, directories, inodes and free space are organized. Common choices:

        - **ext4**: the solid default on many distributions.
        - **xfs**: good with large files and the default on RHEL and Fedora servers.
        - **btrfs**: copy-on-write with snapshots and checksums.
        - **vfat / exfat**: for USB sticks and SD cards that must also work on Windows and macOS.

        `mkfs.ext4 /dev/sdb1` creates an ext4 filesystem, erasing whatever was there. Each filesystem gets a **UUID**, a unique ID you can see with `blkid` or `lsblk -f`.

        ## 4. Mounting

        Linux has one directory tree, so a new filesystem has to be attached to it somewhere. That is called **mounting**, and the directory it is attached to is the **mount point**:

        ```
        sudo mkdir -p /mnt/data
        sudo mount /dev/sdb1 /mnt/data
        ls /mnt/data             # now shows the files on that partition
        sudo umount /mnt/data    # detach it again
        ```

        Anything that was inside `/mnt/data` before mounting is hidden (not deleted) until you unmount. `umount` refuses with "target is busy" if a process still uses a file there, or if your own shell is sitting inside the directory; `lsof +D /mnt/data` or `fuser -m /mnt/data` shows who.

        ## 5. Mounting at boot: /etc/fstab

        Mounts made by hand disappear on reboot. Permanent mounts are listed in `/etc/fstab`, one per line:

        ```
        # device                                  mount point  type  options   dump pass
        UUID=3f1c...-9a2b                         /data        ext4  defaults  0    2
        ```

        Using the **UUID** instead of `/dev/sdb1` matters, because device names can change between boots when disks are added or reordered. After editing fstab, run `sudo mount -a` to mount everything listed; if it reports an error, fix it **before** rebooting, since a broken fstab entry can drop the machine into emergency mode. The `nofail` option tells the system to boot even if that disk is missing.

        ## Seeing the whole picture

        ```
        lsblk -f          # disks, partitions, filesystems, UUIDs and mount points as a tree
        df -h             # how full each mounted filesystem is
        findmnt           # every mount, including virtual ones like /proc
        mount | grep sdb  # the options a filesystem is mounted with
        ```

        ## Keeping filesystems healthy

        A filesystem that was not unmounted cleanly, for example after a power cut, may be checked at boot with `fsck`. If the kernel detects serious errors while running, it may remount the filesystem **read-only** to protect your data, and writes start failing with "Read-only file system". Then the right move is to look at `dmesg`, unmount, run `fsck` on the device and check the disk's own health with `smartctl`.

        Swap space is configured the same way: a partition or file is prepared with `mkswap`, turned on with `swapon` and listed in fstab with the type `swap`.
        """);
}
