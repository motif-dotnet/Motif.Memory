# Glossary

The words this project uses, and what they mean. People and agents use these terms in code, docs, issues and discussion, and avoid the words listed under **Avoid**. A new concept gets an entry, marked *proposed*, in the same change that introduces it. An entry with no status is agreed.

The format is described in the [Motif Glossary design note](https://github.com/motif-dotnet/.github/blob/main/design/glossary.md). **Sketch** gives the id of the element or port in Motif Memory's own design ([sketch.motifsketch.json](sketch.motifsketch.json)) that the term comes from.

Nothing is built yet, so most terms are proposed, and none has an **In code** field.

## Change doc

A temporary doc that carries what one stage of the workflow found or decided on to the next stage, for a single change. It's thrown away once the change is done, after the Memory Auditor has taken any lessons from it.

- **Kind:** domain concept
- **Avoid:** handoff, ticket, scratch file
- **Status:** proposed
- **See also:** [Stage](#stage), [Workflow](#workflow)

## Design guideline

A piece of advice on how to design or write the code, picked and mixed at setup and kept in the memory. Only what needs judgement: anything a formatter or analyser can enforce belongs in `.editorconfig` instead.

- **Kind:** domain concept
- **Avoid:** coding standard, style rule
- **See also:** [Setup](#setup)

## Developer

The person who sets up their agent's memory: they choose the roles, workflow and design guidelines that suit their way of working.

- **Kind:** actor
- **Avoid:** user (that's the port)
- **Status:** proposed
- **Sketch:** `4d3e0a02`

## Instructions file

The file an agent reads at the start of every session, such as `CLAUDE.md` or `AGENTS.md`. With Motif Memory it holds only the pointer.

- **Kind:** domain concept
- **Avoid:** config, prompt
- **Status:** proposed
- **See also:** [Pointer](#pointer)

## Memory

An agent's external memory: Markdown files in a git repo of their own, found a level at a time from the starting point. It grows as it's used, and every change can be reviewed as a diff.

- **Kind:** domain concept
- **Avoid:** knowledge base, notes, context (that's what the agent has in mind in one session)
- **Status:** proposed
- **See also:** [Memory Repo](#memory-repo), [Starting point](#starting-point)

## Memory Auditor

The role that looks after the memory. It's how lessons find their way in. What else it does (pruning, merging) is still open.

- **Kind:** domain concept
- **Avoid:** curator, librarian
- **Status:** proposed
- **See also:** [Role](#role)

## Memory Files

The port that writes the starter memory out: its Markdown files, and the pointer for the agent's instructions file.

- **Kind:** port
- **Status:** proposed
- **Sketch:** `4d3e0c02`
- **See also:** [Memory Repo](#memory-repo)

## Memory Repo

Where the starter memory goes on the Developer's computer: Markdown files to make into a git repo of its own, and the pointer for the agent's instructions file.

- **Kind:** storage
- **Avoid:** memory folder, vault
- **Status:** proposed
- **Sketch:** `4d3e0a03`
- **See also:** [Memory](#memory), [Memory Files](#memory-files)

## Motif Memory

This app: it sets up an agent's external memory, with the roles, workflow and design guidelines you choose. Also the system on its own context diagram.

- **Kind:** system
- **Avoid:** memory tool, Memory app
- **Sketch:** `4d3e0a01`

## Pointer

The one thing the agent's instructions file holds: where the memory's starting point is.

- **Kind:** domain concept
- **Avoid:** link, import, include
- **Status:** proposed
- **See also:** [Instructions file](#instructions-file), [Starting point](#starting-point)

## Role

A part the agent plays in the workflow, such as the Memory Auditor. The Developer picks which roles their memory has.

- **Kind:** domain concept
- **Avoid:** persona, agent (the agent is what plays a role)
- **Status:** proposed
- **See also:** [Memory Auditor](#memory-auditor), [Workflow](#workflow)

## Setup

What the Developer does in Motif Memory: choose the roles, workflow, design guidelines and how memory changes are committed, then write the first version of the memory out.

- **Kind:** domain concept
- **Avoid:** wizard, configuration, install
- **Status:** proposed

## Stage

One step in the workflow, usually played by a role. Each stage hands on to the next through the change doc.

- **Kind:** domain concept
- **Avoid:** phase, step
- **Status:** proposed
- **See also:** [Change doc](#change-doc), [Workflow](#workflow)

## Starting point

The first file of the memory, the one the pointer names. It's short and points onwards, so an agent reads only what the task in hand needs.

- **Kind:** domain concept
- **Avoid:** index, root, home
- **Status:** proposed
- **See also:** [Memory](#memory), [Pointer](#pointer)

## User port

The driving port people use. Its real adapter is the Web App, the setup page.

- **Kind:** port
- **Avoid:** user, for a person (say Developer)
- **Status:** proposed
- **Sketch:** `4d3e0c01`

## Workflow

The stages each change goes through, from start to finish, with the roles that play them. The Developer shapes it at setup.

- **Kind:** domain concept
- **Avoid:** process, pipeline
- **Status:** proposed
- **See also:** [Role](#role), [Stage](#stage)
