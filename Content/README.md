# Agent memory

An external memory for coding agents, set up with [Motif Memory](https://github.com/motif-dotnet/Motif.Memory). Agents start at [Start here](Start%20here.md) and read only what the task in hand needs. This README is for people: agents don't need it.

## Set it up on a machine

1. Clone this repo somewhere that stays put.
2. Add one line to each agent's instructions file, with the path to where you cloned it:

   ```text
   Before starting any task, read <path to this repo>/Start here.md and follow it.
   ```

   | Agent | Instructions file |
   |---|---|
   | Claude Code | `~/.claude/CLAUDE.md` |
   | Codex, and other agents that read `AGENTS.md` | `~/.codex/AGENTS.md`, or the agent's own global instructions file |

   That line is all the instructions file holds about the memory. Everything else lives here.

## How it grows

- **The agent proposes, you agree.** Nothing in the memory changes without your OK. Say "audit memory" during a piece of work, and the Memory Auditor suggests improvements from what happened.
- **Three layers.** This memory holds what applies everywhere. `Repos/` holds private notes about single repos: your remotes, accounts or machine. Rules anyone working on a repo needs go in that repo's own `motif/` folder instead.
- **Adding a part later.** To add a role or guideline pack you didn't choose at setup, find it in the Motif Memory catalogue, and ask the agent to merge it in.
