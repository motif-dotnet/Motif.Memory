namespace Motif.Memory.Specs;

/// <summary>
/// The real content is valid, and the format's own rules, on content made up for them.
/// </summary>
public class ContentSpecs
{
    private static readonly Content Real = new(Fixture.RealContent.Files);

    [Fact]
    public void The_content_is_valid()
    {
        Real.Problems.Should().BeEmpty();
    }

    [Fact]
    public void Every_role_note_has_front_matter_and_every_workflow_role_writes_the_progress_doc()
    {
        Real.Roles.Should().HaveCount(12);
        Real.Notes.Where(Content.IsRoleNote).Should().OnlyContain(n => n.Facts.Has("core"));
        Real.Roles.Where(r => r.InWorkflow).Should().OnlyContain(r => r.Writes.Contains("PROGRESS.md"));
        Real.Roles.Single(r => r.Name == "Design Griller").InWorkflow.Should().BeFalse();
    }

    [Fact]
    public void Every_doc_named_in_front_matter_has_its_bullet()
    {
        Real.Docs.Select(d => d.FileName).Should().BeEquivalentTo(
            "PROGRESS.md", "BEHAVIOUR_CHECKLIST.md", "CODE_REVIEW.md", "BUGS.md", "MANUAL_TESTS.md");
        Real.Roles.SelectMany(r => r.Writes.Concat(r.Reads)).Distinct()
            .Should().OnlyContain(doc => Real.Docs.Any(d => d.FileName == doc));
    }

    private static Content Of(string text) => new([new ContentFile("Note.md", text)]);

    [Theory]
    [InlineData("<!-- if Nobody -->\nText\n<!-- end -->\n", "unknown condition \"Nobody\"")]
    [InlineData("<!-- if workflow -->\nText\n", "an if left open")]
    [InlineData("Text\n<!-- end -->\n", "an end with no if")]
    [InlineData("A<!-- if workflow -->B\n", "an if left open at the end of the line")]
    [InlineData("<!-- if workflow -->\n<!-- if commit: never -->\nA\n<!-- end -->\n", "an if inside the if on line 1")]
    [InlineData("<!-- if workflow and Bug Fixer or commit: never -->\nA\n<!-- end -->\n", "mixes and with or")]
    public void A_tag_that_doesnt_work_is_a_content_error(string text, string problem)
    {
        Of(text).Problems.Should().ContainSingle().Which.Should().Contain(problem);
    }

    [Fact]
    public void A_role_note_with_no_front_matter_is_a_content_error()
    {
        new Content([new ContentFile("Roles/Someone.md", "Job\n- Something.\n")]).Problems
            .Should().ContainSingle().Which.Should().Contain("no front matter");
    }

    [Fact]
    public void A_doc_named_in_front_matter_with_no_bullet_is_a_content_error()
    {
        new Content([new ContentFile("Roles/Someone.md", "---\ncore: false\nwrites: [PROGRESS.md, NOTES.md]\nreads: []\n---\nJob\n")]).Problems
            .Should().Contain(p => p.Contains("NOTES.md has no bullet"));
    }

    private const string Role = "---\ncore: false\nwrites: []\nreads: []\n---\n\nJob\n- Help.\n";
    private const string General = "---\npack: General\noptional: false\n---\n\n### Prefer\n- One.\n";

    [Fact]
    public void Tags_join_conditions_with_and_or_or_inline_or_in_blocks()
    {
        var f = Fixture.Of(
            ("Roles/Alpha.md", Role), ("Roles/Beta.md", Role), ("Guidelines/General.md", General),
            ("Start here.md",
                "<!-- if Alpha and Beta -->\nBoth.\n<!-- end -->\n\n<!-- if Alpha or Beta -->\nEither.\n<!-- end -->\n\n" +
                "Inline<!-- if Beta --> beta<!-- end --><!-- if Alpha --> alpha<!-- end -->.\n"));
        f.Tasks.TickRole("Beta", false);

        f.Write()["Start here.md"].Should().Be("Either.\n\nInline alpha.\n");
    }

    [Fact]
    public void A_list_item_starting_with_a_link_to_a_note_not_written_is_dropped_with_its_indented_lines()
    {
        var f = Fixture.Of(
            ("Roles/Alpha.md", Role), ("Roles/Beta.md", Role), ("Guidelines/General.md", General),
            ("Start here.md",
                "- [Alpha](Roles/Alpha.md): first\n- [Beta](Roles/Beta.md): second\n  more about Beta\n" +
                "- Plain, and [Beta](Roles/Beta.md) later\n- [Web](https://example.com)\n"));
        f.Tasks.TickRole("Beta", false);

        f.Write()["Start here.md"].Should().Be(
            "- [Alpha](Roles/Alpha.md): first\n- Plain, and [Beta](Roles/Beta.md) later\n- [Web](https://example.com)\n");
    }

    [Fact]
    public void A_note_with_a_when_condition_is_written_only_when_it_holds()
    {
        var f = Fixture.Of(("Guidelines/General.md", General), ("Extra.md", "---\nwhen: commit: never\n---\nHello.\n"));

        f.Write().Keys.Should().NotContain("Extra.md");

        f.Tasks.ChooseCommitting(Committing.Never);
        f.Write()["Extra.md"].Should().Be("Hello.\n");
    }

    [Fact]
    public void Links_in_code_arent_links()
    {
        Links.Targets("Roles/Note.md", "Use `[Name](Folder/Note.md)`, or [Real](../Start%20here.md#top).\n```\n[Fenced](No.md)\n```\n")
            .Should().Equal("Start here.md");
    }
}
