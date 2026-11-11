Rules for designing types, naming things and writing checks, on top of the [General guidelines](General.md). They match the opinions of the [Motif.NET](https://github.com/motif-dotnet) tools, and hold whether or not a repo uses them.

### Prefer X over Y
- Prefer interface names without the `I` prefix (`Clock`), and implementations named for what makes them different (`SystemClock`, `FixedClock`), over suffixes like `Impl`.
- Prefer callers that see only interfaces, with the concrete class named only where it's created. A factory with shared members (`Clock.System`, `Clock.Fixed(...)`) keeps that to one place.
- Prefer specs that drive the app through a driving port, with fakes for the driven ones, over tests of single classes. Follow how the repo's existing specs do it.
- Prefer reading the repo's `decisions.md`, when it has one, before changing the design, over finding its reasons later. Say if a change goes against a decision, and record new decisions there.
- Prefer the repo's glossary terms, when it has a `motif/glossary.md`, over the words it lists under **Avoid**. When introducing a new concept, add an entry for it, marked *proposed*, in the same change.
