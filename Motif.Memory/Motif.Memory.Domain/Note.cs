namespace Motif.Memory;

/// <summary>
/// A note of Content/, split from its front matter.
/// </summary>
public sealed record Note(string Path, FrontMatter Facts, string Body);
