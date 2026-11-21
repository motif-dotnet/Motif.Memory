# Manual checks

Every spec runs the real adapters over fake Fittings, so the real Fittings are checked by hand. Run a Fitting's checks before a release when it has changed, in Chrome (which has a folder picker) and in Firefox (which hasn't). If a fake Fitting turns out to behave differently from the real one, tighten the contract in its Seam's doc comments, and fix the fake.

## The ways, in order

- [ ] In Chrome, the Folder section comes first, and at the end **Write to a folder** is the primary button, before **Download zip**. `Program.cs` registers the folder, then the zip, and no spec can see that order (the specs' Fixture builds its own).
- [ ] In Firefox, there's no Folder section, the end says "This browser can't write to a folder.", and **Download zip** is the primary button.

## Folder (`JsInteropFolder`, `JsInteropChosenFolder`, `wwwroot/files.js`)

- [ ] **Pick a folder…** opens the folder picker and asks to edit the folder. An empty folder, or one with only `.git`, shows its name and the write button names it.
- [ ] Closing the picker keeps the folder picked before (declined).
- [ ] Refusing to let the browser edit the folder says "The browser didn't allow editing files in that folder." (refused).
- [ ] A folder with files in it is refused as "isn't empty".
- [ ] Writing writes every file, nested folders included, and the folder is no longer picked.

## Download (`JsInteropDownload`, `wwwroot/download.js`)

- [ ] **Download zip** downloads `agent-memory.zip`, and it unzips to the memory's files at its root.
