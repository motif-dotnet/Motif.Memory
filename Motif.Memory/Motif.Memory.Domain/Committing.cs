namespace Motif.Memory;

/// <summary>
/// When the memory's own changes are committed.
/// </summary>
public enum Committing
{
    /// <summary>
    /// Once the user has agreed them: one commit per set of edits.
    /// </summary>
    Agreed,

    /// <summary>
    /// Only when the user asks.
    /// </summary>
    Asked,

    /// <summary>
    /// Never: the Developer commits.
    /// </summary>
    Never,
}
