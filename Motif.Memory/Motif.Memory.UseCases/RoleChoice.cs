namespace Motif.Memory;

/// <param name="Line">When to use it, as the start note says.</param>
public sealed record RoleChoice(
    string Name, string Line, bool Ticked, bool Core);
