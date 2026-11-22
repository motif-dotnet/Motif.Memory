namespace Motif.Memory.BrowserSpecs;

/// <summary>
/// Dragging a workflow stage with the mouse, which needs the browser's own
/// drag and drop and app.js. Where a stage lands is in the specs over the
/// User port; these check that a drag reaches it.
/// </summary>
[Collection(SiteCollection.Name)]
public sealed class Reordering(Site site)
{
    [Fact]
    public async Task Dragging_a_stage_onto_a_later_one_moves_it_below_that()
    {
        await using var profile = await site.Profile();
        var page = await site.Open(profile);
        var before = await SetupPage.Stages(page);

        await SetupPage.Stage(page, before[1])
            .DragToAsync(SetupPage.Stage(page, before[3]));

        await Assertions.Expect(SetupPage.StageNames(page))
            .ToHaveTextAsync(
                [before[0], before[2], before[3], before[1], .. before.Skip(4)]);
    }

    [Fact]
    public async Task While_dragging_the_stage_it_is_over_shows_where_it_lands()
    {
        await using var profile = await site.Profile();
        var page = await site.Open(profile);
        var stages = await SetupPage.Stages(page);
        var picked = SetupPage.Stage(page, stages[1]);
        var over = SetupPage.Stage(page, stages[3]);

        await picked.HoverAsync();
        await page.Mouse.DownAsync();
        await over.HoverAsync();

        await Assertions.Expect(picked).ToHaveClassAsync(
            new System.Text.RegularExpressions.Regex(@"\bdragging\b"));
        await Assertions.Expect(over).ToHaveClassAsync(
            new System.Text.RegularExpressions.Regex(@"\bdrop-below\b"));
        await page.Mouse.UpAsync();
    }
}
