using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Permissions = new(
        "permissions",
        "File permissions",
        "users & permissions",
        "Reading rwxr-x---, what read, write and execute mean for files and directories, and chmod in letters and numbers.",
        """
        ## Three questions, three groups of people

        Every file and directory has an **owner**, a **group** and a set of **permission bits**. When a process tries to use a file, the kernel asks: is this process running as the file's owner? If not, is it in the file's group? If not, it counts as **others**. Exactly one of those three classes applies, checked in that order, and then the kernel looks at what that class may do.

        Each class gets three permissions:

        - **r** read
        - **w** write
        - **x** execute

        ## Reading ls -l

        ```
        ls -l
        -rwxr-x--- 1 alice devs 4096 Oct 14 10:02 deploy.sh
        drwxr-xr-x 2 alice devs 4096 Oct 14 10:00 docs
        ```

        Take `-rwxr-x---` apart. The first character is the file type (`-` file, `d` directory). Then come three groups of three:

        - `rwx` for the **owner** (alice): read, write and execute.
        - `r-x` for the **group** (devs): read and execute, no write.
        - `---` for **others**: nothing at all.

        ## What r, w and x mean

        For **files** it is intuitive: read the content, change the content, run it as a program. A script needs both `r` and `x` to be run directly.

        For **directories** the same letters mean something slightly different, and this trips up many people:

        - **r** lets you **list** the names in the directory.
        - **w** lets you **create, delete and rename** entries in it.
        - **x** lets you **enter** it and reach the files inside by name.

        Two consequences follow. First, to read `/srv/data/report.csv` you need `x` on every directory along the path, not just `r` on the file; `namei -l /srv/data/report.csv` shows the permissions of each step. Second, **deleting a file depends on the directory**, not the file. If you can write to a directory you can delete files in it, even read-only files owned by someone else (unless the sticky bit is set, see the next chapter).

        ## chmod with letters

        `chmod` changes permissions. The symbolic form says **who** (`u` user/owner, `g` group, `o` others, `a` all), **how** (`+` add, `-` remove, `=` set exactly) and **what** (`r`, `w`, `x`):

        ```
        chmod +x deploy.sh        # everyone may execute
        chmod u+x,g-w file        # owner gets x, group loses w
        chmod o= secret.txt       # others get nothing
        chmod -R g+rw shared/     # recursively give the group read and write
        ```

        ## chmod with numbers

        The numeric (octal) form is shorter and very common. Each permission has a value, **r = 4, w = 2, x = 1**, and you add them up per class:

        - 7 = rwx, 6 = rw-, 5 = r-x, 4 = r--, 0 = ---

        Three digits give owner, group and others:

        ```
        chmod 755 script.sh     # rwxr-xr-x: typical for programs and directories
        chmod 644 index.html    # rw-r--r--: typical for normal files
        chmod 600 ~/.ssh/id_ed25519   # rw-------: private keys, only you
        chmod 700 ~/.ssh        # rwx------: a private directory
        ```

        Some programs **insist** on strict permissions. ssh refuses to use a private key that others can read, and sshd ignores `authorized_keys` if the file or `~/.ssh` is writable by others. That is a security feature, not a bug.

        ## Owner and group

        `chown` changes the owner and `chgrp` the group; only root may give files away to another user:

        ```
        sudo chown www-data:www-data /var/www/site -R
        sudo chgrp devs report.csv
        ```

        ## Common mistakes

        - `chmod -R 777` "fixes" a permission error by letting **everyone** write and run everything. It hides the real problem and opens a security hole. Find out which user needs access and give exactly that.
        - Forgetting that a web server runs as its own user (`www-data`, `nginx`), so files you own with mode 600 are invisible to it.
        - Using `chmod -R 644` on a tree, which removes `x` from directories and makes them impossible to enter. Use `find dir -type f -exec chmod 644 {} +` for files and `-type d` with 755 for directories.

        When something says **Permission denied**, ask three questions: which user is the process running as (`ps -o user`), who owns the file and its directories (`ls -l`, `namei -l`), and which class of the three applies. The answer is almost always right there.
        """);
}
