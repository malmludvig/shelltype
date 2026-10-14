using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic SpecialPermissions = new(
        "special-permissions",
        "setuid, setgid, the sticky bit, umask and ACLs",
        "users & permissions",
        "The less common permission bits that explain how passwd works, why /tmp is safe, and where new files get their mode.",
        """
        ## Beyond rwx

        The nine `rwx` bits cover most needs, but Linux has three more permission bits and a couple of related mechanisms. You will not set them every day, but you will see them, and several important parts of the system depend on them.

        ## setuid: run as the file's owner

        Normally a program runs with the rights of the user who started it. If a program file has the **setuid** bit, it runs with the rights of the file's **owner** instead. The classic example is `passwd`:

        ```
        ls -l /usr/bin/passwd
        -rwsr-xr-x 1 root root 68208 /usr/bin/passwd
        ```

        See the `s` where the owner's `x` would be? `passwd` is owned by root and has setuid, so when you run it, it runs as root. That is how a normal user can change their own password even though the password hashes in `/etc/shadow` are only writable by root. The program itself is written carefully to only let you change **your own** password.

        setuid programs are powerful and therefore a favorite target for attackers: a bug in one can give away root. Keep them few. You can list them with `find / -perm -4000 -type f 2>/dev/null`. The kernel ignores setuid on shell scripts for safety.

        ## setgid: run as the group, or share a group

        **setgid** on a program works the same way but for the group. On a **directory** it does something more useful in daily work: new files created inside inherit the **directory's group** instead of the creator's primary group. That makes shared project folders work:

        ```
        sudo chgrp devs /srv/project
        sudo chmod 2775 /srv/project     # 2 = setgid, 775 = rwxrwxr-x
        ls -ld /srv/project
        drwxrwsr-x 2 root devs 4096 /srv/project
        ```

        Now every file anyone in `devs` creates there belongs to `devs`, so the whole team can work on it.

        ## The sticky bit: only delete your own files

        As the permissions chapter explained, anyone who can write to a directory can delete any file in it. That would be a disaster for `/tmp`, which everybody writes to. The **sticky bit** on a directory fixes it: files in it can only be deleted or renamed by their owner, the directory's owner or root.

        ```
        ls -ld /tmp
        drwxrwxrwt 18 root root 4096 /tmp
        ```

        The `t` at the end is the sticky bit.

        ## Numbers for the special bits

        The special bits form a **fourth** octal digit in front of the usual three: **4 = setuid, 2 = setgid, 1 = sticky**. So `chmod 4755` sets setuid, `chmod 2775` setgid and `chmod 1777` gives `/tmp`'s mode. In `ls -l`, a capital `S` or `T` means the special bit is set but the matching `x` is not, which is usually a mistake.

        ## umask: default permissions for new files

        Why does a new file get mode 644 and a new directory 755? Programs ask for 666 (files) or 777 (directories), and then the **umask** removes bits. The umask is a mask of permissions to take away, and the usual value is `022`, which removes write for group and others:

        ```
        umask          # 0022
        touch a; mkdir b; ls -ld a b
        -rw-r--r-- a   # 666 minus 022
        drwxr-xr-x b   # 777 minus 022
        ```

        A umask of `077` makes everything you create private to you. It is set per process, usually in your shell startup files, and services can have their own (`UMask=` in a systemd unit).

        ## ACLs: when owner, group and others are not enough

        Sometimes you need to give one extra user access to one file without changing its group. **Access control lists** do that:

        ```
        setfacl -m u:bob:rw report.csv      # give bob read and write
        getfacl report.csv                  # show all entries
        setfacl -x u:bob report.csv         # remove bob's entry
        ```

        A `+` at the end of the mode in `ls -l` (`-rw-rw-r--+`) tells you a file has an ACL, so the nine letters are not the whole story.

        ## Attributes

        Finally, ext4 and some other filesystems support **attributes** set with `chattr`. The most famous is `+i`, **immutable**: the file cannot be changed, renamed or deleted, not even by root, until the attribute is removed. `lsattr` shows them. It is a good thing to know when a file mysteriously refuses to be deleted as root.
        """);
}
