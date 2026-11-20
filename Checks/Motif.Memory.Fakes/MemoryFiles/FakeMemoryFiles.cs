namespace Motif.Memory.Fakes;

/// <summary>
/// Records what was written. Picking can be set to cancel, find the folder not empty or fail, and writing
/// to find the folder filled since or fail. It can also have no folder picker.
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

    public enum Writing
    {
        Write,
        NotEmpty,
        Fail,
    }

    public bool CanPickFolder { get; set; } = true;

    public Picking Picks { get; set; } = Picking.EmptyFolder;

    public Writing Writes { get; set; } = Writing.Write;

    public string FolderName { get; set; } = "memory";

    /// <summary>
    /// The files last written to a folder or a zip, by path.
    /// </summary>
    public IReadOnlyDictionary<string, string> Written { get; private set; } = new Dictionary<string, string>();

    public string ZipName { get; private set; } = "";

    public Task<FolderPick> PickFolder() =>
        Task.FromResult<FolderPick>(Picks switch
        {
            Picking.Cancel => new FolderPick.Cancelled(),
            Picking.NotEmpty => new FolderPick.NotEmpty(FolderName),
            Picking.Fail => new FolderPick.Failed("The folder can't be opened."),
            _ => new FolderPick.Picked(FolderName),
        });

    public Task<FolderWrite> WriteToFolder(IReadOnlyList<MemoryFile> files)
    {
        FolderWrite result = Writes switch
        {
            Writing.NotEmpty => new FolderWrite.NotEmpty(FolderName),
            Writing.Fail => new FolderWrite.Failed("The folder can't be written to."),
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
