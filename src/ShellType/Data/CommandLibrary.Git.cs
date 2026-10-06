using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CommandLibrary
{
    private static readonly CommandEntry[] Git = Build(CommandCategory.Git,
        ("git status", "Show the state of the working tree", Easy),
        ("git add .", "Stage every change in the current directory", Easy),
        ("git commit -m 'fix typo'", "Commit staged changes with a message", Medium),
        ("git push origin main", "Push the main branch to origin", Easy),
        ("git pull --rebase", "Fetch and rebase local commits on top", Medium),
        ("git log --oneline --graph", "Compact, graphical commit history", Medium),
        ("git diff --staged", "Show changes that are staged for commit", Medium),
        ("git checkout -b feature/login", "Create and switch to a new branch", Medium),
        ("git switch main", "Switch to the main branch", Easy),
        ("git branch -d old-branch", "Delete a merged branch", Easy),
        ("git stash", "Shelve uncommitted changes", Easy),
        ("git stash pop", "Re-apply the most recent stash", Easy),
        ("git reset --hard HEAD~1", "Throw away the last commit and its changes", Medium),
        ("git restore --staged file.txt", "Unstage a file", Medium),
        ("git clone git@github.com:user/repo.git", "Clone a repository over SSH", Hard),
        ("git remote -v", "List configured remotes", Easy),
        ("git rebase -i HEAD~3", "Interactively rewrite the last three commits", Medium),
        ("git cherry-pick a1b2c3d", "Apply a single commit onto the current branch", Medium),
        ("git blame -L 10,20 app.py", "See who last changed lines 10 to 20", Hard),
        ("git tag -a v1.0.0 -m 'release'", "Create an annotated release tag", Hard));
}
