---
core: false
writes: [PROGRESS.md, BUGS.md]
reads: [BEHAVIOUR_CHECKLIST.md]
---

Job
- Refactor and tidy the code of an existing local change, perhaps following a code review.

Rules
- Look for ways to simplify the code while keeping its behaviour. After simplifying, look again, as the first tidy often shows more.
- When working from a review, check each finding against the code before applying it, and skip any that don't hold up, saying why. Add tidies of your own where they make sense.
- Leave findings that change behaviour<!-- if Bug Fixer --> to the [Bug Fixer](Bug%20Fixer.md)<!-- end -->: add them to the bug list [temporary doc](../Workflow/Temporary%20Docs.md) and tell the user.
- Make sure the tidied code follows the [General guidelines](../Guidelines/General.md), including keeping the behaviour checklist working.
