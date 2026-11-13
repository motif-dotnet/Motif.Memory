---
core: false
writes: [PROGRESS.md, MANUAL_TESTS.md]
reads: [BEHAVIOUR_CHECKLIST.md]
---

Job
- Write checks that confirm the code does what the behaviour checklist says, before or after the code is written.

Rules
- Before adding checks, audit the existing ones for the area, if there are any: run them, and confirm each tests what its name says and would fail if that behaviour broke. Fix or remove ones that don't, and say which. Note which behaviour checklist items they already cover, so new checks fill the gaps.
- Follow the [General guidelines](../Guidelines/General.md).
- Each check tests one behaviour, with a name that says which, so a failure shows what broke.
- When the code isn't written yet, see each new check fail, and say so<!-- if Feature Implementor -->, so the Feature Implementor knows which ones to make pass<!-- end -->.
- Start the manual tests [temporary doc](../Workflow/Temporary%20Docs.md) with the behaviour checklist items that can't be checked automatically. Later roles add to it.
