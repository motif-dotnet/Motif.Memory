// The Folder Fitting: one browser call per function, and no rules.
// The rules (what counts as empty, writing once, error wording) are in C#.

export function canPick() {
  return typeof window.showDirectoryPicker === "function";
}

// The folder comes back as a reference C# keeps, with its name.
export function pick() {
  return attempt(async () => {
    const folder = await window.showDirectoryPicker({ id: "motif-memory", mode: "readwrite" });
    return { folder: DotNet.createJSObjectReference(folder), name: folder.name };
  });
}

// Only the folder's names are listed: no file in it is opened.
export function names(folder) {
  return attempt(async () => {
    const names = [];
    for await (const name of folder.keys()) names.push(name);
    return { names };
  });
}

// Writes the text at a path such as "Roles/Tester.md", making its folders.
export function write(folder, path, text) {
  return attempt(async () => {
    const parts = path.split("/");
    let dir = folder;
    for (const part of parts.slice(0, -1)) dir = await dir.getDirectoryHandle(part, { create: true });
    const file = await dir.getFileHandle(parts[parts.length - 1], { create: true });
    const writable = await file.createWritable();
    await writable.write(text);
    await writable.close();
    return {};
  });
}

// Hands any error back as its name and message, which C# throws as a
// FolderException of the Seam's own kind. A DOMException's name, such as
// AbortError, isn't kept when Blazor turns a thrown error into a JSException.
async function attempt(work) {
  try {
    return { error: "", message: "", ...(await work()) };
  } catch (e) {
    console.error("Motif Memory: the browser's folder access failed", e);
    return { error: e?.name || "Error", message: (e?.message || String(e)).trim() };
  }
}
