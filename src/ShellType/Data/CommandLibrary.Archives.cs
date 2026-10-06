using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CommandLibrary
{
    private static readonly CommandEntry[] Archives = Build(CommandCategory.Archives,
        ("tar -czf backup.tar.gz project/", "Create a gzip-compressed tarball", Medium),
        ("tar -xzf backup.tar.gz", "Extract a gzip-compressed tarball", Medium),
        ("tar -tf archive.tar", "List the contents of a tarball", Easy),
        ("tar -xJf linux.tar.xz -C /usr/src", "Extract an xz tarball into a directory", Hard),
        ("gzip access.log", "Compress a file with gzip", Easy),
        ("gunzip access.log.gz", "Decompress a gzip file", Easy),
        ("zip -r site.zip public/", "Zip a directory recursively", Medium),
        ("unzip site.zip -d site/", "Unzip into a target directory", Medium),
        ("zcat access.log.gz | grep 500", "Search inside a compressed log without extracting it", Hard),
        ("xz -9 dump.sql", "Compress a file with maximum xz compression", Easy),
        ("7z x archive.7z", "Extract a 7-Zip archive", Easy),
        ("tar --exclude=node_modules -czf app.tgz app/", "Tar a project while skipping node_modules", Hard));
}
