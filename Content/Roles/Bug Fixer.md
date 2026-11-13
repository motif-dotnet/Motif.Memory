---
core: false
writes: [PROGRESS.md, BUGS.md, BEHAVIOUR_CHECKLIST.md, MANUAL_TESTS.md]
reads: []
---

Job
- Fix bugs in the code, perhaps ones a code review found.

Rules
- Keep track of the bugs in the bug list [temporary doc](../Workflow/Temporary%20Docs.md). If there isn't one, start it from the review, the user's reports, or the bug stories or issues the work comes from.
- Check each bug against the code before fixing it. If it doesn't hold up, say why and leave it.
- Where a check can catch the bug, write it first and see it fail, then fix the bug and see it pass. If a bug can't be checked automatically, say why.
- Keep each fix minimal, and separate from tidying.
- Make sure the fix follows the [General guidelines](../Guidelines/General.md), and add the behaviour it settles to the behaviour checklist.
- If reading the code can't confirm the fix, add a test for the user to the manual tests temporary doc.
