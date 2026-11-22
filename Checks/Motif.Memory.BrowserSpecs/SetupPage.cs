namespace Motif.Memory.BrowserSpecs;

/// <summary>The parts of the setup page the specs reach for.</summary>
public static class SetupPage
{
    /// <summary>The headings of the page's sections, in order.</summary>
    public static Task<IReadOnlyList<string>> Sections(IPage page) =>
        page.Locator("details.choice > summary h2").AllInnerTextsAsync();

    /// <summary>The buttons for the ways of saving, in order.</summary>
    public static ILocator Ways(IPage page) =>
        page.Locator(".write button");

    public static ILocator Way(IPage page, string name) =>
        page.Locator(".write")
            .GetByRole(AriaRole.Button, new() { Name = name });

    /// <summary>Picks a folder through the Folder section's button.</summary>
    public static Task PickAFolder(IPage page) =>
        page.Locator("details.choice .folder button").ClickAsync();

    /// <summary>The folder shown as picked, if one is.</summary>
    public static ILocator Picked(IPage page) =>
        page.Locator("details.choice .folder .picked");

    /// <summary>The newest toast, which shows below any earlier ones.</summary>
    public static ILocator Toast(IPage page) => page.Locator(".toast").Last;

    /// <summary>The names of the workflow's stages, in order.</summary>
    public static Task<IReadOnlyList<string>> Stages(IPage page) =>
        page.Locator("ol.stages > li .who").AllInnerTextsAsync();

    public static ILocator Stage(IPage page, string name) =>
        page.Locator("ol.stages > li").Filter(new()
        {
            Has = page.Locator(".who", new() { HasTextString = name }),
        });

    /// <summary>
    /// Downloads the zip, and hands back its name and its files by path.
    /// </summary>
    public static async Task<(string Name, Dictionary<string, string> Files)>
        DownloadZip(IPage page)
    {
        var download = await page.RunAndWaitForDownloadAsync(
            () => Way(page, "Download zip").ClickAsync());
        var path = await download.PathAsync();
        using var zip = System.IO.Compression.ZipFile.OpenRead(path);
        var files = zip.Entries.ToDictionary(
            e => e.FullName,
            e =>
            {
                using var reader = new StreamReader(e.Open());
                return reader.ReadToEnd();
            });
        return (download.SuggestedFilename, files);
    }
}
