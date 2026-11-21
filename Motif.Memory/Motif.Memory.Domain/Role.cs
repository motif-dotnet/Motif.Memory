namespace Motif.Memory;

/// <summary>
/// A role the agent can play, with its role facts from the front matter of
/// its note.
/// </summary>
/// <param name="Line">When to use it: its line in the start note, or its Job
/// if it has none there.</param>
public sealed record Role(
    string Name,
    string Path,
    bool Core,
    IReadOnlyList<string> Writes,
    IReadOnlyList<string> Reads,
    string Line)
{
    /// <summary>
    /// A role that isn't core and hands on through temporary docs, so it
    /// brings in the workflow.
    /// </summary>
    public bool InWorkflow => !Core && (Writes.Count > 0 || Reads.Count > 0);
}
