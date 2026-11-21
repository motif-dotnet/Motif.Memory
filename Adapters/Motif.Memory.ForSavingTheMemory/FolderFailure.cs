namespace Motif.Memory.Adapters;

/// <summary>
/// How a call to the Folder Seam failed, in the Seam's own terms rather than
/// a platform's error names.
/// </summary>
public enum FolderFailure
{
    /// <summary>
    /// The user declined to choose a folder, such as by closing the picker.
    /// </summary>
    Declined,

    /// <summary>
    /// Access to edit the folder was refused.
    /// </summary>
    Refused,

    /// <summary>
    /// Anything else.
    /// </summary>
    Failed,
}
