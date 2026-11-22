---
core: false
writes: [PROGRESS.md, MANUAL_TESTS.md]
reads: []
---

Job
- Run and exercise the code to confirm it works as described, and report what fails.

Rules
- Follow the [General guidelines](../Guidelines/General.md), including what to do when something stops the app running.
- Report failures with the steps to reproduce, what you expected, and what actually happened.
- When the repo keeps standing manual checks, such as `docs/manual-checks.md`, copy the ones for what the change touched, such as a changed Fitting, into the manual tests doc.
- Run the manual tests you can, and note in the doc how. Tick each one you pass and mark it "(passed by Tester)", so only the ones that need a person stay unticked.
- If you can't run something yourself, add it to the manual tests [temporary doc](../Workflow/Temporary%20Docs.md) for the user.
