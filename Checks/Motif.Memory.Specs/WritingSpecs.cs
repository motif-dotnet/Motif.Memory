namespace Motif.Memory.Specs;

/// <summary>
/// The memory the choices make from the real content.
/// </summary>
public class WritingSpecs
{
    [Fact]
    public void With_everything_ticked_every_file_is_written_with_no_front_matter_or_tags()
    {
        var memory = new Fixture().Write();

        memory.Keys.Should().BeEquivalentTo(
            Fixture.RealContent.Files.Select(f => f.Path));
        memory.Values.Should().OnlyContain(text => !text.StartsWith("---") && !text.Contains("<!--"));
    }

    [Fact]
    public void With_everything_ticked_only_front_matter_and_tags_are_taken_out()
    {
        var memory = new Fixture().Write();

        memory["Roles/Code Tidier.md"].Should().Contain("Leave findings that change behaviour to the [Bug Fixer](Bug%20Fixer.md): add them");
        memory["Start here.md"].Should().Contain("When your role's part of a piece of work is done, read the [Workflow](Workflow/Workflow.md)");
        memory[Content.WorkflowNote].Should().Contain(
            "- The user has tried it, and every manual test is ticked off, passed by the Tester and accepted by the user, or skipped with a reason they agreed to.");
        memory["Guidelines/General.md"].Should().Contain(
            "when there's a note for it: [C#](CSharp.md). Read the [Motif guidelines](Motif.md) when designing types, naming things or writing checks. Put");
        memory.Values.Should().OnlyContain(text => !text.Contains("\n\n\n") && text.EndsWith("\n") && !text.EndsWith("\n\n"));
    }

    [Fact]
    public void Unticking_a_role_leaves_out_its_note_its_start_line_its_stage_and_its_tagged_text()
    {
        var f = new Fixture();
        f.Tasks.TickRole("Bug Fixer", false);

        var memory = f.Write();

        memory.Keys.Should().NotContain("Roles/Bug Fixer.md");
        memory["Start here.md"].Should().NotContain("Bug Fixer");
        memory["Roles/Code Tidier.md"].Should().Contain("Leave findings that change behaviour: add them to the bug list");
        Fixture.Steps(memory).Should().NotContain(s => s.Contains("Bug Fixer"));
        Fixture.Steps(memory).Select(s => s.Split('.')[0]).Should().Equal(Enumerable.Range(1, 9).Select(n => n.ToString()));
        f.StageNames.Should().NotContain("Bug Fixer");
    }

    [Fact]
    public void A_temporary_doc_no_ticked_role_writes_is_left_out()
    {
        var f = new Fixture();
        f.Tasks.TickRole("Code Reviewer", false);

        var docs = f.Write()[Content.TemporaryDocsNote];

        docs.Should().NotContain("CODE_REVIEW.md");
        docs.Should().Contain("BUGS.md", "the Bug Fixer and Code Tidier still write it");
    }

    [Fact]
    public void Leaving_out_every_role_that_writes_a_doc_drops_the_text_tagged_with_it()
    {
        var f = new Fixture();
        foreach (var role in new[] { "Code Reviewer", "Bug Fixer", "Code Tidier" }) f.Tasks.TickRole(role, false);
        f.Tasks.TickRole("Tester", false);

        var memory = f.Write();

        memory[Content.TemporaryDocsNote].Should().NotContain("BUGS.md");
        memory[Content.WorkflowNote].Should().NotContain("Every bug is fixed").And.NotContain("The Tester has walked");
        memory[Content.WorkflowNote].Should().Contain(
            "- The user has tried it, and every manual test is ticked off, accepted by the user, or skipped with a reason they agreed to.");
    }

    [Fact]
    public void With_only_the_core_there_is_no_workflow_and_nothing_mentions_it()
    {
        var f = new Fixture();
        f.OnlyTheCore();

        var memory = f.Write();

        memory.Keys.Should().BeEquivalentTo(
            "README.md", "Start here.md", "Set up.md", "Roles/Memory Editor.md", "Roles/Memory Auditor.md",
            "Guidelines/General.md", "Guidelines/CSharp.md", "Guidelines/Motif.md");
        memory.Values.Should().OnlyContain(text => !text.Contains("Workflow/") && !text.Contains("Temporary%20Docs"));
        memory["Roles/Memory Auditor.md"].Should().NotContain("temporary docs are deleted");
        memory["Guidelines/General.md"].Should().NotContain("behaviour checklist");
        f.Queries.Stages.Should().OnlyContain(s => s.IsUsers);
    }

    public static TheoryData<string> LeftOut()
    {
        var data = new TheoryData<string>();
        foreach (var role in Fixture.RoleNames) data.Add("role " + role);
        data.Add("pack C#");
        data.Add("pack Motif");
        data.Add("everything");
        data.Add("nothing");
        return data;
    }

    [Theory]
    [MemberData(nameof(LeftOut))]
    public void No_memory_has_a_broken_link(string leftOut)
    {
        var f = new Fixture();
        if (leftOut.StartsWith("role ")) f.Tasks.TickRole(leftOut[5..], false);
        if (leftOut.StartsWith("pack ")) f.Tasks.TickPack(leftOut[5..], false);
        if (leftOut == "everything")
        {
            f.OnlyTheCore();
            f.Tasks.TickPack("C#", false);
            f.Tasks.TickPack("Motif", false);
        }

        Fixture.BrokenLinks(f.Write()).Should().BeEmpty();
    }

    [Theory]
    [InlineData(Committing.Agreed, "Commit each agreed set of edits as one commit")]
    [InlineData(Committing.Asked, "Commit only when the user asks: one commit")]
    [InlineData(Committing.Never, "Don't commit: the user commits the memory themselves.")]
    public void Each_committing_choice_writes_its_own_version_of_the_Memory_Editors_rule(Committing committing, string rule)
    {
        var f = new Fixture();
        f.Tasks.ChooseCommitting(committing);

        var editor = f.Write()["Roles/Memory Editor.md"];

        f.Queries.Committing.Should().Be(committing);
        editor.Should().Contain("- Change nothing without the user's OK. " + rule);
        editor.Split('\n').Single(l => l.StartsWith("- Change nothing")).Should().NotContainAny(
            new[] { "Commit each", "Commit only", "Don't commit" }.Where(other => !rule.StartsWith(other)));
    }

}
