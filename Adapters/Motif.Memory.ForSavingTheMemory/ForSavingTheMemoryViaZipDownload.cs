using System.IO.Compression;
using System.Text;

namespace Motif.Memory.Adapters;

/// <summary>
/// Saves the memory as a zip the browser downloads, through the Browser
/// Download Seam. Every browser can, and there's nowhere to choose.
/// </summary>
public sealed class ForSavingTheMemoryViaZipDownload(Download browser)
    : ForSavingTheMemory
{
    public const string ZipName = "agent-memory.zip";

    public Task<string> Unavailable() => Task.FromResult("");

    public SavingWay Way { get; } = new(
        Name: "Zip",
        Icon: "download",
        About: $"Downloaded as {ZipName}, to unzip into a folder that "
            + "stays put.",
        Choose: "",
        Where: "",
        Save: "Download zip",
        Hint: "");

    public Task<Choosing> ChooseWhere() =>
        Task.FromResult<Choosing>(new Choosing.Chosen());

    public async Task<Saving> Save(IReadOnlyList<MemoryFile> files)
    {
        await browser.Save(ZipName, Zip(files));
        return new Saving.Saved(
            "Memory downloaded",
            $"Unzip {ZipName} into a folder that stays put.");
    }

    /// <summary>
    /// The files at the zip's root, as UTF-8 without a byte order mark.
    /// </summary>
    private static byte[] Zip(IReadOnlyList<MemoryFile> files)
    {
        using var stream = new MemoryStream();
        using (var zip = new ZipArchive(
            stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var file in files)
            {
                using var entry = zip
                    .CreateEntry(file.Path, CompressionLevel.Optimal)
                    .Open();
                entry.Write(new UTF8Encoding(false).GetBytes(file.Text));
            }
        }
        return stream.ToArray();
    }
}
