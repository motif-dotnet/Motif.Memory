// Browser Files: the folder picker, and saving the zip's bytes. The zip itself is made in C#.

export function canPickFolder() {
  return typeof window.showDirectoryPicker === "function";
}

// Writes each [path, text] into a folder the Developer picks, if it holds nothing but .git.
// Only the folder's names are listed: no file in it is opened.
export async function writeToFolder(files) {
  let folder;
  try {
    folder = await window.showDirectoryPicker({ id: "motif-memory", mode: "readwrite" });
  } catch (e) {
    // Only closing the picker is a cancel. A folder picked but refused write access must say so.
    if (e?.name === "AbortError") return { outcome: "cancelled", name: "", reason: "" };
    console.error("Motif Memory: the folder picker failed", e);
    if (e?.name === "NotAllowedError") return { outcome: "failed", name: "", reason: "The browser didn't allow editing files in that folder." };
    return { outcome: "failed", name: "", reason: message(e) };
  }

  for await (const name of folder.keys()) {
    if (name !== ".git") return { outcome: "notEmpty", name: folder.name, reason: "" };
  }

  let path = "";
  try {
    for (const [p, text] of files) {
      path = p;
      const parts = path.split("/");
      let dir = folder;
      for (const part of parts.slice(0, -1)) dir = await dir.getDirectoryHandle(part, { create: true });
      const file = await dir.getFileHandle(parts[parts.length - 1], { create: true });
      const writable = await file.createWritable();
      await writable.write(text);
      await writable.close();
    }
  } catch (e) {
    console.error(`Motif Memory: writing ${path} failed`, e);
    return { outcome: "failed", name: folder.name, reason: `${path}: ${message(e)}` };
  }
  return { outcome: "written", name: folder.name, reason: "" };
}

function message(e) {
  const text = (e?.message || String(e)).trim();
  return /[.!?]$/.test(text) ? text : text + ".";
}

export function save(name, bytes) {
  const url = URL.createObjectURL(new Blob([bytes], { type: "application/zip" }));
  const a = document.createElement("a");
  a.href = url;
  a.download = name;
  document.body.appendChild(a);
  a.click();
  a.remove();
  setTimeout(() => URL.revokeObjectURL(url), 10000);
}
