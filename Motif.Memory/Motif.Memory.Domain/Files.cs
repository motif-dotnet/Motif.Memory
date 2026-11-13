namespace Motif.Memory;

/// <summary>
/// A file of Content/, by its path from the folder's root, with forward slashes ("Roles/Bug Fixer.md").
/// </summary>
public sealed record ContentFile(string Path, string Text);

/// <summary>
/// A file of the memory to write, by its path from the memory's root.
/// </summary>
public sealed record MemoryFile(string Path, string Text);
