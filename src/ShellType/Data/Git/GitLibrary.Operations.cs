using ShellType.Models;

namespace ShellType.Data;

public static partial class GitLibrary
{
    /// <summary>Everyday git workflows as sequences of commands, typed in order in operations mode.</summary>
    public static IReadOnlyList<Operation> Operations { get; } =
    [
        // ---- short: 2 to 4 steps ----
        Op("commit a change", "save a fix you just made as a commit.",
            ("git status", "See what changed before you stage anything"),
            ("git add app.py", "Stage the file you fixed"),
            ("git commit -m 'Fix crash on empty input'", "Commit it with a message that says why")),

        Op("fix the last commit", "you forgot a file in the commit you just made.",
            ("git add forgotten.py", "Stage the missing file"),
            ("git commit --amend --no-edit", "Fold it into the last commit and keep the message"),
            ("git log --oneline -1", "Check that the commit now has everything")),

        Op("unstage a file", "you staged a file by mistake.",
            ("git status -s", "Find the file that should not be staged"),
            ("git restore --staged secrets.env", "Unstage it and keep the file as it is"),
            ("git status -s", "Check that it is no longer staged")),

        Op("undo a pushed commit", "a commit on main broke something and others already pulled it.",
            ("git log --oneline -5", "Find the hash of the bad commit"),
            ("git revert a1b2c3d", "Undo it with a new commit instead of rewriting history"),
            ("git push", "Share the fix")),

        Op("switch with unfinished work", "you need to look at another branch, but your work is not ready to commit.",
            ("git stash -u", "Put your changes aside, untracked files too"),
            ("git switch main", "Go to the other branch with a clean tree"),
            ("git switch -", "Come back when you are done"),
            ("git stash pop", "Pick up where you left off")),

        // ---- medium: 5 to 7 steps ----
        Op("start a new project", "turn a directory into a repository and push it to github.",
            ("git init -b main", "Create the repository with main as the first branch"),
            ("git add .", "Stage every file"),
            ("git commit -m 'Initial commit'", "Make the first commit"),
            ("git remote add origin git@github.com:user/repo.git", "Connect it to the empty github repository"),
            ("git push -u origin main", "Push main and remember origin/main as its upstream")),

        Op("work on a feature branch", "build a feature on its own branch and push it for review.",
            ("git switch main", "Start from main"),
            ("git pull", "Make sure main is up to date"),
            ("git switch -c feature/login", "Create the feature branch"),
            ("git add -A", "Stage your work when it is done"),
            ("git commit -m 'Add login form'", "Commit it"),
            ("git push -u origin feature/login", "Push the branch so you can open a pull request")),

        Op("update a branch with main", "main has moved on and your feature branch is behind.",
            ("git switch main", "Go to main"),
            ("git pull", "Get the latest commits"),
            ("git switch feature/login", "Back to your branch"),
            ("git rebase main", "Replay your commits on top of the new main"),
            ("git push --force-with-lease", "Rebasing rewrote the branch, so overwrite it safely")),

        Op("resolve a merge conflict", "a merge stopped because both branches changed the same lines.",
            ("git merge feature", "The merge that stops with a conflict"),
            ("git status", "See which files have conflicts"),
            ("git diff", "Look at the conflict markers"),
            ("git add app.py", "After fixing the file, stage it to mark it resolved"),
            ("git commit", "Finish the merge with its merge commit"),
            ("git log --oneline --graph -5", "Check that the branches came together")),

        Op("clean up after a merge", "your pull request was merged, so tidy up locally.",
            ("git switch main", "Leave the merged branch"),
            ("git pull", "Get the merge commit"),
            ("git branch -d feature/login", "Delete the local branch, it is merged now"),
            ("git fetch --prune", "Drop remote-tracking branches deleted on github"),
            ("git branch -a", "Check what is left")),

        Op("tag a release", "mark the current commit on main as version 1.1.0.",
            ("git switch main", "Releases are cut from main"),
            ("git pull", "Make sure you tag the latest commit"),
            ("git log --oneline -3", "Check which commit you are tagging"),
            ("git tag -a v1.1.0 -m 'Release 1.1'", "Make an annotated tag with a message"),
            ("git push origin v1.1.0", "Tags are not pushed by default, so push it")),

        // ---- long: 8 to 10 steps ----
        Op("tidy commits before review", "squash your messy work-in-progress commits into clean ones.",
            ("git switch feature/login", "Go to the branch to tidy"),
            ("git fetch", "Get the latest main from the remote"),
            ("git log --oneline origin/main..HEAD", "See your commits that are not on main"),
            ("git rebase -i origin/main", "Squash, reword and reorder them in the editor"),
            ("git status", "If it stops on a conflict, see which files"),
            ("git add app.py", "Fix the conflict and stage the file"),
            ("git rebase --continue", "Carry on with the rest of the rebase"),
            ("git log --oneline origin/main..HEAD", "Check the result"),
            ("git push --force-with-lease", "Update the pull request with the rewritten branch")),

        Op("find the commit that broke it", "the tests passed in v1.0.0 but fail now.",
            ("git bisect start", "Start the search"),
            ("git bisect bad", "The current commit is broken"),
            ("git bisect good v1.0.0", "This release worked"),
            ("git bisect good", "Git checked out a commit in the middle; the tests pass here"),
            ("git bisect bad", "The tests fail at the next one"),
            ("git bisect good", "Pass again, the range keeps halving"),
            ("git show HEAD", "Git has found the first bad commit: read what it changed"),
            ("git bisect reset", "Go back to where you started"),
            ("git revert a1b2c3d", "Undo the culprit with a new commit")),

        Op("recover from a bad reset", "you ran git reset --hard and lost commits you needed.",
            ("git log --oneline -3", "The commits are gone from the branch"),
            ("git reflog", "But HEAD remembers where it has been"),
            ("git show HEAD@{1}", "Check that this is the commit you lost"),
            ("git branch rescue HEAD@{1}", "Put a branch on it so it cannot get lost again"),
            ("git switch rescue", "Look at it"),
            ("git log --oneline -3", "Your commits are back"),
            ("git switch main", "Go back to main"),
            ("git reset --hard rescue", "Move main back to where it was"),
            ("git branch -d rescue", "The rescue branch is not needed any more")),

        Op("hotfix while mid-feature", "production is broken and you are in the middle of a feature.",
            ("git stash push -m 'login wip'", "Put the feature work aside with a name"),
            ("git switch main", "Hotfixes start from main"),
            ("git pull", "Get the code that is in production"),
            ("git switch -c hotfix/crash", "A branch for the fix"),
            ("git commit -am 'Fix crash on start'", "Commit the fix"),
            ("git push -u origin hotfix/crash", "Push it for a quick review"),
            ("git switch feature/login", "Back to the feature"),
            ("git stash list", "Find your stash"),
            ("git stash pop", "And carry on")),
    ];

    private static Operation Op(string name, string goal, params (string Text, string Description)[] steps) =>
        new(name, goal, steps.Select(s => new CommandEntry(s.Text, s.Description, CommandCategory.Git, Difficulty.Medium, Prompt)).ToList());
}
