# Glossary

The words this project uses, and what they mean. People and agents use these terms in code, docs, issues and discussion, and avoid the words listed under **Avoid**. A new concept gets an entry, marked *proposed*, in the same change that introduces it. An entry with no status is agreed.

The format is described in the [Motif Glossary design note](https://github.com/motif-dotnet/.github/blob/main/design/glossary.md). **Sketch** gives the id of the element or port in Motif Memory's own design ([sketch.motifsketch.json](sketch.motifsketch.json)) that the term comes from.

In prose, the names of specific things (roles, ports, actors, storage) take Title Case: the Memory Auditor, the Developer. Other terms stay lower case: lesson, temporary doc. Use italics only when writing about the word itself. A doc for people may link a term to its entry the first time it appears.

Nothing is built yet, so most terms are proposed, and none has an **In code** field.

## Catalogue

The page of Motif Memory where any single role or pack can be viewed or downloaded on its own, with the workflow stage and temporary docs it needs. It's how a part skipped at setup is added later: the Memory Editor merges it in.

- **Kind:** domain concept
- **Avoid:** library, store, marketplace
- **Status:** proposed
- **See also:** [Pack](#pack), [Role](#role)

## Core

The part of every memory that isn't a choice: the starting point, the Memory Editor and the Memory Auditor. On its own, with guidelines, it's a working memory.

- **Kind:** domain concept
- **Avoid:** base, minimum
- **Status:** proposed
- **See also:** [Memory Auditor](#memory-auditor), [Memory Editor](#memory-editor), [Starting point](#starting-point)

## Design guideline

A piece of advice on how to design or write the code, picked from packs at setup and kept in the memory. Only what needs judgement: anything a formatter or analyser can enforce belongs in the code-style baseline that Motif Sketch's solution plan sets up.

- **Kind:** domain concept
- **Avoid:** coding standard, style rule
- **See also:** [Pack](#pack), [Setup](#setup)

## Developer

The person who sets up their agent's memory: they choose the roles, workflow order and design guidelines that suit their way of working.

The memory's own text calls this person "the user", the agent's word for them. That's deliberate: the memory is written for the agent.

- **Kind:** actor
- **Avoid:** user (that's the port, and the memory's word, not ours)
- **Status:** proposed
- **Sketch:** `4d3e0a02`

## Instructions file

The file an agent reads at the start of every session, such as `CLAUDE.md` or `AGENTS.md`. With Motif Memory it holds only the pointer, which the Developer pastes in.

- **Kind:** domain concept
- **Avoid:** config, prompt
- **Status:** proposed
- **See also:** [Pointer](#pointer)

## Lesson

Something learned while working that's worth keeping, such as a correction, a preference, or why a bug was left alone. The Memory Auditor and Review Responder turn lessons into proposed rules. A lesson isn't in the memory until the user agrees a rule for it.

- **Kind:** domain concept
- **Avoid:** learning, insight, finding (that's a code review's), memory (that's the whole thing)
- **Status:** proposed
- **See also:** [Memory Auditor](#memory-auditor), [Rule](#rule)

## Memory

An agent's external memory: Markdown files in a git repo of their own, found a level at a time from the starting point. It grows as it's used, and every change can be reviewed as a diff.

- **Kind:** domain concept
- **Avoid:** knowledge base, notes, context (that's what the agent has in mind in one session)
- **Status:** proposed
- **See also:** [Memory Repo](#memory-repo), [Repo memory](#repo-memory), [Starting point](#starting-point)

## Memory Auditor

The core role that looks after the memory, and how lessons find their way in. It proposes rules from the user's corrections, flags rules that are unclear, duplicated or stale, harvests temporary docs before they're deleted, and checks that discovery and the layers still hold. It runs when asked, and the Coordinator suggests it before a piece of work is finished.

- **Kind:** domain concept
- **Avoid:** curator, librarian
- **Status:** proposed
- **See also:** [Core](#core), [Role](#role)

## Memory Content

The driven port that reads the role and pack texts the memory is made from. Its real adapter reads the Markdown in `Content/`, shipped with the app.

- **Kind:** port
- **Avoid:** templates, resources
- **Status:** proposed
- **Sketch:** `4d3e0c03`
- **See also:** [Catalogue](#catalogue), [Pack](#pack)

## Memory Editor

The core role that changes the memory when the user asks: roles, rules, guidelines and the starting point. It keeps links working, each fact in one place, and new notes in their kind's shape.

- **Kind:** domain concept
- **Avoid:** maintainer
- **Status:** proposed
- **See also:** [Core](#core), [Role](#role)

## Memory Files

The port that writes the starter memory's Markdown files out.

- **Kind:** port
- **Status:** proposed
- **Sketch:** `4d3e0c02`
- **See also:** [Memory Repo](#memory-repo)

## Memory Repo

Where the starter memory goes on the Developer's computer: Markdown files the Developer makes into a git repo of its own.

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

## Pack

A group of design guidelines picked together at setup, such as General or C#. Each guideline in a pack can be unticked, and each pack is one note in the memory's `Guidelines/` folder.

- **Kind:** domain concept
- **Avoid:** bundle, preset, set
- **Status:** proposed
- **See also:** [Design guideline](#design-guideline)

## Pointer

The one thing the agent's instructions file holds: where the memory's starting point is. Motif Memory shows it at the end of setup, and the Developer pastes it in.

- **Kind:** domain concept
- **Avoid:** link, import, include
- **Status:** proposed
- **See also:** [Instructions file](#instructions-file), [Starting point](#starting-point)

## Repo memory

The shared part of the memory for one repo: a file in that repo's `motif/` folder, committed, holding what anyone working on the repo needs. The agent creates it the first time a lesson belongs there. It never points to anyone's memory.

- **Kind:** domain concept
- **Avoid:** project memory, local memory
- **Status:** proposed
- **See also:** [Memory](#memory), [Repo note](#repo-note)

## Repo note

A private note about one repo, or about every repo of one owner such as an organisation, kept in the memory's `Repos/` folder and named after the repo or owner: what only the user needs there, such as their remotes, accounts, boards or machine.

- **Kind:** domain concept
- **Avoid:** codebase note
- **Status:** proposed
- **See also:** [Repo memory](#repo-memory)

## Role

A part the agent plays, such as the Bug Fixer or the Memory Auditor. The Developer ticks which roles their memory has; each brings its workflow stage and the temporary docs it writes.

- **Kind:** domain concept
- **Avoid:** persona, agent (the agent is what plays a role)
- **Status:** proposed
- **See also:** [Core](#core), [Stage](#stage), [Workflow](#workflow)

## Rule

One agreed line in a memory note, such as one of a role's Rules or a design guideline. The Memory Editor writes it, and only with the user's OK.

- **Kind:** domain concept
- **Avoid:** instruction (that's the instructions file), policy
- **Status:** proposed
- **See also:** [Design guideline](#design-guideline) (a kind of rule), [Lesson](#lesson)

## Setup

What the Developer does in Motif Memory: tick the roles, order the workflow stages, pick guideline packs and choose when memory changes are committed, then write the first version of the memory out.

- **Kind:** domain concept
- **Avoid:** wizard, configuration, install
- **Status:** proposed

## Stage

One step in the workflow, played by a role or by the user. Stages hand on to each other through temporary docs.

- **Kind:** domain concept
- **Avoid:** phase, step
- **Status:** proposed
- **See also:** [Temporary doc](#temporary-doc), [Workflow](#workflow)

## Starting point

The first file of the memory, `Start here.md`, the one the pointer names. It only routes: the list of roles, each with when to use it, and where to look for repo rules.

- **Kind:** domain concept
- **Avoid:** index, root, home
- **Status:** proposed
- **See also:** [Memory](#memory), [Pointer](#pointer)

## Temporary doc

A working note for one piece of work, such as the progress doc or the bug list, kept in `_TEMP_DO_NOT_MERGE/` and deleted when the work is finished, after the Memory Auditor has taken any lessons from it. On a branch it's committed there and deleted before merging; straight on the main branch it's never committed.

- **Kind:** domain concept
- **Avoid:** change doc, handoff, ticket, scratch file
- **Status:** proposed
- **See also:** [Stage](#stage), [Workflow](#workflow)

## User port

The driving port people use. Its real adapter is the Web App: the setup page and the catalogue.

- **Kind:** port
- **Avoid:** user, for a person (say Developer)
- **Status:** proposed
- **Sketch:** `4d3e0c01`

## Workflow

The ideal order of stages for a piece of work, on a branch or straight on the main branch, from start to finish, with the roles that play them. Steps loop back or are skipped as the work needs. The Developer orders the stages at setup.

- **Kind:** domain concept
- **Avoid:** process, pipeline
- **Status:** proposed
- **See also:** [Role](#role), [Stage](#stage)
