using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class GitLibrary
{
    private static readonly CommandEntry[] Merging =
    [
        G("git merge feature", "Merge another branch into the current one", Easy),
        G("git merge --no-ff feature", "Merge with a merge commit, even when a fast-forward is possible", Medium),
        G("git merge --squash feature", "Stage a branch's changes as one change, without committing", Medium),
        G("git merge --abort", "Give up a merge with conflicts and go back to before it", Medium),
        G("git rebase main", "Replay the current branch's commits on top of main", Medium),
        G("git rebase -i HEAD~3", "Interactively reword, squash or drop the last three commits", Medium),
        G("git rebase --continue", "Carry on rebasing after you have fixed a conflict", Medium),
        G("git rebase --abort", "Give up a rebase and go back to before it started", Medium),
        G("git cherry-pick a1b2c3d", "Copy a single commit onto the current branch", Medium),
        G("git cherry-pick a1b2c3d..e4f5a6b", "Copy a range of commits onto the current branch, leaving out the first one", Hard),
        G("git merge -X theirs feature", "Merge and settle every conflict in favor of the other branch", Hard),
        G("git rebase --onto main old-base feature", "Move the commits feature made after old-base so they start from main", Hard),
    ];

    private static readonly CommandEntry[] Remotes =
    [
        G("git remote -v", "List the remotes and their urls", Easy),
        G("git remote add origin git@github.com:user/repo.git", "Connect the repository to a remote called origin", Hard),
        G("git remote set-url origin https://github.com/user/repo.git", "Point an existing remote at a new url", Hard),
        G("git fetch", "Download new commits from the remote without changing your branches", Easy),
        G("git fetch --prune", "Fetch and remove remote-tracking branches deleted on the remote", Medium),
        G("git pull", "Fetch and merge the remote's changes into the current branch", Easy),
        G("git pull --rebase", "Fetch and replay your local commits on top of the remote's", Medium),
        G("git push", "Upload the current branch's commits to its upstream", Easy),
        G("git push -u origin feature", "Push a new branch and make origin/feature its upstream", Medium),
        G("git push --force-with-lease", "Overwrite the remote branch, unless someone else pushed since you fetched", Hard),
        G("git push origin --delete feature", "Delete a branch on the remote", Medium),
    ];

    private static readonly CommandEntry[] Undoing =
    [
        G("git restore app.py", "Throw away the unstaged changes to a file", Easy),
        G("git restore --staged app.py", "Unstage a file but keep its changes", Medium),
        G("git restore --source=HEAD~1 app.py", "Bring back a file as it was one commit ago", Hard),
        G("git reset HEAD~1", "Undo the last commit and keep its changes, unstaged", Medium),
        G("git reset --soft HEAD~1", "Undo the last commit and keep its changes staged", Medium),
        G("git reset --hard HEAD~1", "Undo the last commit and throw its changes away", Medium),
        G("git reset --hard origin/main", "Make the current branch exactly match the remote's main", Hard),
        G("git revert a1b2c3d", "Make a new commit that undoes an earlier one, safe on shared branches", Medium),
        G("git clean -fd", "Delete untracked files and directories", Medium),
        G("git clean -n", "Dry run: list the untracked files clean would delete", Easy),
    ];

    private static readonly CommandEntry[] Stashing =
    [
        G("git stash", "Put uncommitted changes aside and get a clean working tree", Easy),
        G("git stash -u", "Stash untracked files too", Easy),
        G("git stash push -m 'half done login'", "Stash with a message so you can find it later", Medium),
        G("git stash list", "List the stashes you have", Easy),
        G("git stash pop", "Re-apply the latest stash and remove it from the list", Easy),
        G("git stash apply stash@{1}", "Re-apply a specific stash and keep it in the list", Hard),
        G("git stash drop", "Delete the latest stash", Easy),
    ];

    private static readonly CommandEntry[] Tags =
    [
        G("git tag", "List the tags", Easy),
        G("git tag v1.0.0", "Make a lightweight tag at the current commit", Easy),
        G("git tag -a v1.1.0 -m 'Release 1.1'", "Make an annotated tag with a message", Hard),
        G("git push origin v1.1.0", "Push one tag to the remote", Medium),
        G("git push --tags", "Push every tag to the remote", Medium),
        G("git tag -d v1.0.0", "Delete a local tag", Medium),
    ];

    private static readonly CommandEntry[] Advanced =
    [
        G("git reflog", "Every place HEAD has been: the way back after a bad reset or rebase", Medium),
        G("git bisect start", "Start a binary search for the commit that introduced a bug", Medium),
        G("git bisect bad", "Mark the current commit as broken during a bisect", Medium),
        G("git bisect good v1.0.0", "Mark a commit as known to work during a bisect", Hard),
        G("git bisect reset", "End the bisect and go back to where you started", Medium),
        G("git worktree add ../hotfix hotfix", "Check out another branch in a second directory", Hard),
        G("git grep -n TODO", "Search the tracked files for a pattern, with line numbers", Medium),
        G("git submodule update --init --recursive", "Fetch and check out every submodule", Hard),
        G("git switch --detach v1.0.0", "Look at an old commit without being on any branch", Hard),
    ];
}
