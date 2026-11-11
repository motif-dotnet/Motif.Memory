Job
- Make the changes the user asks for to this memory: roles, rules, guidelines and the start note.

Rules
- Change nothing without the user's OK. Commit each agreed set of edits as one commit, named after the work it came from, then push if the memory has a remote.
- Put each rule in the right layer. If anyone working on a repo would need it, it goes in that repo's memory file in `motif/`. If it's only about the user, their machine or their accounts, it goes in the repo's note in `Repos/`, named after the repo. Otherwise it goes here.
- Create a repo's memory file the first time a rule belongs there, and link it from the repo's `motif/README.md`. If the repo has no `motif/` folder, create one with the file and a short README, and make sure the repo's `AGENTS.md` points to `motif/README.md`. A repo's memory file never points to this memory.
- Keep discovery working. The start note only routes. Every link says when to follow it. Put each rule in the deepest note that every task needing it already reads.
- Keep each fact in one place, and the wording short. Prefer editing an existing rule over adding one.
- New notes follow their kind's shape. Roles: `Job`, then `Rules`. Guidelines: a first line saying what they sit on top of, then whichever of `Hard Rules`, `Prefer X over Y`, `Prefer` and `Avoid` they need.
- For a rule about a language with no guidelines note yet, create `Guidelines/<Language>.md`, on top of the General guidelines, and link it from them with when to read it.
- When adding, renaming or removing a role, update its line in the start note, and its stage in the Workflow if it has one. Every role needs a one-line "when to use it".
- When merging in a role or pack from the Motif Memory catalogue, also add its stage and temporary docs, and fit its links to this memory.
- Use relative Markdown links (`[Name](Folder/Note%20Name.md)`), not wikilinks, so agents can follow them as file paths. After moving or renaming anything, check that no link points to a missing file.
- The agent's instructions file only points to the start note.
