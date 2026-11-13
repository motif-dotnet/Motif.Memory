namespace Motif.Memory;

/// <summary>
/// The User port's queries: everything the setup page shows.
/// </summary>
public interface UserQueries
{
    IReadOnlyList<RoleChoice> Roles { get; }

    /// <summary>
    /// The stages of the roles ticked, and the user's, in the Developer's order.
    /// </summary>
    IReadOnlyList<StageChoice> Stages { get; }

    IReadOnlyList<PackChoice> Packs { get; }

    Committing Committing { get; }

    bool CanPickFolder { get; }

    /// <summary>
    /// What the last write command did: written to a folder, downloaded as a zip, or nothing.
    /// </summary>
    Written Written { get; }

    /// <summary>
    /// The name of the folder the memory was last written to, or empty.
    /// </summary>
    string FolderName { get; }

    /// <summary>
    /// The note in the memory that the Developer points their agent at, to finish setting it up.
    /// </summary>
    string SetupNote { get; }

    /// <summary>
    /// What went wrong with the last command, or empty.
    /// </summary>
    string Problem { get; }
}

public enum Written
{
    Not,
    ToFolder,
    AsZip,
}

/// <param name="Line">When to use it, as the start note says.</param>
public sealed record RoleChoice(string Name, string Line, bool Ticked, bool Core);

/// <param name="Id">Its number in the default order, which stays the same when it's moved.</param>
/// <param name="Name">The role that plays it, or "User".</param>
/// <param name="Warning">Why it may not work where it is, or empty.</param>
/// <param name="Pinned">It stays last, and nothing moves below it.</param>
public sealed record StageChoice(int Id, string Name, string Summary, bool IsUsers, string Warning, bool Pinned, bool CanMoveUp, bool CanMoveDown);

public sealed record PackChoice(string Name, bool Optional, bool Ticked, IReadOnlyList<GuidelineSection> Sections)
{
    public int Count => Sections.Sum(s => s.Guidelines.Count);

    public int TickedCount => Sections.Sum(s => s.Guidelines.Count(g => g.Ticked));
}

public sealed record GuidelineSection(string Name, IReadOnlyList<GuidelineChoice> Guidelines);

public sealed record GuidelineChoice(string Id, string Text, bool Ticked);
