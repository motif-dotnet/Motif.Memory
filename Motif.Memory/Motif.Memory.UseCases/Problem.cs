namespace Motif.Memory;

/// <summary>
/// What went wrong with the last command, as what didn't happen (its title)
/// and why, for any UI to show. <see cref="None"/> when all is well.
/// </summary>
public sealed record Problem(string Title, string Text)
{
    public static Problem None { get; } = new("", "");
}
