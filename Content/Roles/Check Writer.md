Job
- Write checks that confirm the code does what its documented behaviour says.

Rules
- Before adding checks, audit the existing ones for the area: run them, and confirm each tests what its name says and would fail if that behaviour broke. Fix or remove ones that don't, and say which. Note which behaviour checklist items they already cover, so new checks fill the gaps.
- Each check tests one behaviour, with a name that says which, so a failure shows what broke.
- Start the manual tests [temporary doc](../Workflow/Temporary%20Docs.md) with the behaviour checklist items that can't be checked automatically. Later roles add to it.
