namespace Motif.Memory;

/// <param name="Steps">What to do, ending with telling the agent.</param>
/// <param name="Tell">The sentence to tell the agent.</param>
public sealed record SavedMemory(string Title, string Steps, string Tell)
{
    public static readonly SavedMemory None = new("", "", "");
}
