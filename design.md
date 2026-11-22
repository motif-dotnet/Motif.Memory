# Motif Memory: version 1 design

How the first version is built. The why is in [decisions.md](decisions.md) (#33 to #40 for this version), the words in the [glossary](motif/glossary.md), and the shape of the solution in the [solution plan](motif/plan.md).

## What it does

One page. The Developer chooses where the memory will be saved, such as by picking a folder, where the browser can, makes the setup choices, every one with a default already chosen, then saves the memory one of the ways the browser can use: into that folder or as a zip (#49, #50). A toast then says what to tell their agent, which finishes the setup through the setup note (#42). There's no catalogue yet (#33), and the app never reads an existing memory (#28).

The choices, in the page's order:

| Section | Choice | Default |
|---|---|---|
| Folder | Where the memory will live: shown for each way of saving with somewhere to choose, so only where the browser has a folder picker (#49, #50) | None picked |
| Roles | Tick each non-core role. The core roles (Memory Editor, Memory Auditor) are shown ticked and can't be unticked | All ticked |
| Workflow | Move each stage up or down, the user's stages among them. A warning shows beside a stage that reads a temporary doc no earlier stage writes | Workflow.md's order |
| Guidelines | Untick single guidelines in any pack; leave out whole packs other than General (#36) | Everything ticked |
| Committing | When the memory's changes are committed: once agreed, only when asked, or never (the Developer commits) (#23) | Once agreed |

Then a button for each way of saving the browser can use, in the order registered, the first as the primary: **Write to “*folder*”** (naming the folder picked) and **Download zip** (always). A way that isn't ready says what to do first, and a way the browser can't use is left out, with a line saying why (#50).

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

- **Folder** (#37, #49): picked first, through the browser's folder picker, asking for write access. A folder that holds anything but `.git` is refused as soon as it's picked, and the page says it must be empty or a fresh git repo. Writing checks again, in case it has filled since, and writes nothing if so. Each folder picked is written to once, so writing again means picking again. Cancelling the picker keeps the folder picked before. The app lists the folder's names only, and never opens its files.
- **Zip:** the same files in `agent-memory.zip`, at its root, built in the adapter with `System.IO.Compression`. The browser only saves the bytes.
- Each way is its own adapter at the For saving the memory port, with its own words: its heading, its labels, what it says when it's done or refused (#50).
- Either way the page stays on setup, with every choice kept, and a toast says what to tell the agent: "Read Set up.md in this folder and follow it." For a zip, it says to unzip it first, into a folder that stays put. The toast shows the sentence on a line of its own, with a Copy button under it, and stays until it's dismissed (#45).

## Ports

All in `Motif.Memory.UseCases`, filling the plan's empty interfaces. These are the shapes to aim at; the specs settle the details. Follow the Motif guidelines: no `null` as a meaningful value (an empty string is fine for no problem or no path).

```csharp
/// <summary>
/// A file of Content/, by its path from the folder's root, with forward slashes ("Roles/Bug Fixer.md").
/// </summary>
public sealed record ContentFile(string Path, string Text);

public interface ForReadingTheContent
{
    IReadOnlyList<ContentFile> Files { get; }
}

/// <summary>
/// A file of the memory to write, by its path from the memory's root.
/// </summary>
public sealed record MemoryFile(string Path, string Text);

/// <summary>
/// One way of saving the memory, such as into a folder or as a zip.
/// </summary>
public interface ForSavingTheMemory
{
    Task<string> Unavailable();

    SavingWay Way { get; }

    Task<Choosing> ChooseWhere();

    Task<Saving> Save(IReadOnlyList<MemoryFile> files);
}
```

`Unavailable` says why the browser can't save this way, or nothing when it can. `SavingWay` is what the user sees of the way as it stands: its name, its icon, what saving this way means, its labels for choosing and saving, where was chosen, and what to do before it's ready, if anything; it's ready when there's nothing to do first. `Choosing` says chosen, kept (the picker was closed), or refused (with a title and the problem). `Saving` says saved (with what happened and any steps before telling the agent), or refused (with the problem).

**User port.** `ForSeeing`: the roles (name, its start-note line, ticked, core), the stages in order (each with its warning, or none), the packs and their guidelines (ticked), the committing choice and the choices' labels and lines, the ways of saving offered (and why any aren't), what the last save did and what to tell the agent, the setup note's name, and the problem to show, with its title. `ForDoing`: tick a role, move a stage up or down or to another's place, tick a pack or a guideline, choose committing, choose where to save a way, save it, dismiss the problem.

What a UI in another technology would have to write again is behind the User port, where specs reach it: the wording, labels and outcomes. The page keeps only layout, menus, theme, toasts, busy and drag marks (#51).

The rules that make a memory from the choices and the content (front matter, tags, the workflow's order and warnings) are in `Motif.Memory.Domain`.

## Adapters

| Adapter | Project | How |
|---|---|---|
| Bundled Content | `Motif.Memory.ForReadingTheContent` | Embeds `../../Content/**` as resources, dot-files too, with each file's path kept as its logical name (#40) |
| Folder | `Motif.Memory.ForSavingTheMemory` | Its rules (what counts as empty, writing a folder once, the failures' wording) are C#. It reaches the browser through the Folder Seam (`Folder`), whose real Fitting, `JsInteropFolder`, makes one call to `wwwroot/files.js` per member |
| Zip Download | `Motif.Memory.ForSavingTheMemory` | Makes the zip in C#, and has the browser save it through the Download Seam (`Download`), whose real Fitting, `JsInteropDownload`, calls `wwwroot/download.js` |
| Fake Folder, Fake Download | `Motif.Memory.Fakes` | The fake Fittings, so specs run the real adapters. The picker hands back a folder held in memory, or fails as the Folder Seam's own kinds (declined, refused, failed), which the real Fitting maps from the browser's error names; writing can fail; there can be no folder picker. The download keeps the last one |
| Fake for reading the content | `Motif.Memory.Fakes` | Content a spec makes up, for format rules the real content doesn't use |

The Web App is the composition root (`Program.cs`), so it alone references the real adapters, as Motif Glossary's does. It registers each way of saving as the port, folder then zip, and that order is the order offered (#50). That's one reference more than the plan's §3, which covers the stubs.

## The page

Built on the Motif web kit, as Motif Glossary is: copy `scripts/sync-suite.sh` from it and follow the kit's README (`Motif.NET/brand/web`). The app bar has the `ToolSwitcher` (`Current="memory"`), the `ThemeMenu` and Help. Deploy with Glossary's `pages.yml` and `suite.yml`, changed to this solution. The tool stays `Ready: false` in the kit's `Tools.All` until launch (issue #1, step 6).

- Sections as in the table above, top to bottom, each with a line saying what it's for. Each can be closed, and its heading says what's chosen in it (#48). Orange only for the warnings and the primary button, as the kit's conventions say.
- Workflow: a list with up and down buttons per stage. A stage can also be dragged onto another to take its place (#46). The user's last stage is pinned: it shows "Always last" where the buttons would be (#47). A role unticked above takes its stage out of the list; ticking it again puts it back where it was in the default order.
- Guidelines: each pack with its own tick and the number of guidelines ticked, opening to show the guidelines grouped by section.
- Help: what Motif Memory makes, the three layers, and that nothing leaves the browser.

## Specs

Through the User port, with the real Bundled Content where the real content is the point and the fake for reading the content for the format's own rules. Saving goes through the real adapters, folder then zip, over the fake Fittings. The behaviours to cover:

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
11. A folder that isn't empty is refused when it's picked, with a problem; a cancelled picker keeps the folder picked before; nothing is written before a folder is picked, or to one that has filled since; a folder written must be picked again; a failed write doesn't report an earlier one; a folder written or a zip downloaded says so, and keeps the choices. The folder is offered first, then the zip; a browser with no folder picker is offered only the zip, and told why.
12. The content is valid: every tag is closed and its conditions known, every role note has front matter, and every doc named in front matter has its bullet in `Temporary Docs.md`.

The browser specs (`Checks/Motif.Memory.BrowserSpecs`, decision 54) run the published app in Edge, with the real composition root and the real Fittings, and a stand-in folder picker over the browser's private storage. They check only what the specs above can't:

1. With a folder picker, the folder is offered first, then the zip, as `Program.cs` registers them; with none, only the zip, and the page says why.
2. A picked folder, empty or holding only `.git`, is named; one with a file in it is refused; closing the picker keeps the folder before; refusing editing says so.
3. Writing to a folder writes the same files as the zip holds, nested ones too.
4. Download zip saves `agent-memory.zip`, with the memory at its root.
5. Dragging a stage onto a later one moves it below that, and while dragging, the stage it's over shows where it lands.
6. The toast's Copy puts what to tell the agent on the clipboard.
7. The theme is kept after a reload.
8. At 375, 637 and 1280 px, the app bar and the saving buttons fit, with nothing overlapping, with or without a folder picker, and with a long folder name.

## Building it

1. Mark up `Content/`: front matter and tags, as above.
2. The Domain rules and the ports, test-first through the specs.
3. The adapters.
4. The page, on the web kit.
5. The workflows, then try it in the browser, both ways of writing out.
6. Fill in the glossary's **In code** fields, and mark the terms used in code as agreed with the user. The sketch now says Bundled Content embeds resources: re-export `plan.md` and the diagrams in Motif Sketch, which the build doesn't do by hand.
