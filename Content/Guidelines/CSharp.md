---
pack: C#
optional: true
---

Rules for C# code, on top of the [General guidelines](General.md), which come first.

### Prefer X over Y
- Prefer doc comments laid out as three lines (`/// <summary>`, `/// The text.`, `/// </summary>`) over `/// <summary>The text.</summary>`.
- Prefer one type per file in production code, over several types in a file, unless they're very small and closely related. Specs are the exception: a spec file holds the classes of one namespace, each a fragment of a proposition.
