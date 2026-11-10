# Motif Memory

An idea for a tool in the **[Motif.NET](https://github.com/motif-dotnet)** suite. It sets up an agent's external memory: a starter git repo of Markdown files, set out so an agent finds what it needs a level at a time. The memory grows as it's used, so the tool's job is the first version: the files, the structure and the roles that suit your way of working.

## The idea

- **The memory lives in a repo of its own.** Your `CLAUDE.md` (or another agent's instructions file) holds only a pointer to the memory's starting point. Everything else lives in the memory repo.
- **Progressively discoverable.** The starting point is short and points onwards, so an agent reads only what the task in hand needs.
- **Why not the agent's built-in memory?** A structure of your own choosing. It's portable across machines, readable by any agent rather than one, and in git, so you can review each change as a diff and see its history.
- **It grows as it's used.** A **Memory Auditor** role looks after it, and that's how lessons find their way in.
- **The tool is mainly for setup.** It configures and writes out the first version of the memory, and is probably quite a simple app.

## What setup would let you choose

- **Roles** the agent can play, such as Memory Auditor.
- **A workflow** that each change goes through, with temporary docs that carry information from one stage to the next.
- **Design guidelines**, to pick and mix. They're called guidelines rather than coding standards because anything a formatter or analyser can enforce belongs in `.editorconfig`, not in memory.
- **How the memory changes**: who commits a change and when. You could also leave this open and let the flow develop over time.

## Status

**Idea.** Nothing is built here yet. There's an earlier version of the memory, and perhaps a desktop tool that configures it. The plan is to review and sanity-check those, then turn the tool into a web app in the Motif style. The decisions and open questions so far are in [decisions.md](decisions.md).

## License

MIT, see [LICENSE](LICENSE).
