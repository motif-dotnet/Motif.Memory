# Motif Memory: version 1 design

How the first version is built. The why is in [decisions.md](decisions.md) (#33 to #40 for this version), the words in the [glossary](motif/glossary.md), and the shape of the solution in the [solution plan](motif/plan.md).

## What it does

One page. The Developer makes the setup choices, every one with a default already chosen, then writes the memory to a folder or downloads it as a zip. A toast then says what to tell their agent, which finishes the setup through the setup note (#42). There's no catalogue yet (#33), and the app never reads an existing memory (#28).

The choices, in the page's order:

| Section | Choice | Default |
|---|---|---|
| Roles | Tick each non-core role. The core roles (Memory Editor, Memory Auditor) are shown ticked and can't be unticked | All ticked |
| Workflow | Move each stage up or down, the user's stages among them. A warning shows beside a stage that reads a temporary doc no earlier stage writes | Workflow.md's order |
| Guidelines | Untick single guidelines in any pack; leave out whole packs other than General (#36) | Everything ticked |
| Committing | When the memory's changes are committed: once agreed, only when asked, or never (the Developer commits) (#23) | Once agreed |

Then **Write to a folder…** (where the browser has a folder picker) and **Download zip** (always).

## Content format

`Content/` is the app's input, not a memory (#30). Its files are the memory's files, at the paths they're written to, with two additions the app strips out: front matter and tags.

### Front matter

YAML between `---` lines at the very top of a note. Every role note has it (#34):

```yaml
---
core: false
writes: [PROGRESS.md, BUGS.md]
reads: [BEHAVIOUR_CHECKLIST.md]
---
```

- The role's name is its file name. Its start-note line stays in `Start here.md`.
- `writes` and `reads` name temporary docs by file name, as `Temporary Docs.md` lists them. `reads` is only what the role needs an earlier stage to have written, for the workflow warning. A doc a role reads only "if there is one" isn't listed.
- A role is a **workflow role** if it isn't core and writes or reads any temporary doc. Every workflow role writes `PROGRESS.md`.

Every guidelines note has it too:

```yaml
---
pack: C#
optional: true
---
```

`General.md` has `optional: false`. Any other note may have `when:` with a condition (below), and is written only when it holds: the two Workflow notes have `when: workflow`. A note with no front matter is always written.

A first set of role facts, from what the roles' rules say:

| Role | writes | reads |
|---|---|---|
| Design Griller | | |
| Feature Implementor | PROGRESS.md | |
| Behaviour Documenter | PROGRESS.md, BEHAVIOUR_CHECKLIST.md | |
| Check Writer | PROGRESS.md, MANUAL_TESTS.md | BEHAVIOUR_CHECKLIST.md |
| Code Reviewer | PROGRESS.md, BUGS.md, CODE_REVIEW.md | |
| Bug Fixer | PROGRESS.md, BUGS.md, BEHAVIOUR_CHECKLIST.md, MANUAL_TESTS.md | |
| Code Tidier | PROGRESS.md, BUGS.md | BEHAVIOUR_CHECKLIST.md |
| Review Responder | PROGRESS.md | BEHAVIOUR_CHECKLIST.md |
| Tester | PROGRESS.md, MANUAL_TESTS.md | |
| Coordinator | PROGRESS.md | |
| Memory Editor, Memory Auditor | core | |

### Tags

Text written only when a condition holds is wrapped in an HTML comment pair (#35):

```markdown
<!-- if Bug Fixer -->
- Every bug is fixed, or left with a reason the user has agreed to.
<!-- end -->
```

- **Block or inline.** A tag on a line of its own wraps whole lines, and goes with them. A tag inside a line wraps part of it: `Also read [C#](CSharp.md)<!-- if Motif -->, and the [Motif guidelines](Motif.md)<!-- end -->.`
- **Conditions:** a role's name, a pack's name, a temporary doc's file name (true when a chosen role writes it), `workflow` (true when any workflow role is chosen), or `commit: agreed`, `commit: asked`, `commit: never`. Join several with ` and ` or with ` or `, but not both. No nesting.
- An unknown condition, a tag left open or an `end` with no `if` is a content error, which a spec catches.

### Rules without tags

- A list item that starts with a link to a note that isn't written is dropped. That covers the start note's role lines and the Workflow's role stages.
- The Workflow's steps (the numbered list under `### The steps`) are written in the Developer's order and renumbered.
- In a guidelines note, each top-level bullet under a `###` section is one guideline. A section with none left is dropped, heading and all.
- More than one blank line in a row left behind by dropped text closes up to one.

### Tags to add

What the current text needs, besides front matter. A spec (below) finds any that are missed.

| Note | Text | Condition |
|---|---|---|
| `Start here.md` | "When your role's part of a piece of work is done…" | `workflow` |
| `Workflow/Workflow.md` | In step 8: "The [Review Responder]… proposes general rules from them." | `Review Responder` |
| | "Every bug is fixed…" | `BUGS.md` |
| | In "The user has tried it…": the part about manual tests | `MANUAL_TESTS.md` |
| | In it, "passed by the Tester" | `Tester` |
| | "The Tester has walked through the behaviour checklist…" | `Tester and BEHAVIOUR_CHECKLIST.md` |
| `Workflow/Temporary Docs.md` | Each doc's bullet, except Progress | its file name |
| `Guidelines/General.md` | The Hard Rule about the behaviour checklist | `BEHAVIOUR_CHECKLIST.md` |
| | The link to the C# guidelines; the sentence about the Motif guidelines | `C#`; `Motif` |
| `Roles/Code Tidier.md` | "to the [Bug Fixer](Bug%20Fixer.md)" | `Bug Fixer` |
| `Roles/Memory Auditor.md` | The rule about temporary docs | `workflow` |
| `Roles/Memory Editor.md` | The first rule's commit sentence, in three versions: "Commit each agreed set of edits as one commit, named after the work it came from, then push if the memory has a remote." / "Commit only when the user asks: one commit, named after the work it came from, then push if the memory has a remote." / "Don't commit: the user commits the memory themselves." | `commit: agreed` / `asked` / `never` |

New file: `Content/Set up.md`, the setup note (#42), with its commit step tagged `commit: agreed or commit: asked` and `commit: never`.

## Writing out

- **Folder** (#37): the browser's folder picker, asking for write access. If the folder holds anything but `.git`, nothing is written, and the page says the folder must be empty or a fresh git repo. Cancelling the picker changes nothing. The app lists the folder's names only, and never opens its files.
- **Zip:** the same files in `agent-memory.zip`, at its root, built in the adapter with `System.IO.Compression`. The browser only saves the bytes.
- Either way the page stays on setup, with every choice kept, and a toast says what to tell the agent: "Read Set up.md in this folder and follow it." For a zip, it says to unzip it first, into a folder that stays put. The toast shows the sentence on a line of its own, with a Copy button under it, and stays until it's dismissed (#45).

## Ports

All in `Motif.Memory.UseCases`, filling the plan's empty interfaces. These are the shapes to aim at; the specs settle the details. Follow the Motif guidelines: no `null` as a meaningful value (an empty string is fine for no problem or no path).

```csharp
/// <summary>
/// A file of Content/, by its path from the folder's root, with forward slashes ("Roles/Bug Fixer.md").
/// </summary>
public sealed record ContentFile(string Path, string Text);

public interface MemoryContent
{
    IReadOnlyList<ContentFile> Files { get; }
}

/// <summary>
/// A file of the memory to write, by its path from the memory's root.
/// </summary>
public sealed record MemoryFile(string Path, string Text);

public interface MemoryFiles
{
    bool CanPickFolder { get; }

    /// <summary>
    /// Shows the folder picker, then writes the files if the folder is empty or holds only .git.
    /// </summary>
    Task<FolderWrite> WriteToFolder(IReadOnlyList<MemoryFile> files);

    Task DownloadZip(string name, IReadOnlyList<MemoryFile> files);
}
```

`FolderWrite` says written (with the folder's name, for the toast), cancelled, not empty, or failed.

**User port.** `UserQueries`: the roles (name, its start-note line, ticked, core), the stages in order (each with its warning, or none), the packs and their guidelines (ticked), the committing choice, `CanPickFolder`, what the last write did (and the folder's name), the setup note's name, and the problem to show. `UserTasks`: tick a role, move a stage up or down or to another's place, tick a pack or a guideline, choose committing, write to a folder, download the zip, dismiss the problem.

The rules that make a memory from the choices and the content (front matter, tags, the workflow's order and warnings) are in `Motif.Memory.Domain`.

## Adapters

| Adapter | Project | How |
|---|---|---|
| Bundled Content | `Motif.Memory.MemoryContent` | Embeds `../../Content/**` as resources, dot-files too, with each file's path kept as its logical name (#40) |
| Browser Files | `Motif.Memory.MemoryFiles` | A JS module (`wwwroot/files.js`) for `showDirectoryPicker` and saving the zip's bytes; the zip is made in C# |
| Fake Memory Files | `Motif.Memory.Fakes` | Records what was written, and can be set to cancel, find the folder not empty, or have no folder picker |
| Fake Memory Content | `Motif.Memory.Fakes` | Content a spec makes up, for format rules the real content doesn't use |

The Web App is the composition root (`Program.cs`), so it alone references the real adapters, as Motif Glossary's does. That's one reference more than the plan's §3, which covers the stubs.

## The page

Built on the Motif web kit, as Motif Glossary is: copy `scripts/sync-suite.sh` from it and follow the kit's README (`Motif.NET/brand/web`). The app bar has the `ToolSwitcher` (`Current="memory"`), the `ThemeMenu` and Help. Deploy with Glossary's `pages.yml` and `suite.yml`, changed to this solution. The tool stays `Ready: false` in the kit's `Tools.All` until launch (issue #1, step 6).

- Sections as in the table above, top to bottom, each with a line saying what it's for. Each can be closed, and its heading says what's chosen in it (#48). Orange only for the warnings and the primary button, as the kit's conventions say.
- Workflow: a list with up and down buttons per stage. A stage can also be dragged onto another to take its place (#46). The user's last stage is pinned: it shows "Always last" where the buttons would be (#47). A role unticked above takes its stage out of the list; ticking it again puts it back where it was in the default order.
- Guidelines: each pack with its own tick and the number of guidelines ticked, opening to show the guidelines grouped by section.
- Help: what Motif Memory makes, the three layers, and that nothing leaves the browser.

## Specs

Through the User port, with the real Bundled Content where the real content is the point and Fake Memory Content for the format's own rules. The behaviours to cover:

1. The defaults are as in the table above.
2. With everything ticked, every file in `Content/` is written, with no front matter and no tags left in any of them.
3. Core roles can't be unticked.
4. Unticking a role leaves out its note, its start-note line, its stage (with the rest renumbered), the text tagged with it, and any temporary doc no role left writes.
5. With only the core, there's no `Workflow/` folder, and nothing mentions it.
6. No written memory has a broken link: everything ticked, only the core, and each role or optional pack left out on its own.
7. Moving a stage reorders and renumbers the written steps. A stage dropped on another takes its place. The user's last stage stays last, and nothing moves below it. Moving a reader above the writer of its doc shows a warning beside it, and moving it back clears the warning.
8. An unticked guideline isn't written; a section with none left loses its heading; a pack left out loses its file and General's links to it.
9. Each committing choice writes its own version of the Memory Editor's rule.
10. The setup note has the agent add the README's pointer line and delete the note, and its first commit follows the committing choice.
11. A folder that isn't empty gets a problem and no files; a cancelled picker changes nothing, and doesn't report an earlier write; a folder written or a zip downloaded says so, and keeps the choices.
12. The content is valid: every tag is closed and its conditions known, every role note has front matter, and every doc named in front matter has its bullet in `Temporary Docs.md`.

## Building it

1. Mark up `Content/`: front matter and tags, as above.
2. The Domain rules and the ports, test-first through the specs.
3. The adapters.
4. The page, on the web kit.
5. The workflows, then try it in the browser, both ways of writing out.
6. Fill in the glossary's **In code** fields, and mark the terms used in code as agreed with the user. The sketch now says Bundled Content embeds resources: re-export `plan.md` and the diagrams in Motif Sketch, which the build doesn't do by hand.
