namespace Motif.Memory.Specs;

/// <summary>
/// The real content is valid, and the format's own rules, on content made up
/// for them.
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
        Real.Notes.Where(Content.IsRoleNote)
            .Should().OnlyContain(n => n.Facts.Has("core"));
        Real.Roles.Where(r => r.InWorkflow)
            .Should().OnlyContain(r => r.Writes.Contains("PROGRESS.md"));
        Real.Roles.Single(r => r.Name == "Design Griller")
            .InWorkflow.Should().BeTrue();
    }

    [Fact]
    public void Every_doc_named_in_front_matter_has_its_bullet()
    {
        Real.Docs.Select(d => d.FileName).Should().BeEquivalentTo(
            "GRILLING.md", "PROGRESS.md", "BEHAVIOUR_CHECKLIST.md", "CODE_REVIEW.md",
            "BUGS.md", "MANUAL_TESTS.md");
        Real.Roles.SelectMany(r => r.Writes.Concat(r.Reads)).Distinct()
            .Should().OnlyContain(doc => Real.Docs.Any(d => d.FileName == doc));
    }

    private static Content Of(string text) =>
        new([new ContentFile("Note.md", text)]);

    [Theory]
    [InlineData("<!-- if Nobody -->\nText\n<!-- end -->\n", "unknown condition \"Nobody\"")]
    [InlineData("<!-- if workflow -->\nText\n", "an if left open")]
    [InlineData("Text\n<!-- end -->\n", "an end with no if")]
    [InlineData("A<!-- if workflow -->B\n", "an if left open at the end of the line")]
    [InlineData("<!-- if workflow -->\n<!-- if commit: never -->\nA\n<!-- end -->\n", "an if inside the if on line 1")]
    [InlineData("<!-- if workflow and Bug Fixer or commit: never -->\nA\n<!-- end -->\n", "mixes and with or")]
    public void A_tag_that_doesnt_work_is_a_content_error(string text, string problem)
    {
        Of(text).Problems.Should().ContainSingle()
            .Which.Should().Contain(problem);
    }

    [Fact]
    public void A_role_note_with_no_front_matter_is_a_content_error()
    {
        new Content([new ContentFile(
                "Roles/Someone.md", "Job\n- Something.\n")])
            .Problems
            .Should().ContainSingle().Which.Should().Contain("no front matter");
    }

    [Fact]
    public void A_note_with_empty_front_matter_reads_as_having_none()
    {
        var note = Of("---\n---\nHello.\n").Notes.Single();

        note.Facts.IsEmpty.Should().BeTrue();
        note.Body.Should().Be("Hello.\n");
    }

    [Fact]
    public void A_doc_named_in_front_matter_with_no_bullet_is_a_content_error()
    {
        new Content([new ContentFile("Roles/Someone.md",
                "---\ncore: false\nwrites: [PROGRESS.md, NOTES.md]\n" +
                "reads: []\n---\nJob\n")])
            .Problems
            .Should().Contain(p => p.Contains("NOTES.md has no bullet"));
    }

    private const string Role =
        "---\ncore: false\nwrites: []\nreads: []\n---\n\nJob\n- Help.\n";
    private const string General =
        "---\npack: General\noptional: false\n---\n\n### Prefer\n- One.\n";

    [Fact]
    public void Tags_join_conditions_with_and_or_or_inline_or_in_blocks()
    {
        var f = Fixture.Of(
            ("Roles/Alpha.md", Role), ("Roles/Beta.md", Role),
            ("Guidelines/General.md", General),
            ("Start here.md",
                "<!-- if Alpha and Beta -->\nBoth.\n<!-- end -->\n\n" +
                "<!-- if Alpha or Beta -->\nEither.\n<!-- end -->\n\n" +
                "Inline<!-- if Beta --> beta<!-- end -->" +
                "<!-- if Alpha --> alpha<!-- end -->.\n"));
        f.Doing.TickRole("Beta", false);

        f.Write()["Start here.md"].Should().Be("Either.\n\nInline alpha.\n");
    }

    [Fact]
    public void A_list_item_starting_with_a_link_to_a_note_not_written_is_dropped_with_its_indented_lines()
    {
        var f = Fixture.Of(
            ("Roles/Alpha.md", Role), ("Roles/Beta.md", Role),
            ("Guidelines/General.md", General),
            ("Start here.md",
                "- [Alpha](Roles/Alpha.md): first\n" +
                "- [Beta](Roles/Beta.md): second\n  more about Beta\n" +
                "- Plain, and [Beta](Roles/Beta.md) later\n" +
                "- [Web](https://example.com)\n"));
        f.Doing.TickRole("Beta", false);

        f.Write()["Start here.md"].Should().Be(
            "- [Alpha](Roles/Alpha.md): first\n" +
            "- Plain, and [Beta](Roles/Beta.md) later\n" +
            "- [Web](https://example.com)\n");
    }

    [Fact]
    public void A_note_with_a_when_condition_is_written_only_when_it_holds()
    {
        var f = Fixture.Of(
            ("Guidelines/General.md", General),
            ("Extra.md", "---\nwhen: commit: never\n---\nHello.\n"));

        f.Write().Keys.Should().NotContain("Extra.md");

        f.Doing.ChooseCommitting(Committing.Never);
        f.Write()["Extra.md"].Should().Be("Hello.\n");
    }

    private static Content Made(params (string Path, string Text)[] files) =>
        new(files.Select(f => new ContentFile(f.Path, f.Text)));

    private const string DocsNote =
        "- Progress (`PROGRESS.md`): what's done.\n- Notes (`NOTES.md`): notes.\n";

    [Fact]
    public void Notes_are_in_path_order_with_forward_slashes()
    {
        Made(("b.md", ""), ("Roles\\a.md", ""), ("A.md", ""))
            .Notes.Select(n => n.Path)
            .Should().Equal("A.md", "Roles/a.md", "b.md");
    }

    [Fact]
    public void A_missing_note_reads_as_empty_and_a_doc_with_no_bullet_is_named_by_its_file()
    {
        var content = Made((Content.TemporaryDocsNote, DocsNote));

        content.Body("Missing.md").Should().Be("");
        content.Doc("NOTES.md").Should().Be(new TemporaryDoc("Notes", "NOTES.md"));
        content.Doc("OTHER.md").Should().Be(new TemporaryDoc("OTHER.md", "OTHER.md"));
    }

    [Fact]
    public void Only_markdown_notes_in_the_roles_folder_are_role_notes()
    {
        Made(("Roles/Alpha.md", Role), ("Roles/picture.png", ""), ("Alpha.md", Role))
            .Roles.Select(r => r.Path).Should().Equal("Roles/Alpha.md");
    }

    [Fact]
    public void Plain_text_keeps_a_links_words_and_drops_its_target_and_backticks()
    {
        Content.Plain(" Use `checks` and [the Workflow](Workflow/Workflow.md). ")
            .Should().Be("Use checks and the Workflow.");
    }

    [Fact]
    public void A_roles_line_is_its_start_note_bullet_or_else_its_job()
    {
        const string facts = "---\ncore: false\nwrites: []\nreads: []\n---\n";
        var content = Made(
            ("Start here.md",
                "- [Alpha](Roles/Alpha.md): when `testing`\n" +
                "* [Beta](Roles/Beta.md): starred, which isn't a role's line\n"),
            ("Roles/Alpha.md", Role),
            ("Roles/Beta.md", facts + "Job\n- Build things.\n"),
            ("Roles/Gamma.md", facts + "Job\nNo bullet.\n"),
            ("Roles/Delta.md", facts + "Intro\nJob"));

        content.Roles.ToDictionary(r => r.Name, r => r.Line).Should().Equal(
            new Dictionary<string, string>
            {
                ["Alpha"] = "when testing",
                ["Beta"] = "Build things.",
                ["Gamma"] = "",
                ["Delta"] = "",
            });
    }

    [Fact]
    public void A_step_is_played_by_the_role_it_starts_with_a_link_to_or_else_by_the_user()
    {
        var content = Made(
            ("Roles/Alpha.md", Role),
            (Content.WorkflowNote,
                "Intro\n\n### The steps\n" +
                "1. [Alpha](../Roles/Alpha.md): writes the `checks`\n" +
                "2. User: says yes\n" +
                "3. [Elsewhere](https://example.com) and on\n" +
                "4. Just text\n" +
                "### After\n5. Not a step\n"));

        content.Stages.Select(s => (s.Id, s.Role, s.Summary)).Should().Equal(
            (1, "Alpha", "writes the checks"),
            (2, "", "says yes"),
            (3, "", "Elsewhere and on"),
            (4, "", "Just text"));
    }

    [Fact]
    public void A_tag_can_test_a_role_a_pack_or_a_doc_and_nothing_else()
    {
        Made(
                ("Roles/Alpha.md", Role),
                ("Guidelines/General.md", General),
                (Content.TemporaryDocsNote, DocsNote),
                ("Note.md",
                    "<!-- if Alpha -->\nA\n<!-- end -->\n" +
                    "<!-- if General -->\nG\n<!-- end -->\n" +
                    "<!-- if NOTES.md -->\nN\n<!-- end -->\n" +
                    "<!-- if workflow -->\nW\n<!-- end -->\n" +
                    "<!-- if Nobody -->\nX\n<!-- end -->\n"),
                ("Picture.svg", "<!-- if Nobody -->\n"))
            .Problems.Should().ContainSingle()
            .Which.Should().Contain("Note.md").And.Contain("\"Nobody\"");
    }

    [Fact]
    public void An_unknown_condition_in_front_matter_is_a_content_error()
    {
        Of("---\nwhen: Nobody\n---\nHello.\n").Problems.Should().Equal(
            "Note.md: unknown condition \"Nobody\" in its front matter");
    }

    [Fact]
    public void A_doc_a_role_only_reads_with_no_bullet_is_a_content_error()
    {
        Made(
                (Content.TemporaryDocsNote, "- Progress (`PROGRESS.md`): what's done.\n"),
                ("Roles/Alpha.md",
                    "---\ncore: false\nwrites: [PROGRESS.md]\nreads: [NOTES.md]\n---\n"))
            .Problems.Should().Equal(
                $"Roles/Alpha.md: NOTES.md has no bullet in {Content.TemporaryDocsNote}");
    }

    [Fact]
    public void A_workflow_role_that_doesnt_write_the_progress_doc_is_a_content_error()
    {
        Made(
                (Content.TemporaryDocsNote, DocsNote),
                ("Roles/Alpha.md",
                    "---\ncore: false\nwrites: []\nreads: [NOTES.md]\n---\n"))
            .Problems.Should().Equal(
                "Roles/Alpha.md: a workflow role that doesn't write PROGRESS.md");
    }

    [Fact]
    public void Links_in_code_arent_links()
    {
        Links.Targets("Roles/Note.md",
                "Use `[Name](Folder/Note.md)`, " +
                "or [Real](../Start%20here.md#top).\n" +
                "```\n[Fenced](No.md)\n```\n")
            .Should().Equal("Start here.md");
    }
}
