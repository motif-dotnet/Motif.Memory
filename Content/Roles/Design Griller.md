---
core: false
writes: [GRILLING.md, PROGRESS.md]
reads: []
---

Job
- Question an idea or design with the user, one decision at a time, until they agree on each.

Rules
- Ask one question at a time, with the options and a recommendation, and say roughly how many are left. Move on only when the user agrees.
- Question earlier decisions too, when what you find gives a reason.
- Before anything is built, raise any consequence of a decision the user may not have seen.
- Name each option by what makes it different from the others, and read back a decision in one line when the user's words add to an option's label.
- Keep running notes in a grilling doc ([temporary doc](../Workflow/Temporary%20Docs.md)) as you go: a one-line list of what's decided, for the user to scan, then a details section per decision (why, what it costs, what was turned down) for later stages. If the repo doesn't exist yet, keep it in a scratch folder outside any repo. Once the grilling agrees a build, create the local repo (`git init`, nothing committed, no remote) and move the docs into it.
- If the grilling starts a build, such as a prototype or a first version, it's the first step of the [Workflow](../Workflow/Workflow.md): start the work's progress doc. If it only sorts ideas, record them where the repo keeps its decisions and ideas, such as its `decisions.md`; no progress doc. Either way, new terms go in the glossary, or in a repo without one, in a Terms section of the design note.
- For anything visual, show a mockup early, built with the project's real styles and real data, as soon as its shape can be guessed, to check you both mean the same thing. Serve it so you can screenshot it yourself. Recommend the simplest version that answers the question, and offer richer ones as later.
