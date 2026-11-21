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

    /// <summary>
    /// Why the memory has no workflow, or empty when it has one.
    /// </summary>
    string WhyNoWorkflow { get; }

    IReadOnlyList<PackChoice> Packs { get; }

    Committing Committing { get; }

    /// <summary>
    /// The committing choices, in the order offered, with what each means.
    /// </summary>
    IReadOnlyList<CommittingChoice> Committings { get; }

    /// <summary>
    /// The ways of saving this browser can use, in the order offered. The
    /// first is the default.
    /// </summary>
    IReadOnlyList<SavingWay> Ways { get; }

    /// <summary>
    /// Why the ways this browser can't use aren't offered, or empty.
    /// </summary>
    string Unavailable { get; }

    /// <summary>
    /// What the last save did, and what to tell the agent, or
    /// <see cref="SavedMemory.None"/>.
    /// </summary>
    SavedMemory Saved { get; }

    /// <summary>
    /// The note in the memory that the Developer points their agent at, to finish setting it up.
    /// </summary>
    string SetupNote { get; }

    /// <summary>
    /// What went wrong with the last command, or <see cref="Problem.None"/>.
    /// </summary>
    Problem Problem { get; }
}

/// <param name="Steps">What to do, ending with telling the agent.</param>
/// <param name="Tell">The sentence to tell the agent.</param>
public sealed record SavedMemory(string Title, string Steps, string Tell)
{
    public static readonly SavedMemory None = new("", "", "");
}

public sealed record CommittingChoice(
    Committing Value, string Label, string Line, bool Chosen);

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
