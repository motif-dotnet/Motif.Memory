namespace Motif.Memory;

/// <summary>
/// One step of the Workflow, played by a role, or by the user when
/// <paramref name="Role"/> is empty.
/// </summary>
/// <param name="Id">Its number in the Workflow's default order.</param>
/// <param name="Text">The step as written, after its number.</param>
/// <param name="Summary">What it's for, as plain text.</param>
public sealed record Stage(int Id, string Role, string Text, string Summary)
{
    public bool IsUsers => Role == "";

    public string Name => IsUsers ? "User" : Role;
}
