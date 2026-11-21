namespace Motif.Memory;

/// <summary>
/// How saving went.
/// </summary>
public abstract record Saving
{
    private Saving()
    {
    }

    /// <param name="Title">What happened, such as "Memory downloaded".</param>
    /// <param name="Steps">What the user does before telling their agent, or
    /// empty.</param>
    public sealed record Saved(string Title, string Steps) : Saving;

    /// <summary>
    /// Nothing was saved, or only some of it.
    /// </summary>
    public sealed record Refused(string Problem) : Saving;
}
