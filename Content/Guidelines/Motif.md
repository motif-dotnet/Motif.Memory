---
pack: Motif
optional: true
---

Rules for designing types, naming things and writing checks, on top of the [General guidelines](General.md). They match the opinions of the [Motif.NET](https://github.com/motif-dotnet) tools, and hold whether or not a repo uses them.

### Prefer X over Y
- Prefer interface names without the `I` prefix (`Clock`), and implementations named for what makes them different (`SystemClock`, `FixedClock`), over suffixes like `Impl`. Ports and adapters are the exception: a port is named for what the app uses it for (`ForSavingTheMemory`), and its adapters `<Port>Via<Way>` (`ForSavingTheMemoryViaFolder`), so the difference comes after `Via`. A Seam is named for what it reaches, in the app's terms (`Folder`), and its Fittings for their technology (`JsInteropFolder`, `FakeFolder`).
- Prefer deep adapters over thin ones, as Motif.NET's [adapters design note](https://github.com/motif-dotnet/.github/blob/main/design/adapters.md) sets out. An adapter keeps the rules that need the outside world, over a Seam whose Fitting makes one outside call per member, with no branches worth testing. An adapter with no rules of its own is its own Fitting. Reuse a Seam the platform provides when it's small and stable, such as `HttpMessageHandler`.
- Prefer the meaning a UI in another technology would also need behind the User port, where specs reach it, over the same in the view: what's offered and enabled, its wording, and outcomes as one typed value that carries its own wording. The view keeps only its technology's own, such as layout, gestures and a widget's form.
- Prefer callers that see only interfaces, with the concrete class named only where it's created. A factory with shared members (`Clock.System`, `Clock.Fixed(...)`) keeps that to one place.
- Prefer specs that drive the app through the User port, over the real adapters with only their Fittings faked, over tests of single classes or fakes of whole ports. Write an adapter's own specs only for what those can't reach, such as a file format, in a folder per adapter in the specs project. In a repo not yet moved to deep adapters, follow how its existing specs do it, and leave the move to its issue.
- Prefer specs over the User port, plus a few manual checks per real Fitting in the app's `docs/manual-checks.md`, run before a release when that Fitting has changed, for what those specs can reach. Add browser specs for what they can't: layout, the browser's own storage, the order the composition root registers adapters in, and the real Fittings, standing in only for what a check can't drive, such as a folder picker. Keep them fast and steady: a slow or flaky browser spec costs more than it covers.
- Prefer spec bodies that echo the shape and words of the spec's name (`hands_back_what_it_threw` → `var handedBack = …`) over names chosen for the code alone.
- Prefer spec bodies that show what the name claims at the call site (`new List<int> { 3, 4 }` claimed against `new[] { 3, 4 }`) over ones that rely on a declaration or a compiler detail to make the point.
- Prefer reading the repo's `decisions.md`, when it has one, before changing the design, over finding its reasons later. Say if a change goes against a decision, and record new decisions there.
- Prefer the repo's glossary terms, when it has a `motif/glossary.md`, over the words it lists under **Avoid**. When introducing a new concept, add an entry for it, marked *proposed*, in the same change.
