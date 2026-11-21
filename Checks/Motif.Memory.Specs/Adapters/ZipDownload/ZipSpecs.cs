using System.IO.Compression;

namespace Motif.Memory.Specs;

/// <summary>
/// The zip adapter's zip: what other tools see when they open it, which the
/// behaviour specs don't, since they read it back as text.
/// </summary>
public class ZipSpecs
{
    [Fact]
    public async Task The_zip_holds_the_files_at_its_root_as_UTF8_without_a_byte_order_mark()
    {
        var f = new Fixture();

        await f.Doing.Save("Zip");

        using var zip = new ZipArchive(new MemoryStream(f.Download.SavedBytes));
        var start = zip.GetEntry("Start here.md")!;
        using var bytes = new MemoryStream();
        start.Open().CopyTo(bytes);
        bytes.ToArray().Take(3).Should().NotEqual([0xEF, 0xBB, 0xBF]);
        zip.Entries.Should().Contain(e => e.FullName.StartsWith("Roles/"));
        zip.Entries.Should().NotContain(e =>
            e.FullName.StartsWith("/") ||
            e.FullName.StartsWith("agent-memory/"));
    }
}
