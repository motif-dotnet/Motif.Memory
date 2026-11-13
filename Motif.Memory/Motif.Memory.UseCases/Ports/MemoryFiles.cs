namespace Motif.Memory;

/// <summary>
/// The Memory Files port: writes the memory's files out, to a folder or as a zip.
/// </summary>
public interface MemoryFiles
{
    bool CanPickFolder { get; }

    /// <summary>
    /// Shows the folder picker, then writes the files if the folder is empty or holds only .git.
    /// </summary>
    Task<FolderWrite> WriteToFolder(IReadOnlyList<MemoryFile> files);

    Task DownloadZip(string name, IReadOnlyList<MemoryFile> files);
}

/// <summary>
/// How writing to a folder went.
/// </summary>
public abstract record FolderWrite
{
    private FolderWrite()
    {
    }

    public sealed record Written(string FolderName) : FolderWrite;

    /// <summary>
    /// The picker was closed, or write access refused: nothing changes.
    /// </summary>
    public sealed record Cancelled : FolderWrite;

    /// <summary>
    /// The folder holds something other than .git, so nothing was written.
    /// </summary>
    public sealed record NotEmpty(string FolderName) : FolderWrite;

    /// <summary>
    /// The browser couldn't write a file. Some may have been written.
    /// </summary>
    public sealed record Failed(string Reason) : FolderWrite;
}
