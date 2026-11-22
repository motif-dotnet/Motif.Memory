namespace Motif.Memory.BrowserSpecs;

/// <summary>
/// The real Folder Fitting (JsInteropFolder, files.js and the calls between
/// them), over real folder handles from the browser's private storage. Only
/// the picker is stood in for (see <see cref="Folders"/>). What the rules
/// decide is in the specs over the User port; these check the browser's
/// side of each.
/// </summary>
[Collection(SiteCollection.Name)]
public sealed class TheFolder(Site site)
{
    [Fact]
    public async Task An_empty_folder_is_picked_and_the_write_button_names_it()
    {
        await using var profile = await site.Profile();
        var page = await site.Open(profile);
        await Folders.WillPick(page, "my-memory");

        await SetupPage.PickAFolder(page);

        await Assertions.Expect(SetupPage.Picked(page))
            .ToHaveTextAsync("my-memory");
        await Assertions.Expect(SetupPage.Ways(page).First)
            .ToHaveTextAsync("Write to “my-memory”");
    }

    [Fact]
    public async Task A_folder_holding_only_dot_git_is_picked()
    {
        await using var profile = await site.Profile();
        var page = await site.Open(profile);
        await Folders.Put(page, "repo", ".git/HEAD");
        await Folders.WillPick(page, "repo");

        await SetupPage.PickAFolder(page);

        await Assertions.Expect(SetupPage.Picked(page))
            .ToHaveTextAsync("repo");
    }

    [Fact]
    public async Task A_folder_with_a_file_in_it_is_refused_as_not_empty()
    {
        await using var profile = await site.Profile();
        var page = await site.Open(profile);
        await Folders.Put(page, "full", "notes.md");
        await Folders.WillPick(page, "full");

        await SetupPage.PickAFolder(page);

        await Assertions.Expect(SetupPage.Toast(page))
            .ToContainTextAsync("“full” isn't empty");
        await Assertions.Expect(SetupPage.Picked(page)).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task Closing_the_picker_keeps_the_folder_picked_before()
    {
        await using var profile = await site.Profile();
        var page = await site.Open(profile);
        await Folders.WillPick(page, "first");
        await SetupPage.PickAFolder(page);
        await Assertions.Expect(SetupPage.Picked(page))
            .ToHaveTextAsync("first");
        await Folders.WillDecline(page);

        await SetupPage.PickAFolder(page);

        await Assertions.Expect(SetupPage.Picked(page))
            .ToHaveTextAsync("first");
        await Assertions.Expect(SetupPage.Toast(page)).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task Refusing_to_let_the_browser_edit_the_folder_says_so()
    {
        await using var profile = await site.Profile();
        var page = await site.Open(profile);
        await Folders.WillRefuse(page);

        await SetupPage.PickAFolder(page);

        await Assertions.Expect(SetupPage.Toast(page)).ToContainTextAsync(
            "The browser didn't allow editing files in that folder.");
    }

    [Fact]
    public async Task Writing_writes_every_file_the_zip_holds_nested_ones_too()
    {
        await using var profile = await site.Profile();
        var page = await site.Open(profile);
        var (_, zipped) = await SetupPage.DownloadZip(page);
        await Folders.WillPick(page, "memory");
        await SetupPage.PickAFolder(page);

        await SetupPage.Ways(page).First.ClickAsync();

        await Assertions.Expect(SetupPage.Toast(page))
            .ToContainTextAsync("Memory written to “memory”");
        var written = await Folders.Files(page, "memory");
        written.Should().Equal(zipped);
        written.Keys.Should().Contain(path => path.Contains('/'));
    }
}
