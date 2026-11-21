namespace Motif.Memory.Adapters;

/// <summary>
/// The Folder Seam: what the folder adapter needs to reach a folder, one call
/// each. Every Fitting must behave as these comments say. A failure throws a
/// <see cref="FolderException"/> of one of the Seam's own kinds.
/// </summary>
public interface Folder
{
    /// <summary>
    /// Whether a folder can be chosen here, such as with a folder picker.
    /// </summary>
    Task<bool> CanChoose();

    /// <summary>
    /// Asks where the memory should go, such as with a folder picker, for a
    /// folder that can be edited. The user declining, such as by closing the
    /// picker, fails as <see cref="FolderFailure.Declined"/>. A folder chosen
    /// but with access to edit it refused fails as
    /// <see cref="FolderFailure.Refused"/>.
    /// </summary>
    Task<ChosenFolder> Choose();
}
