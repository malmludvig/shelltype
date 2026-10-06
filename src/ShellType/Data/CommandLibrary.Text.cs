using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CommandLibrary
{
    private static readonly CommandEntry[] Text = Build(CommandCategory.Text,
        ("echo hello", "Print text to standard output", Easy),
        ("wc -l file.txt", "Count the lines in a file", Easy),
        ("sort names.txt", "Sort lines alphabetically", Easy),
        ("sort -n numbers.txt", "Sort lines numerically", Easy),
        ("uniq -c", "Collapse repeated lines and count them", Easy),
        ("sort | uniq -c | sort -rn", "Count occurrences and rank them, most frequent first", Medium),
        ("cut -d: -f1 /etc/passwd", "Print the first colon-separated field of each line", Medium),
        ("tr a-z A-Z", "Translate lowercase letters to uppercase", Medium),
        ("sed 's/foo/bar/g' input.txt", "Replace every foo with bar", Medium),
        ("sed -i 's/http:/https:/g' links.txt", "Edit a file in place, upgrading links to https", Hard),
        ("sed -n '10,20p' app.log", "Print only lines 10 through 20", Hard),
        ("awk '{print $1}' access.log", "Print the first column of every line", Medium),
        ("awk -F, '{sum += $3} END {print sum}' sales.csv", "Sum the third column of a CSV file", Hard),
        ("column -t -s, data.csv", "Pretty-print a CSV as an aligned table", Medium),
        ("paste -sd+ nums.txt | bc", "Add up a column of numbers", Hard),
        ("tee output.log", "Write stdin to a file and to stdout", Easy),
        ("rev", "Reverse each line of input", Easy),
        ("jq '.items[].name' data.json", "Extract a field from every item in a JSON array", Hard),
        ("printf '%s\\n' one two three", "Print each argument on its own line", Hard),
        ("nl -ba script.sh", "Number every line of a file", Medium));
}
