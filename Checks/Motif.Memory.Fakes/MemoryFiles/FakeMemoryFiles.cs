namespace Motif.Memory.Fakes;

/// <summary>
/// Records what was written. It can be set to cancel, find the folder not empty, fail, or have no folder picker.
/// </summary>
public sealed class FakeMemoryFiles : MemoryFiles
{
    public enum Picking
    {
        EmptyFolder,
        Cancel,
        NotEmpty,
        Fail,
    }

    public bool CanPickFolder { get; set; } = true;

    public Picking Picks { get; set; } = Picking.EmptyFolder;

    public string FolderName { get; set; } = "memory";

    /// <summary>
    /// The files last written to a folder or a zip, by path.
    /// </summary>
    public IReadOnlyDictionary<string, string> Written { get; private set; } = new Dictionary<string, string>();

    public string ZipName { get; private set; } = "";

    public Task<FolderWrite> WriteToFolder(IReadOnlyList<MemoryFile> files)
    {
        FolderWrite result = Picks switch
        {
            Picking.Cancel => new FolderWrite.Cancelled(),
            Picking.NotEmpty => new FolderWrite.NotEmpty(FolderName),
            Picking.Fail => new FolderWrite.Failed("The folder can't be written to."),
            _ => new FolderWrite.Written(FolderName),
        };
        if (result is FolderWrite.Written) Record(files);
        return Task.FromResult(result);
    }

    public Task DownloadZip(string name, IReadOnlyList<MemoryFile> files)
    {
        ZipName = name;
        Record(files);
        return Task.CompletedTask;
    }

    private void Record(IReadOnlyList<MemoryFile> files) => Written = files.ToDictionary(f => f.Path, f => f.Text);
}
