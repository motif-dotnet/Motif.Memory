using System.IO.Compression;
using System.Text;
using Microsoft.JSInterop;

namespace Motif.Memory.Adapters;

/// <summary>
/// Writes the memory into a folder through the browser's folder picker, or saves it as a zip (wwwroot/files.js).
/// Call <see cref="Start"/> once at startup, so <see cref="CanPickFolder"/> is known before the page shows.
/// </summary>
public sealed class BrowserFilesMemoryFiles(IJSRuntime js) : MemoryFiles, IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> module =
        new(() => js.InvokeAsync<IJSObjectReference>("import", "./_content/Motif.Memory.MemoryFiles/files.js").AsTask());

    public bool CanPickFolder { get; private set; }

    public async Task Start() => CanPickFolder = await (await module.Value).InvokeAsync<bool>("canPickFolder");

    public async Task<FolderWrite> WriteToFolder(IReadOnlyList<MemoryFile> files)
    {
        // As an object, the list is one argument. As an array, each file in it would be an argument of its own.
        var list = (object)files.Select(f => new[] { f.Path, f.Text }).ToArray();
        var result = await (await module.Value).InvokeAsync<Result>("writeToFolder", list);
        return result.Outcome switch
        {
            "written" => new FolderWrite.Written(result.Name),
            "notEmpty" => new FolderWrite.NotEmpty(result.Name),
            "failed" => new FolderWrite.Failed(result.Reason),
            "cancelled" => new FolderWrite.Cancelled(),
            _ => new FolderWrite.Failed($"The browser gave an answer Motif Memory doesn't know: \"{result.Outcome}\"."),
        };
    }

    private sealed record Result(string Outcome, string Name, string Reason);

    public async Task DownloadZip(string name, IReadOnlyList<MemoryFile> files) =>
        await (await module.Value).InvokeVoidAsync("save", name, Zip(files));

    /// <summary>
    /// The files at the zip's root, as UTF-8 without a byte order mark.
    /// </summary>
    private static byte[] Zip(IReadOnlyList<MemoryFile> files)
    {
        using var stream = new MemoryStream();
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var file in files)
            {
                using var entry = zip.CreateEntry(file.Path, CompressionLevel.Optimal).Open();
                entry.Write(new UTF8Encoding(false).GetBytes(file.Text));
            }
        }
        return stream.ToArray();
    }

    public async ValueTask DisposeAsync()
    {
        if (module.IsValueCreated) await (await module.Value).DisposeAsync();
    }
}
