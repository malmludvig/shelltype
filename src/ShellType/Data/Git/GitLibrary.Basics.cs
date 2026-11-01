using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class GitLibrary
{
    private static readonly CommandEntry[] Setup =
    [
        G("git init", "Turn the current directory into a new, empty repository", Easy),
        G("git init -b main", "Create a new repository whose first branch is called main", Easy),
        G("git clone https://github.com/user/repo.git", "Copy a remote repository, with its full history, over https", Medium),
        G("git clone --depth 1 git@github.com:user/repo.git", "Shallow clone over ssh: only the latest commit, no history", Hard),
        G("git config --global user.name 'Ada Lovelace'", "Set the name recorded as the author of your commits", Medium),
        G("git config --global user.email ada@example.com", "Set the email recorded as the author of your commits", Medium),
        G("git config --global init.defaultBranch main", "Make every new repository start on a branch called main", Hard),
        G("git config --list", "Show every config setting and its value", Easy),
    ];

    private static readonly CommandEntry[] Staging =
    [
        G("git status", "Show what is staged, changed and untracked", Easy),
        G("git status -s", "Short status: one line per file with a two-letter code", Easy),
        G("git add app.py", "Stage one file for the next commit", Easy),
        G("git add .", "Stage every change in the current directory and below", Easy),
        G("git add -A", "Stage every change in the whole repository, deletions included", Easy),
        G("git add -p", "Pick which hunks of your changes to stage, one by one", Medium),
        G("git rm old.txt", "Delete a file and stage the deletion", Easy),
        G("git rm --cached secrets.env", "Stop tracking a file but keep it on disk", Medium),
        G("git mv main.py app.py", "Rename a file and stage the rename", Easy),
    ];

    private static readonly CommandEntry[] Committing =
    [
        G("git commit", "Commit what is staged, writing the message in your editor", Easy),
        G("git commit -m 'Add login form'", "Commit what is staged with a one-line message", Easy),
        G("git commit -am 'Fix typo'", "Stage every tracked, changed file and commit in one go", Medium),
        G("git commit --amend", "Replace the last commit with one that includes what is staged now", Medium),
        G("git commit --amend --no-edit", "Add the staged changes to the last commit and keep its message", Medium),
        G("git commit --allow-empty -m 'Trigger CI'", "Make a commit with no changes, e.g. to rerun a pipeline", Hard),
    ];

    private static readonly CommandEntry[] History =
    [
        G("git log", "Show the commit history of the current branch, newest first", Easy),
        G("git log --oneline", "History with one short line per commit", Easy),
        G("git log --oneline --graph --all", "Draw every branch's history as a graph", Medium),
        G("git log -p app.py", "Show every change ever made to one file, with diffs", Medium),
        G("git log --author=Ada --since='2 weeks ago'", "Commits by one author in the last two weeks", Hard),
        G("git log main..feature", "Commits on feature that are not on main yet", Hard),
        G("git diff", "Show changes that are not staged yet", Easy),
        G("git diff --staged", "Show the changes that are staged for the next commit", Easy),
        G("git diff main...feature", "What feature changed since it branched off main", Hard),
        G("git show HEAD", "Show the last commit: its message and its diff", Easy),
        G("git show HEAD~2:app.py", "Print a file as it was two commits ago", Hard),
        G("git blame app.py", "Show who last changed each line of a file, and in which commit", Easy),
        G("git shortlog -sn", "Count commits per author, most active first", Medium),
    ];

    private static readonly CommandEntry[] Branching =
    [
        G("git branch", "List local branches, with a star on the current one", Easy),
        G("git branch -a", "List local and remote-tracking branches", Easy),
        G("git branch feature", "Create a branch at the current commit without switching to it", Easy),
        G("git switch feature", "Switch to an existing branch", Easy),
        G("git switch -c feature/login", "Create a new branch and switch to it", Easy),
        G("git switch -", "Switch back to the branch you were on before", Medium),
        G("git checkout -b hotfix", "Create and switch to a branch, the older way", Easy),
        G("git branch -m old-name new-name", "Rename a branch", Medium),
        G("git branch -d feature", "Delete a branch that has been merged", Easy),
        G("git branch -D experiment", "Force delete a branch, even if it is not merged", Medium),
        G("git branch --merged", "List the branches already merged into the current one", Medium),
        G("git branch --set-upstream-to=origin/feature", "Make origin/feature the branch the current one pulls from and pushes to", Hard),
    ];
}
