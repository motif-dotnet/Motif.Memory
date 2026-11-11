Rules for designing types and naming things, on top of the [General guidelines](General.md). They match the opinions of the [Motif.NET](https://github.com/motif-dotnet) tools, and hold whether or not a repo uses them.

### Prefer X over Y
- Prefer interface names without the `I` prefix (`Clock`), and implementations named for what makes them different (`SystemClock`, `FixedClock`), over suffixes like `Impl`.
- Prefer callers that see only interfaces, with the concrete class named only where it's created. A factory with shared members (`Clock.System`, `Clock.Fixed(...)`) keeps that to one place.
- Prefer the repo's glossary terms, when it has a `motif/glossary.md`, over the words it lists under **Avoid**. When introducing a new concept, add an entry for it, marked *proposed*, in the same change.
