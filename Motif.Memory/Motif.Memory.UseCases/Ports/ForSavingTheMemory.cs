namespace Motif.Memory;

/// <summary>
/// The For Saving The Memory port: one way of saving the memory, such as into
/// a folder or as a zip. Saving takes two steps, choosing where, then saving.
/// Each way has its own words, since only it knows what saving that way means.
/// </summary>
public interface ForSavingTheMemory
{
    /// <summary>
    /// Why this browser can't save this way, or empty when it can. Asked once,
    /// at startup.
    /// </summary>
    Task<string> Unavailable();

    /// <summary>
    /// What the user sees of this way, as it stands.
    /// </summary>
    SavingWay Way { get; }

    /// <summary>
    /// Asks the user where to save, such as by picking a folder, and keeps
    /// the choice until it's saved to.
    /// </summary>
    Task<Choosing> ChooseWhere();

    /// <summary>
    /// Saves the files where was chosen. Each choice is saved to once: it must
    /// be chosen again to save again.
    /// </summary>
    Task<Saving> Save(IReadOnlyList<MemoryFile> files);
}
