namespace Motif.Memory.Specs;

/// <summary>
/// Unticking single guidelines, and leaving out whole packs.
/// </summary>
public class GuidelineSpecs
{
    private static GuidelineChoice Guideline(Fixture f, string pack, string startsWith) =>
        f.Queries.Packs.Single(p => p.Name == pack).Sections.SelectMany(s => s.Guidelines).Single(g => g.Text.StartsWith(startsWith));

    [Fact]
    public void An_unticked_guideline_isnt_written()
    {
        var f = new Fixture();
        var names = Guideline(f, "General", "Prefer the shortest real word");

        f.Tasks.TickGuideline(names.Id, false);

        Guideline(f, "General", "Prefer the shortest real word").Ticked.Should().BeFalse();
        f.Queries.Packs.Single(p => p.Name == "General").TickedCount.Should().Be(f.Queries.Packs.Single(p => p.Name == "General").Count - 1);
        var general = f.Write()["Guidelines/General.md"];
        general.Should().NotContain("shortest real word");
        general.Should().Contain("Prefer names in the domain's words");
    }

    [Fact]
    public void A_section_with_no_guidelines_left_loses_its_heading()
    {
        var f = new Fixture();

        f.Tasks.TickGuideline(Guideline(f, "General", "Minimal, readable").Id, false);

        var general = f.Write()["Guidelines/General.md"];
        general.Should().NotContain("### Prefer\n").And.NotContain("Minimal, readable");
        general.Should().Contain("### Prefer X over Y").And.Contain("### Avoid");
    }

    [Fact]
    public void A_pack_left_out_loses_its_file_and_Generals_links_to_it()
    {
        var f = new Fixture();

        f.Tasks.TickPack("C#", false);

        f.Queries.Packs.Single(p => p.Name == "C#").Ticked.Should().BeFalse();
        var memory = f.Write();
        memory.Keys.Should().NotContain("Guidelines/CSharp.md");
        memory["Guidelines/General.md"].Should().Contain("when there's a note for it. Read the [Motif guidelines]");
    }

    [Fact]
    public void A_pack_with_every_guideline_unticked_is_left_out()
    {
        var f = new Fixture();
        foreach (var g in f.Queries.Packs.Single(p => p.Name == "Motif").Sections.SelectMany(s => s.Guidelines))
            f.Tasks.TickGuideline(g.Id, false);

        var memory = f.Write();

        memory.Keys.Should().NotContain("Guidelines/Motif.md");
        memory["Guidelines/General.md"].Should().NotContain("Motif guidelines");
    }
}
