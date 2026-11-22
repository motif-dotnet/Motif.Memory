# Manual checks

Every spec runs the real adapters over fake Fittings. The browser specs (`Checks/Motif.Memory.BrowserSpecs`, decision 54) then run the published app in Edge with the real composition root and the real Fittings: the order of the ways, a browser with no folder picker, picking, refusing and writing a folder, the zip download, dragging a stage, the toast's Copy button, the theme and the layout. They stand in for one thing: the folder picker, which hands out folders in the browser's private storage, or fails as the browser does. So what's left by hand is the picker itself, a folder on disk, and the browsers the specs don't run. Run these before a release when a Fitting has changed, in Chrome (which has a folder picker) and in Firefox (which hasn't). If a fake Fitting, or the specs' stand-in picker, turns out to behave differently from the real one, tighten the contract in its Seam's doc comments, and fix the fake.

## The ways, in other browsers

- [ ] In Firefox, there's no Folder section, the end says "This browser can't write to a folder.", and **Download zip** is the primary button. The browser specs check this by removing the picker, so this checks Firefox really has none.

## Folder (`JsInteropFolder`, `JsInteropChosenFolder`, `wwwroot/files.js`)

- [ ] **Pick a folder…** opens the folder picker and asks to edit the folder. An empty folder, or one with only `.git`, shows its name and the write button names it.
- [ ] Closing the picker keeps the folder picked before (declined).
- [ ] Refusing to let the browser edit the folder says "The browser didn't allow editing files in that folder." (refused).
- [ ] Writing writes every file into the folder on disk, nested folders included.

## Download (`JsInteropDownload`, `wwwroot/download.js`)

- [ ] **Download zip** downloads `agent-memory.zip`, and it unzips to the memory's files at its root.
