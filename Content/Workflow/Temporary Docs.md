Temporary docs are working notes for a branch. They all live in `_TEMP_DO_NOT_MERGE/<branch>/` at the repo root, away from the repo's real docs. Delete the folder before merging. If any of it turns up in a merge, that's a mistake.

Each doc starts with:

> Temporary notes. **Do not merge onto `<base branch>`.** Delete before merging the branch.

If the doc already exists, add to it rather than starting another. In a .NET repo, also list the docs in a `_TEMP_DO_NOT_MERGE` solution folder in the main solution, so the user can see them, and remove the solution folder when deleting the docs.

They're deleted before merging, so anything a future reader of the code needs, such as why a bug was left alone, also goes in a comment by the code.

- Progress (`PROGRESS.md`): what's been done, what's next, open questions for the user and their answers once decided, and the commit the branch started from. Each role adds its own finished part; the Coordinator keeps the whole picture current.
- Behaviour checklist (`BEHAVIOUR_CHECKLIST.md`): what the user was happy with after their first round of testing, as numbered points from the user's point of view. The numbers run on across sections, so each item can be pointed to. It must keep working through tidying, refactoring and bug fixing, unless the user redefines it.
- Code review (`CODE_REVIEW.md`): the findings, most severe first, then a "Checked and OK" section.
- Bug list (`BUGS.md`): one numbered item per bug, saying where it is and how it shows up. Strike a fixed bug through (`~~...~~`) and add a line saying how it was fixed. If a bug is left alone, say why. Never delete items.
- Manual tests (`MANUAL_TESTS.md`): tests for the user, for anything an agent can't run itself. Each is a checkbox with the steps, what should happen, and the bug, change or behaviour checklist items it covers. Say what each test is checking and what a failure would look like, and make sure its steps can be done in the software. The Tester ticks the ones it passes; the user ticks the rest.
