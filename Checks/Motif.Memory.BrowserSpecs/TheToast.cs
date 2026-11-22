namespace Motif.Memory.BrowserSpecs;

/// <summary>
/// The toast after saving offers the sentence to tell the agent, through
/// the browser's own clipboard.
/// </summary>
[Collection(SiteCollection.Name)]
public sealed class TheToast(Site site)
{
    [Fact]
    public async Task Copy_puts_what_to_tell_the_agent_on_the_clipboard()
    {
        await using var profile = await site.Profile();
        await profile.GrantPermissionsAsync(
            ["clipboard-read", "clipboard-write"]);
        var page = await site.Open(profile);
        await SetupPage.DownloadZip(page);
        var toast = SetupPage.Toast(page);
        var tellTheAgent = await toast.Locator(".copy").InnerTextAsync();

        await toast.GetByRole(AriaRole.Button, new() { Name = "Copy" })
            .ClickAsync();

        await Assertions.Expect(toast).ToContainTextAsync("Copied");
        (await page.EvaluateAsync<string>("navigator.clipboard.readText()"))
            .Should().Be(tellTheAgent);
    }
}
