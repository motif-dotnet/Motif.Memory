namespace Motif.Memory;

/// <summary>
/// The User port's queries: everything the setup page shows.
/// </summary>
public interface ForSeeing
{
    IReadOnlyList<RoleChoice> Roles { get; }

    /// <summary>
    /// The stages of the roles ticked, and the user's, in the Developer's
    /// order.
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
    /// The note in the memory that the Developer points their agent at, to
    /// finish setting it up.
    /// </summary>
    string SetupNote { get; }

    /// <summary>
    /// What went wrong with the last command, or <see cref="Problem.None"/>.
    /// </summary>
    Problem Problem { get; }
}
