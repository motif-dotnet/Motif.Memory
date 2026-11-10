# Motif Memory: decisions so far

Motif Memory sets up a git repo of Markdown files as an agent's external memory. These decisions come from a first discussion of the idea, before anything was built.

| # | Topic | Decision |
|---|---|---|
| 1 | Name | Shown to people as "Motif Memory". The repo and namespaces use `Motif.Memory`. |
| 2 | What it makes | A stubbed starter repo for an agent's memory: Markdown files an agent discovers a level at a time. The agent's instructions file (`CLAUDE.md` and the like) holds only a pointer to the starting point. |
| 3 | Mainly setup | The memory grows as it's used, so the tool configures and writes out the first version. It's expected to be a fairly simple app. |
| 4 | Beyond built-in memory | It isn't a copy of an agent's built-in memory. It's a structure you choose, portable across machines, readable by any agent, and kept in git so each change can be reviewed as a diff and its history kept. |
| 5 | Worth more than a template | Setup involves real choices, which is why it's an app and not a template repo: the roles the agent plays, the workflow each change goes through (with temporary docs carrying information from stage to stage), and design guidelines to pick and mix. |
| 6 | Design guidelines | Called design guidelines, not coding standards. Pure style lives in `.editorconfig` and analysers. The memory holds only what needs judgement. |
| 7 | Growth | A Memory Auditor role is how the memory grows. Who commits memory changes, and when, could be a setup choice, or left open to develop over time. |
| 8 | Place in the suite | One more tool alongside the others, not the front door to the suite. The tools are pick and mix, so someone may skip ports and adapters, or another Motif tool, and keep those guidelines in their memory instead. |
| 9 | Starting point | It starts from an earlier version of the memory, and perhaps a desktop tool that configures it. Those will be reviewed and sanity-checked, then the tool turned into a web app in the Motif style. |

## Still open

- **Global or per repo.** The memory is mostly global, for agent-assisted development in general, but there may be a per-repo part. Where's the line, and what rule does an agent follow to decide where a new lesson goes without asking each time?
- **What the Memory Auditor does.** Besides adding things, does it prune facts that are no longer true, merge duplicates, split files that get too big, and move lessons out of the temporary change docs before they're thrown away? Does it run as a stage of every change, or only when called?
- **Committing.** Does the agent commit each change at once, leave the diff for you to commit, propose a branch or pull request, or wait for a retro stage at the end of the workflow?
- **Agents.** Which agents' instructions files setup should write the pointer into, besides `CLAUDE.md`.
- **Form.** A Blazor WebAssembly app on GitHub Pages, like the others, is the default.
