// The Web App's own browser plumbing. Everything here is thin: the logic stays in C#.

// Dragging a stage: some browsers only start a drag that carries data, and the drag itself is tracked in C#.
document.addEventListener("dragstart", e => {
  if (!(e.target instanceof Element) || !e.target.closest(".stage")) return;
  e.dataTransfer.setData("text/plain", "");
  e.dataTransfer.effectAllowed = "move";
});

// Theme: follows the system, with an override kept in this browser only. Shared with the other Motif tools.
export function theme() {
  try { return localStorage.getItem("motif-theme") || "system"; } catch { return "system"; }
}

export function setTheme(value) {
  try { localStorage.setItem("motif-theme", value); } catch { /* storage may be blocked */ }
  if (value === "light" || value === "dark") document.documentElement.setAttribute("data-theme", value);
  else document.documentElement.removeAttribute("data-theme");
}
