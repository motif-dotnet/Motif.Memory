namespace Motif.Memory;

/// <summary>
/// The Memory Files port: writes the memory's files out, to a folder or as a zip.
/// </summary>
public interface MemoryFiles
{
    bool CanPickFolder { get; }

    /// <summary>
    /// Shows the folder picker, and keeps the folder if it's empty or holds only .git.
    /// </summary>
    Task<FolderPick> PickFolder();

    /// <summary>
    /// Writes the files into the folder picked, if it still holds nothing but .git.
    /// Each folder picked is written to once: it must be picked again to write again.
    /// </summary>
    Task<FolderWrite> WriteToFolder(IReadOnlyList<MemoryFile> files);

    Task DownloadZip(string name, IReadOnlyList<MemoryFile> files);
}

/// <summary>
/// How picking a folder went.
/// </summary>
public abstract record FolderPick
{
    private FolderPick()
    {
    }

    public sealed record Picked(string FolderName) : FolderPick;

    /// <summary>
    /// The picker was closed: the folder picked before, if any, is kept.
    /// </summary>
    public sealed record Cancelled : FolderPick;

    /// <summary>
    /// The folder holds something other than .git, so it isn't kept.
    /// </summary>
    public sealed record NotEmpty(string FolderName) : FolderPick;

    /// <summary>
    /// The browser refused the folder, such as write access to it.
    /// </summary>
    public sealed record Failed(string Reason) : FolderPick;
}

/// <summary>
/// How writing to the folder picked went.
/// </summary>
public abstract record FolderWrite
{
    private FolderWrite()
    {
    }

    public sealed record Written(string FolderName) : FolderWrite;

    /// <summary>
    /// The folder has filled since it was picked, so nothing was written.
    /// </summary>
    public sealed record NotEmpty(string FolderName) : FolderWrite;

    /// <summary>
    /// The browser couldn't write a file. Some may have been written.
    /// </summary>
    public sealed record Failed(string Reason) : FolderWrite;
}
