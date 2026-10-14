using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic InodesAndLinks = new(
        "inodes-and-links",
        "Inodes, hard links and symlinks",
        "files",
        "How a file's name, its data and its metadata are kept apart, and what that means for links and deleting.",
        """
        ## A file is not its name

        It is natural to think of a file as "a name with some content", but Linux filesystems split that into separate pieces. The content lives in **data blocks** on the disk. Everything else the system knows about the file, its size, owner, permissions, timestamps and where its data blocks are, is stored in a small record called an **inode** (index node). Every inode has a number that is unique within its filesystem.

        And the name? The name lives in the **directory**. A directory is really just a table that maps names to inode numbers. When you open `/home/alice/todo.txt`, the kernel looks up `home` in `/`, `alice` in `home`, `todo.txt` in `alice`, finds an inode number, reads that inode and from there finds the data.

        ```
        ls -i todo.txt          # show the inode number
        stat todo.txt           # show everything the inode holds
        ```

        Notice what is **not** in the inode: the file name. That single fact explains hard links, why moving a file is instant, and why deleting does not always free space.

        ## Hard links

        Since a directory entry is just "name → inode number", nothing stops two entries from pointing to the same inode. That is a **hard link**. `ln original.txt copy.txt` creates a second name for the very same file. There is no "original" and "copy" afterwards; both names are equal. Edit through one name and the change is visible through the other, because there is only one set of data.

        The inode keeps a **link count**: how many names point to it. You can see it in the second column of `ls -l`. Deleting a file with `rm` really means **unlinking** a name. The data is only freed when the link count reaches zero **and** no process still has the file open.

        Hard links have limits. They cannot cross filesystems, because inode numbers are only unique within one filesystem, and you normally cannot hard-link directories, because that could create loops in the tree.

        ## Symbolic links

        A **symbolic link** (symlink, soft link) is different: it is a tiny file of its own whose content is a **path**. When you open it, the kernel follows that path. `ln -s /var/www/site/current current` makes `current` point at the target.

        ```
        ln -s /etc/nginx/sites-available/app.conf /etc/nginx/sites-enabled/
        ls -l /etc/nginx/sites-enabled/
        lrwxrwxrwx 1 root root 34 Oct 14 app.conf -> /etc/nginx/sites-available/app.conf
        ```

        Symlinks can point anywhere: across filesystems, to directories, even to paths that do not exist. If the target is removed, the link is left **dangling** and opening it gives "No such file or directory". Relative symlinks are resolved from the directory the link is in, not from where you are standing, which is a common source of confusion.

        Symlinks are everywhere in practice: enabling nginx sites, choosing which Java or Python version `java` or `python3` means, keeping a `current` link to the active release of an app so a deploy is a single atomic switch.

        ## Consequences you will meet

        - **Moving is instant** within one filesystem. `mv` just adds a new name and removes the old one; the data never moves. Across filesystems it has to copy and delete.
        - **Deleted files can still use space.** If a process has a log file open and you `rm` it, the name is gone but the inode lives on until the process closes it. `lsof +L1` lists such files; restarting the process frees the space.
        - **You can run out of inodes** before you run out of space. A filesystem gets a fixed number of inodes when it is created. Millions of tiny files, like cache or session files, can use them all up, and then you get "No space left on device" even though `df -h` shows free space. `df -i` shows inode usage.
        - **Permissions belong to the inode**, so every hard link to a file shares the same permissions and owner.

        ## Quick reference

        ```
        ln target name        # hard link: another name for the same inode
        ln -s target name     # symbolic link: a pointer to a path
        readlink -f name      # where does this link finally lead?
        find . -xtype l       # find dangling symlinks
        df -i                 # inode usage per filesystem
        ```
        """);
}
