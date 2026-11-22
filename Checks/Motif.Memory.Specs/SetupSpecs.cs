namespace Motif.Memory.Specs;

/// <summary>
/// The setup page's choices: their defaults, and the roles that can't be
/// unticked.
/// </summary>
public class SetupSpecs
{
    [Fact]
    public void Every_role_is_ticked_at_first_with_the_core_last()
    {
        var f = new Fixture();

        f.Seeing.Roles.Should().OnlyContain(r => r.Ticked);
        f.Seeing.Roles.Select(r => r.Name).Should().Equal(
            "Design Griller", "Feature Implementor", "Code Reviewer",
            "Code Tidier", "Bug Fixer", "Review Responder",
            "Behaviour Documenter", "Check Writer", "Tester", "Coordinator",
            "Memory Editor", "Memory Auditor");
        f.Seeing.Roles.Where(r => r.Core).Select(r => r.Name)
            .Should().Equal("Memory Editor", "Memory Auditor");
        f.Seeing.Roles.Should().OnlyContain(r => r.Line != "");
    }

    [Fact]
    public void The_stages_start_in_the_workflows_order_with_no_warnings()
    {
        var f = new Fixture();

        f.StageNames.Should().Equal(
            "Design Griller", "Feature Implementor", "User", "Behaviour Documenter",
            "Check Writer", "Code Reviewer", "Bug Fixer", "Code Tidier", "User",
            "Tester", "User");
        f.Seeing.Stages.Should().OnlyContain(s => s.Warning == "");
        f.Seeing.Stages.First().CanMoveUp.Should().BeFalse();
        f.Seeing.Stages.Last().CanMoveDown.Should().BeFalse();
    }

    [Fact]
    public void Every_pack_and_guideline_is_ticked_at_first_and_only_General_is_always_written()
    {
        var f = new Fixture();

        f.Seeing.Packs.Select(p => p.Name)
            .Should().Equal("General", "C#", "Motif");
        f.Seeing.Packs.Should().OnlyContain(
            p => p.Ticked && p.TickedCount == p.Count && p.Count > 0);
        f.Seeing.Packs.Where(p => !p.Optional).Select(p => p.Name)
            .Should().Equal("General");
        f.Seeing.Packs.Single(p => p.Name == "General").Sections
            .Select(s => s.Name).Should().Equal(
                "Hard Rules", "Prefer X over Y", "Prefer", "Avoid");
    }

    [Fact]
    public void Committing_is_once_agreed_and_nothing_is_written_yet()
    {
        var f = new Fixture();

        f.Seeing.Committing.Should().Be(Committing.Agreed);
        f.Seeing.Saved.Should().Be(SavedMemory.None);
        f.Seeing.Problem.Should().Be(Problem.None);
    }

    [Fact]
    public void Each_committing_choice_says_what_it_means()
    {
        var f = new Fixture();

        f.Doing.ChooseCommitting(Committing.Never);

        f.Seeing.Committings.Should().Equal(
            new CommittingChoice(Committing.Agreed, "Once agreed",
                "One commit for each set of changes you agree, " +
                "pushed if the memory has a remote.", false),
            new CommittingChoice(Committing.Asked, "Only when asked",
                "The agent commits when you tell it to.", false),
            new CommittingChoice(Committing.Never, "Never",
                "You commit the memory yourself.", true));
    }

    [Fact]
    public void Core_roles_cant_be_unticked()
    {
        var f = new Fixture();

        f.Doing.TickRole("Memory Editor", false);
        f.Doing.TickRole("Memory Auditor", false);

        f.Seeing.Roles.Where(r => r.Core).Should().OnlyContain(r => r.Ticked);
        f.Write().Keys.Should().Contain(
            ["Roles/Memory Editor.md", "Roles/Memory Auditor.md"]);
    }

    [Fact]
    public void The_General_pack_cant_be_left_out()
    {
        var f = new Fixture();

        f.Doing.TickPack("General", false);

        f.Seeing.Packs.Single(p => p.Name == "General")
            .Ticked.Should().BeTrue();
        f.Write().Keys.Should().Contain("Guidelines/General.md");
    }
}
