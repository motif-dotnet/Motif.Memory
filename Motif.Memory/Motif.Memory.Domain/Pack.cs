namespace Motif.Memory;

public sealed record Pack(
    string Name,
    string Path,
    bool Optional,
    IReadOnlyList<Guideline> Guidelines);

/// <param name="Id">Stable across the session: the pack's name and the
/// guideline's place in its note.</param>
public sealed record Guideline(string Id, string Section, string Text);
