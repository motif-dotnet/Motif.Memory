namespace Motif.Memory.BrowserSpecs;

/// <summary>
/// At a phone's width, between the phone and narrow breakpoints, and on a
/// computer, the app bar and the row of buttons for saving fit, with
/// nothing in them overlapping. The app bar is the same whatever the page
/// shows; the buttons are measured with and without a folder picker, and
/// with a folder picked whose long name the write button carries.
/// </summary>
[Collection(SiteCollection.Name)]
public sealed class TheLayout(Site site)
{
    public enum Showing
    {
        NoPicker,
        NothingPicked,
        LongFolderName,
    }

    public static TheoryData<int, Showing> Cases()
    {
        var cases = new TheoryData<int, Showing>();
        foreach (var width in new[] { 375, 637, 1280 })
            foreach (var showing in Enum.GetValues<Showing>())
                cases.Add(width, showing);
        return cases;
    }

    [Theory]
    [InlineData(375)]
    [InlineData(637)]
    [InlineData(1280)]
    public async Task The_app_bar_fits_with_nothing_overlapping(int width)
    {
        await using var profile = await site.Profile(width);
        var page = await site.Open(profile);

        var bar = await Measuring.Measure(page, "header.appbar");

        bar.ContentWidth.Should().BeLessThanOrEqualTo(bar.Width);
        bar.Overlapping.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public async Task The_saving_buttons_fit_with_nothing_overlapping(
        int width, Showing showing)
    {
        await using var profile = await site.Profile(
            width, picker: showing != Showing.NoPicker);
        var page = await Open(profile, showing);

        var row = await Measuring.Measure(page, ".write");

        row.ContentWidth.Should().BeLessThanOrEqualTo(row.Width);
        row.Overlapping.Should().BeEmpty();
    }

    private async Task<IPage> Open(IBrowserContext profile, Showing showing)
    {
        var page = await site.Open(profile);
        if (showing == Showing.LongFolderName)
        {
            await Folders.WillPick(page, "my-agents-external-memory");
            await SetupPage.PickAFolder(page);
            await SetupPage.Picked(page).WaitForAsync();
        }
        return page;
    }
}
