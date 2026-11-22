namespace Motif.Memory.BrowserSpecs;

/// <summary>
/// The real Download Fitting (JsInteropDownload and download.js): the zip
/// the browser saves. Its format is in the zip adapter's own specs.
/// </summary>
[Collection(SiteCollection.Name)]
public sealed class TheZip(Site site)
{
    [Fact]
    public async Task Saves_agent_memory_zip_with_the_memory_at_its_root()
    {
        await using var profile = await site.Profile();
        var page = await site.Open(profile);

        var (name, files) = await SetupPage.DownloadZip(page);

        name.Should().Be("agent-memory.zip");
        files.Keys.Should().Contain("Start here.md");
        files["Start here.md"].Should().NotBeEmpty();
        await Assertions.Expect(SetupPage.Toast(page))
            .ToContainTextAsync("Memory downloaded");
    }
}
