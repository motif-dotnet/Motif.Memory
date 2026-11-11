# Motif Memory: decisions so far

Motif Memory sets up a git repo of Markdown files as an agent's external memory. The first decisions came from discussing the idea. The rest came from reviewing an example memory that has grown through real use (a private one, so none of its specifics are here), and questioning the first decisions against it.

## The idea

| # | Topic | Decision |
|---|---|---|
| 1 | Name | Shown to people as "Motif Memory". The repo and namespaces use `Motif.Memory`. |
| 2 | What it makes | A starter memory: Markdown files an agent discovers a level at a time. The agent's instructions file (`CLAUDE.md` and the like) holds only a pointer to the starting point. |
| 3 | Mainly setup | The memory grows as it's used, so the tool configures and writes out the first version. It's expected to be a fairly simple app. It sets up better by starting from where a memory ended up after real use, not from a blank design. |
| 4 | Beyond built-in memory | It isn't a copy of an agent's built-in memory. It's a structure you choose, portable across machines, readable by any agent, and kept in git so each change can be reviewed as a diff and its history kept. |
| 5 | Worth more than a template | Setup involves real choices, which is why it's an app and not a template repo: which roles, the order of the workflow's stages, and which design guidelines, down to single ones. |
| 6 | Design guidelines | Called design guidelines, not coding standards. The memory holds only what needs judgement. Anything a formatter or analyser can enforce belongs in the code-style baseline (`.editorconfig`, analysers) that Motif Sketch's solution plan sets up. |
| 7 | Growth | The Memory Auditor is how the memory grows, with the Review Responder proposing rules from the user's own reviews. |
| 8 | Place in the suite | One more tool alongside the others, not the front door to the suite. The tools are pick and mix, so someone may skip ports and adapters, or another Motif tool, and keep those guidelines in their memory instead. |
| 9 | Who it's for | Any user of the Motif suite who chooses it. Opinionated but gentle, as the [Motif principles](https://github.com/motif-dotnet/.github/blob/main/design/ideas.md#principles) say: firm defaults, easy opt-outs. |

## Structure

