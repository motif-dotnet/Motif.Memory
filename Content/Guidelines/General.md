---
pack: General
optional: false
---

These apply in every language. Also read the guidelines for the language you're writing, when there's a note for it<!-- if C# -->: [C#](CSharp.md)<!-- end -->.<!-- if Motif --> Read the [Motif guidelines](Motif.md) when designing types, naming things or writing checks.<!-- end --> Put a rule here unless it only makes sense in one language.

### Hard Rules
<!-- if BEHAVIOUR_CHECKLIST.md -->
- Keep every behaviour in the work's behaviour checklist ([temporary doc](../Workflow/Temporary%20Docs.md)) working.
<!-- end -->
- Run a dev server, or anything else that keeps running, as a background task of your session, and tell the user it stops when the session ends.

### Prefer X over Y
- Prefer the shortest real word that's clear in context (`next`, `min`, `big`) over cryptic names (`t`, `p`, `ret`) or over-spelt compounds (`nextIndex`, `minValue`). One letter only for loop indices.
- Prefer names in the domain's words, and finish a rename across the area it touches: fields, parameters, locals and comments.
- Prefer comments that say why over comments that repeat the code, and plain-word descriptions of behaviour over naming the code (variables, methods, types), so they don't rot when names change. When rewording a comment, first ask whether it's needed at all, and remove it if not.
- Prefer judging an API from its call site in real use over designing it in the abstract. Show example calling code before a design is agreed: in the message when weighing one choice, but once it's real code, such as specs, write it in the codebase for the user to look at in their IDE. Leave a name open until real use shows how it reads.
- Prefer fixing why something is odd over patching the oddity. When something breaks a rule or the surrounding pattern (an out-of-place comment, say), ask why it's there, and keep asking until the cause is tidy.
- Prefer defining a piece of knowledge once, as soon as code repeats it. When an object is only hinted at and has one user, prefer a comment saying what it would be and what should show its shape, over extracting it before a second user arrives.
- Prefer lines of up to 80 columns over longer ones, in code and in comments. Go over only when breaking the line would read worse, and never past 100. Prose in docs is exempt.
- Prefer breaking a long chain where a bulleted list of what it says would, so each line reads as one phrase, over one call per line.

### Prefer
- Minimal, readable, tidy code and docs. In docs, say each fact once, in the doc it belongs in, and link to it from the others.

### Avoid
- Changes to lines the change doesn't otherwise touch, such as whitespace or line endings. If the user's own edits have some, point them out and offer to revert just those lines.
- An empty reference (`null`) as a meaningful value (for example, passing one to mean "hide") when we control the design. Redesign so it can't be empty. An empty string is fine where it reads naturally, like no folder name.
