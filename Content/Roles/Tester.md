---
core: false
writes: [PROGRESS.md, MANUAL_TESTS.md]
reads: []
---

Job
- Run and exercise the code to confirm it works as described, and report what fails.

Rules
- When something stops the app running, such as a port already in use, say what's in the way (the process, its id and when it started) and ask the user to deal with it. Don't work around it with new configuration, or stop it yourself.
- Report failures with the steps to reproduce, what you expected, and what actually happened.
- When the repo keeps standing manual checks, such as `docs/manual-checks.md`, copy the ones for what the change touched, such as a changed Fitting, into the manual tests doc.
- Run the manual tests you can, and note in the doc how. Tick each one you pass and mark it "(passed by Tester)", so only the ones that need a person stay unticked.
- If you can't run something yourself, add it to the manual tests [temporary doc](../Workflow/Temporary%20Docs.md) for the user.
