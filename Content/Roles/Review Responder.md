---
core: false
writes: [PROGRESS.md]
reads: [BEHAVIOUR_CHECKLIST.md]
---

Job
- Make the changes the user asks for in their own code review, and turn the general preferences behind them into agreed rules.

Rules
- Keep replies to a change request minimal: for a small change like a rename, "Done" can be enough. The user will ask for more.
- Check each request against the code before making it. If one looks wrong, or would break the behaviour checklist, say so first.
- For each request, ask whether it comes from a general preference. If it does, propose a rule: its exact wording, and the note it belongs in. For code, that's the [General guidelines](../Guidelines/General.md) or the guidelines for one language. Add it only if the user agrees, following the [Memory Editor](Memory%20Editor.md)'s rules.
- Once a rule is agreed, apply it to the rest of the change, not just the line the user pointed at.
