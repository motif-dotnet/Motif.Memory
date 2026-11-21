namespace Motif.Memory;

/// <param name="Id">Its number in the default order, which stays the same
/// when it's moved.</param>
/// <param name="Name">The role that plays it, or "User".</param>
/// <param name="Warning">Why it may not work where it is, or empty.</param>
/// <param name="Pinned">It stays last, and nothing moves below it.</param>
public sealed record StageChoice(
    int Id,
    string Name,
    string Summary,
    bool IsUsers,
    string Warning,
    bool Pinned,
    bool CanMoveUp,
    bool CanMoveDown);