| # | Topic | Decision |
|---|---|---|
| 10 | Progressive discovery | It comes first. An agent never reads the whole memory at the start of a session, and never digs into detail its task doesn't need. Only the Memory Auditor reads everything. |
| 11 | Discovery rules | The starting point only routes: the role list, each with when to use it, and where to look for repo rules. Every link says when to follow it. A rule goes in the deepest note that every task needing it already reads. |
| 12 | Three layers | The memory (private: roles, workflow, guidelines). A repo note per repo inside it (private: the user's own setup for that repo). The repo memory, a file in each repo's `motif/` folder (shared and committed: what anyone on that repo needs). A lesson goes in the repo memory if anyone on the repo needs it, in the repo note if it's only about the user, their machine or accounts, and in the memory otherwise. A rule for every repo of one owner, such as an organisation, goes in a repo note named after the owner. |
| 13 | Repo memory | The agent creates it the first time a lesson belongs there, linked from `motif/README.md`. If the repo has no `motif/` folder, the agent creates one with the file and a minimal README, and adds the `AGENTS.md` pointer if it's missing. The repo memory never points to anyone's memory. It's a file of its own, not the "In this repo" section of `motif/README.md`: READMEs are mainly for people, the memory mainly for agents. The README may point to it. Other Motif repos keep their "In this repo" sections for now. |
| 14 | Layout | `Start here.md`, `Roles/`, `Guidelines/` (one note per pack), `Workflow/` (`Workflow.md`, `Temporary Docs.md`, written only when workflow roles are chosen) and `Repos/` (repo notes, created by the Memory Editor with the first one, so it's never empty). Names keep their spaces. Links are always relative Markdown, never wikilinks, so any agent can follow them. |
| 15 | Note shapes | Roles: `Job`, then `Rules`. Every other note that holds rules (guidelines, repo and owner notes, repo memory files): a first line saying what it covers and sits on top of, then whichever of `Hard Rules`, `Prefer X over Y`, `Prefer` and `Avoid` it needs, with topic sections only once it grows. Examples are generic. New notes follow their kind's shape. |
| 16 | Temporary docs | Called temporary docs, not change docs: working notes for one piece of work in `_TEMP_DO_NOT_MERGE/`, deleted when it's finished. On a branch they're committed there and deleted before merging. For work straight on the main branch, which is how solo repos often work, they're listed in `.git/info/exclude` and never committed. In a .NET repo they're also listed in a solution folder so they show in the IDE. |
| 17 | The person | The memory's text says "the user", the agent's word. Motif Memory's own code and docs say Developer. |

## Setup choices

| # | Topic | Decision |
|---|---|---|
| 18 | Core | Every memory has the starting point, the Memory Editor and the Memory Auditor. Everything else is a choice. The core plus guidelines is a working memory on its own. |
| 19 | Roles | Roles are the unit of choice, all on by default: Design Griller (before the workflow, with no stage of its own), Feature Implementor, Code Reviewer, Code Tidier, Bug Fixer, Review Responder, Behaviour Documenter, Check Writer, Tester and Coordinator. Each brings its workflow stage and the temporary docs it writes, so the workflow and temporary docs follow from the roles chosen. Every role also works on its own, without the workflow. Where work comes from a story or issue, roles say so generically; how to reach a particular tracker is a note added later. |
| 20 | Workflow order | Stages, the user's own among them, come in a default order and can be moved up or down. The app warns, without stopping you, when a stage would read a temporary doc no earlier stage writes. No loops or branches at setup: those are the Coordinator's judgement. The written workflow stays "the ideal order". |
| 21 | Guideline packs | Guidelines come in packs (General, C#, and the Motif-flavoured ones such as interfaces without an `I` prefix), and single guidelines can be unticked. All on by default. |
| 22 | Languages | The core and roles are language-neutral. Motif is opinionated that C# is the default, so C# is the only language pack at first. Others (VB, F#, HTML, CSS, JavaScript) are grown through use: the Memory Editor creates `Guidelines/<Language>.md` when the first rule for that language comes up. |
| 23 | Committing | No change to the memory without the user's OK, always. When to commit is a choice: once agreed (the default, one commit per set of edits, named after the work it came from, pushed if there's a remote), only when asked, or never (the Developer commits). No branches or pull requests for the memory. |
| 24 | Memory Auditor | Proposes rules from the user's corrections and new roles where none fits; flags rules that are unclear, contradictory, duplicated, unhelpful or no longer true, and broken links and empty sections; harvests temporary docs before they're deleted; checks the discovery rules and the layers. It runs when the user says "audit memory", and the Coordinator suggests one when a piece of work is ready to finish. It isn't a workflow stage. |
| 25 | Obsidian | A tick at setup, off by default. It adds the `.obsidian/` settings that keep links relative Markdown, and the `.gitignore` lines for Obsidian's per-device files. |

## The app

| # | Topic | Decision |
|---|---|---|
| 26 | Writes, doesn't plan | The app writes the memory's files itself, rather than a plan an agent builds from. The memory is prose, not code, and tailoring afterwards is the Memory Editor's and Auditor's job. |
| 27 | Pointer | The app writes only inside the memory folder. At the end it shows the pointer for each agent ticked (Claude Code, Codex via `AGENTS.md`, or just the line), for the Developer to paste. The memory's README repeats the steps, for setting up on another machine. The instructions file holds nothing else, except rules that must apply even when the memory isn't read, such as ones overriding the agent's own defaults. |
| 28 | Setup plus parts | The app never reads an existing memory. It's also a catalogue: any single role or pack can be viewed or downloaded, with its stage and temporary docs, and the Memory Editor merges it in. The memory's README says where the catalogue is. |
| 29 | Form | A Blazor WebAssembly app on GitHub Pages. A `dotnet new` template was considered, but ordering stages, unticking single guidelines, the warnings and the catalogue all need a page. It writes to a folder the Developer picks, or a zip. The browser can't run git, so the last screen gives the next steps. |
| 30 | Content | The role and pack texts are Markdown in `Content/`, shipped with the app and read through the Memory Content port. With everything ticked, `Content/` is itself a complete memory. Improvements found by using it come back as ordinary diffs. |
| 31 | Ordering with Sketch | No clash. The memory is per developer, set up once and outliving every app. Sketch is per app. The repo memory is created when it's needed. |
| 32 | Content first | Write `Content/` first and use it as a real memory on the Motif repos, then build the app around content that's been tried. |

## Still open

- **Content in the plan.** `Content/` isn't a project, so the solution plan can't show it yet. Motif Sketch may one day want non-project folders in a plan.
- **More agents.** Which other agents' instructions files the pointer screen covers, once someone asks.
