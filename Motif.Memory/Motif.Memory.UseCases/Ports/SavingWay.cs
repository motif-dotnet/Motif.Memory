namespace Motif.Memory;

/// <summary>
/// A way of saving the memory, in the words the user sees.
/// </summary>
/// <param name="Name">Its heading, such as "Folder", which also tells the ways
/// apart.</param>
/// <param name="Icon">Its icon's name: "folder" or "download".</param>
/// <param name="About">What saving this way means.</param>
/// <param name="Choose">The label for choosing where, or empty when there's
/// nowhere to choose.</param>
/// <param name="Where">Where was chosen, such as the folder's name, or empty.
/// </param>
/// <param name="Save">The label for saving, such as "Download zip".</param>
/// <param name="Hint">What to do before it's ready to save, or empty when it's
/// ready.</param>
public sealed record SavingWay(
    string Name,
    string Icon,
    string About,
    string Choose,
    string Where,
    string Save,
    string Hint)
{
    public bool CanSave => Hint == "";
}
