---
when: workflow
---

Temporary docs are working notes for a piece of work. They all live in one folder at the repo root, away from the repo's real docs, and the folder is deleted when the work is finished. They never reach the main branch's history.

- **On a branch:** the folder is `_TEMP_DO_NOT_MERGE/<branch>/`, committed on the branch, and deleted before merging. If any of it turns up in a merge, that's a mistake. Each doc starts with:

  > Temporary notes. **Do not merge onto `<base branch>`.** Delete before merging the branch.

- **Straight on the main branch:** the folder is `_TEMP_DO_NOT_MERGE/<name of the work>/`, listed in `.git/info/exclude` so it's never committed. Each doc starts with:

  > Temporary notes. **Never commit.** Delete when the work is finished.

If the doc already exists, add to it rather than starting another. In a .NET repo on a branch, also list the docs in a `_TEMP_DO_NOT_MERGE` solution folder in the main solution, so the user can see them, and remove the solution folder when deleting the docs. Not straight on the main branch: the solution file is committed, so the folder would be too.

They're deleted, so anything a future reader of the code needs, such as why a bug was left alone, also goes in a comment by the code.

- Progress (`PROGRESS.md`): what's been done, what's next, open questions for the user and their answers once decided, and the commit the work started from. Each role adds its own finished part; the Coordinator keeps the whole picture current.
<!-- if BEHAVIOUR_CHECKLIST.md -->
- Behaviour checklist (`BEHAVIOUR_CHECKLIST.md`): how the work should behave, as numbered points from the user's point of view: what the user was happy with after their first round of testing, or what they want, when it's written first. The numbers run on across sections, so each item can be pointed to. It must keep working through tidying, refactoring and bug fixing, unless the user redefines it.
<!-- end -->
<!-- if CODE_REVIEW.md -->
- Code review (`CODE_REVIEW.md`): the findings, most severe first, then a "Checked and OK" section.
<!-- end -->
<!-- if BUGS.md -->
- Bug list (`BUGS.md`): one numbered item per bug, saying where it is and how it shows up. Strike a fixed bug through (`~~...~~`) and add a line saying how it was fixed. If a bug is left alone, say why. Never delete items.
<!-- end -->
<!-- if MANUAL_TESTS.md -->
- Manual tests (`MANUAL_TESTS.md`): tests for the user, for anything an agent can't run itself. Each is a checkbox with the steps, what should happen, and the bug, change or behaviour checklist items it covers. Say what each test is checking and what a failure would look like, and make sure its steps can be done in the software. The Tester ticks the ones it passes; the user ticks the rest.
<!-- end -->
