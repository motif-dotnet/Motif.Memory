namespace Motif.Memory;

/// <summary>
/// How choosing where to save went.
/// </summary>
public abstract record Choosing
{
    private Choosing()
    {
    }

    public sealed record Chosen : Choosing;

    /// <summary>
    /// The user backed out, such as by closing the picker: what was chosen
    /// before, if anything, is kept.
    /// </summary>
    public sealed record Kept : Choosing;

    /// <summary>
    /// Where the user chose can't hold the memory, so nothing is chosen.
    /// </summary>
    public sealed record Refused(string Title, string Problem) : Choosing;
}
