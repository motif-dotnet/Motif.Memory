namespace Motif.Memory.Specs;

/// <summary>
/// The Developer's order for the workflow's stages, and the warnings beside them.
/// </summary>
public class WorkflowSpecs
{
    [Fact]
    public void Moving_a_stage_reorders_and_renumbers_the_written_steps()
    {
        var f = new Fixture();

        f.Move("Code Reviewer", -4);

        f.StageNames.Take(2).Should().Equal("Code Reviewer", "Feature Implementor");
        var steps = Fixture.Steps(f.Write());
        steps[0].Should().StartWith("1. [Code Reviewer]");
        steps[1].Should().StartWith("2. [Feature Implementor]");
        steps.Should().HaveCount(10);
    }

    [Fact]
    public void A_stage_dropped_on_another_takes_its_place_from_either_side()
    {
        var f = new Fixture();
        var before = f.StageNames;

        f.Tasks.MoveStageTo(f.Stage(before[3]).Id, f.Stage(before[0]).Id);

        f.StageNames.Take(4).Should().Equal(before[3], before[0], before[1], before[2]);

        f.Tasks.MoveStageTo(f.Stage(before[3]).Id, f.Stage(before[2]).Id);

        f.StageNames.Should().Equal(before);
    }

    [Fact]
    public void A_stage_dropped_on_itself_stays_where_it_is()
    {
        var f = new Fixture();
        var before = f.StageNames;

        f.Tasks.MoveStageTo(f.Stage(before[2]).Id, f.Stage(before[2]).Id);

        f.StageNames.Should().Equal(before);
    }

    [Fact]
    public void The_users_last_stage_is_pinned_and_no_other_is()
    {
        var f = new Fixture();

        var stages = f.Queries.Stages;

        stages[^1].Should().Match<StageChoice>(s => s.IsUsers && s.Pinned && !s.CanMoveUp && !s.CanMoveDown);
        stages[^2].CanMoveDown.Should().BeFalse();
        stages.SkipLast(1).Should().OnlyContain(s => !s.Pinned);
    }

    [Fact]
    public void The_pinned_stage_stays_last_however_stages_are_moved()
    {
        var f = new Fixture();
        var before = f.StageNames;
        var pinned = f.Queries.Stages[^1].Id;

        f.Tasks.MoveStageUp(pinned);
        f.Tasks.MoveStageTo(pinned, f.Queries.Stages[0].Id);
        f.Tasks.MoveStageDown(f.Queries.Stages[^2].Id);
        f.Tasks.MoveStageTo(f.Queries.Stages[0].Id, pinned);

        f.StageNames.Should().Equal(before);
    }

    [Fact]
    public void A_stage_can_still_move_to_just_above_the_pinned_stage()
    {
        var f = new Fixture();
        var before = f.StageNames;

        f.Tasks.MoveStageTo(f.Queries.Stages[0].Id, f.Queries.Stages[^2].Id);

        f.StageNames[^2].Should().Be(before[0]);
        f.Queries.Stages[^1].Pinned.Should().BeTrue();
    }

    [Fact]
    public void A_stage_cant_move_past_either_end()
    {
        var f = new Fixture();
        var before = f.StageNames;

        f.Tasks.MoveStageUp(f.Queries.Stages[0].Id);
        f.Tasks.MoveStageDown(f.Queries.Stages[^1].Id);

        f.StageNames.Should().Equal(before);
    }

    [Fact]
    public void Moving_a_reader_above_the_writer_of_its_doc_warns_and_moving_it_back_clears_the_warning()
    {
        var f = new Fixture();

        f.Move("Check Writer", -1);

        f.Stage("Check Writer").Warning.Should().Be("Reads the behaviour checklist, which no earlier stage writes.");
        f.Queries.Stages.Where(s => s.Name != "Check Writer").Should().OnlyContain(s => s.Warning == "");

        f.Move("Check Writer", 1);

        f.Stage("Check Writer").Warning.Should().BeEmpty();
    }

    [Fact]
    public void Unticking_the_only_writer_of_a_doc_warns_beside_its_readers()
    {
        var f = new Fixture();

        f.Tasks.TickRole("Behaviour Documenter", false);
        f.Tasks.TickRole("Bug Fixer", false);

        f.Queries.Stages.Where(s => s.Warning != "").Select(s => s.Name).Should().Equal("Check Writer", "Code Tidier");
    }

    [Fact]
    public void A_role_ticked_again_gets_its_stage_back_where_it_was_in_the_default_order()
    {
        var f = new Fixture();
        var before = f.StageNames;

        f.Tasks.TickRole("Code Reviewer", false);
        f.StageNames.Should().NotContain("Code Reviewer");
        f.Tasks.TickRole("Code Reviewer", true);

        f.StageNames.Should().Equal(before);
    }
}
