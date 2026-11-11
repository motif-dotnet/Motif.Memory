These apply in every language. Also read the guidelines for the language you're writing, when there's a note for it: [C#](CSharp.md). Read the [Motif guidelines](Motif.md) when designing types or naming things. Put a rule here unless it only makes sense in one language.

### Hard Rules
- Keep every behaviour in the branch's behaviour checklist ([temporary doc](../Workflow/Temporary%20Docs.md)) working.

### Prefer X over Y
- Prefer the shortest real word that's clear in context (`next`, `min`, `big`) over cryptic names (`t`, `p`, `ret`) or over-spelt compounds (`nextIndex`, `minValue`). One letter only for loop indices.
- Prefer names in the domain's words, and finish a rename across the area it touches: fields, parameters, locals and comments.
- Prefer comments that say why over comments that repeat the code, and plain-word descriptions of behaviour over naming the code (variables, methods, types), so they don't rot when names change. When rewording a comment, first ask whether it's needed at all, and remove it if not.
- Prefer fixing why something is odd over patching the oddity. When something breaks a rule or the surrounding pattern (an out-of-place comment, say), ask why it's there, and keep asking until the cause is tidy.

### Prefer
- Minimal, readable, tidy code and docs. In docs, say each fact once, in the doc it belongs in, and link to it from the others.

### Avoid
- Changes to lines the change doesn't otherwise touch, such as whitespace or line endings. If the user's own edits have some, point them out and offer to revert just those lines.
- An empty reference (`null`) as a meaningful value (for example, passing one to mean "hide") when we control the design. Redesign so it can't be empty. An empty string is fine where it reads naturally, like no folder name.
