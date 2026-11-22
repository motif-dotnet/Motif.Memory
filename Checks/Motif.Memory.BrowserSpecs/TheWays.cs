namespace Motif.Memory.BrowserSpecs;

/// <summary>
/// The ways of saving, as the real composition root registers them: the
/// specs over the User port see only their own Fixture's order.
/// </summary>
[Collection(SiteCollection.Name)]
public sealed class TheWays(Site site)
{
    [Fact]
    public async Task With_a_folder_picker_the_folder_comes_first_then_the_zip()
    {
        await using var profile = await site.Profile();
        var page = await site.Open(profile);

        (await SetupPage.Sections(page)).First().Should().Be("Folder");
        await Assertions.Expect(SetupPage.Ways(page))
            .ToHaveTextAsync(["Write to a folder", "Download zip"]);
        await Assertions.Expect(SetupPage.Ways(page).First)
            .ToHaveClassAsync("primary");
    }

    [Fact]
    public async Task Without_a_folder_picker_only_the_zip_is_offered_and_why()
    {
        await using var profile = await site.Profile(picker: false);
        var page = await site.Open(profile);

        (await SetupPage.Sections(page)).Should().NotContain("Folder");
        await Assertions.Expect(SetupPage.Ways(page))
            .ToHaveTextAsync(["Download zip"]);
        await Assertions.Expect(SetupPage.Ways(page).First)
            .ToHaveClassAsync("primary");
        await Assertions.Expect(page.Locator(".write"))
            .ToContainTextAsync("This browser can't write to a folder.");
    }
}
