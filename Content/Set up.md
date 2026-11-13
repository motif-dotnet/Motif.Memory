The user wants this memory set up on this machine. Do these steps in order, then delete this note: it's only for setting up.

1. Tell the user what you're about to do, as the steps below say, and wait for their OK.
2. If this folder isn't a git repo yet, run `git init` in it.
3. Add this line to your own global instructions file, the one you read at the start of every session (for Claude Code, `~/.claude/CLAUDE.md`; for Codex, `~/.codex/AGENTS.md`), with this folder's full path in place of `<path to this repo>`. If the file already points to a memory, ask the user before replacing it.

   ```text
   Before starting any task, read <path to this repo>/Start here.md and follow it.
   ```

4. Delete this note.
5. <!-- if commit: agreed or commit: asked -->Commit every file in the folder as one commit, "Start my agent memory".<!-- end --><!-- if commit: never -->Don't commit: tell the user the memory is ready for its first commit.<!-- end -->
6. Tell the user it's set up, and that from the next session you'll read the memory first. Other agents on this machine need the same line in their own instructions files.
