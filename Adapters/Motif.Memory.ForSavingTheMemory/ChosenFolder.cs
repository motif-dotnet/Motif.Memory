namespace Motif.Memory.Adapters;

/// <summary>
/// A folder the Folder Seam's <see cref="Folder.Choose"/> handed back.
/// </summary>
public interface ChosenFolder
{
    string Name { get; }

    /// <summary>
    /// The names of everything at the folder's top level, folders such as .git
    /// included, without opening any of it.
    /// </summary>
    Task<IReadOnlyList<string>> Names();

    /// <summary>
    /// Writes the text at a path such as "Roles/Tester.md", making the folders
    /// on the way, and replacing a file that's already there.
    /// </summary>
    Task Write(string path, string text);
}
