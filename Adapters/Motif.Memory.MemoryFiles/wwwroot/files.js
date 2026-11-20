// Browser Files: the folder picker, and saving the zip's bytes. The zip itself is made in C#.

export function canPickFolder() {
  return typeof window.showDirectoryPicker === "function";
}

// The folder the memory will be written to, kept from picking it until it's written to once.
let picked;

// Shows the folder picker, and keeps the folder if it holds nothing but .git.
// Only the folder's names are listed: no file in it is opened.
export async function pickFolder() {
  let folder;
  try {
    folder = await window.showDirectoryPicker({ id: "motif-memory", mode: "readwrite" });
  } catch (e) {
    // Only closing the picker is a cancel, and keeps the folder picked before.
    // A folder picked but refused write access must say so.
    if (e?.name === "AbortError") return { outcome: "cancelled", name: "", reason: "" };
    picked = undefined;
    console.error("Motif Memory: the folder picker failed", e);
    if (e?.name === "NotAllowedError") return { outcome: "failed", name: "", reason: "The browser didn't allow editing files in that folder." };
    return { outcome: "failed", name: "", reason: message(e) };
  }

  picked = undefined;
  if (!(await isEmpty(folder))) return { outcome: "notEmpty", name: folder.name, reason: "" };
  picked = folder;
  return { outcome: "picked", name: folder.name, reason: "" };
}

async function isEmpty(folder) {
  for await (const name of folder.keys()) {
    if (name !== ".git") return false;
  }
  return true;
}

// Writes each [path, text] into the folder picked, if it still holds nothing but .git.
export async function writeToFolder(files) {
  const folder = picked;
  picked = undefined;
  if (!folder) return { outcome: "failed", name: "", reason: "No folder is picked." };
  // It may have filled since it was picked.
  if (!(await isEmpty(folder))) return { outcome: "notEmpty", name: folder.name, reason: "" };

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
