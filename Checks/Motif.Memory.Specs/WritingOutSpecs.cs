using Motif.Memory.Fakes;

namespace Motif.Memory.Specs;

/// <summary>
/// Writing the memory to a folder or a zip, and the setup note the agent finishes it with.
/// </summary>
public class WritingOutSpecs
{
    [Fact]
    public async Task A_folder_written_says_so_and_where()
    {
        var f = new Fixture();

        await f.Tasks.WriteToFolder();

        f.Queries.Written.Should().Be(Written.ToFolder);
        f.Queries.FolderName.Should().Be("memory");
        f.Queries.Problem.Should().BeEmpty();
        f.Files.Written.Keys.Should().Contain(["Start here.md", f.Queries.SetupNote]);
    }

    [Fact]
    public async Task A_folder_that_isnt_empty_gets_a_problem_and_no_files()
    {
        var f = new Fixture();
        f.Files.Picks = FakeMemoryFiles.Picking.NotEmpty;

        await f.Tasks.WriteToFolder();

        f.Queries.Written.Should().Be(Written.Not);
        f.Queries.Problem.Should().Contain("“memory” isn't empty").And.Contain("nothing in it but .git").And.Contain("nothing was written");
        f.Files.Written.Should().BeEmpty();

        f.Tasks.DismissProblem();
        f.Queries.Problem.Should().BeEmpty();
    }

    [Fact]
    public async Task A_cancelled_picker_changes_nothing()
    {
        var f = new Fixture();
        f.Files.Picks = FakeMemoryFiles.Picking.Cancel;

        await f.Tasks.WriteToFolder();

        f.Queries.Written.Should().Be(Written.Not);
        f.Queries.Problem.Should().BeEmpty();
        f.Files.Written.Should().BeEmpty();
    }

    [Fact]
    public async Task A_cancel_after_a_write_doesnt_report_the_earlier_write()
    {
        var f = new Fixture();
        await f.Tasks.WriteToFolder();
        f.Files.Picks = FakeMemoryFiles.Picking.Cancel;

        await f.Tasks.WriteToFolder();

        f.Queries.Written.Should().Be(Written.Not);
    }

    [Fact]
    public async Task A_folder_that_cant_be_written_gets_a_problem()
    {
        var f = new Fixture();
        f.Files.Picks = FakeMemoryFiles.Picking.Fail;

        await f.Tasks.WriteToFolder();

        f.Queries.Written.Should().Be(Written.Not);
        f.Queries.Problem.Should().Contain("can't be written to").And.Contain("download the zip");
    }

    [Fact]
    public async Task A_zip_can_be_downloaded_even_with_no_folder_picker()
    {
        var f = new Fixture();
        f.Files.CanPickFolder = false;

        await f.Tasks.DownloadZip();

        f.Queries.CanPickFolder.Should().BeFalse();
        f.Queries.Written.Should().Be(Written.AsZip);
        f.Queries.FolderName.Should().BeEmpty();
        f.Files.ZipName.Should().Be("agent-memory.zip");
        f.Files.Written.Keys.Should().Contain("Start here.md");
    }

    [Fact]
    public async Task Writing_keeps_the_choices()
    {
        var f = new Fixture();
        f.Tasks.TickRole("Tester", false);
        f.Move("Code Reviewer", -1);
        f.Tasks.ChooseCommitting(Committing.Never);
        var stages = f.StageNames;

        await f.Tasks.DownloadZip();

        f.Queries.Roles.Single(r => r.Name == "Tester").Ticked.Should().BeFalse();
        f.StageNames.Should().Equal(stages);
        f.Queries.Committing.Should().Be(Committing.Never);
    }

    [Fact]
    public void The_setup_note_has_the_agent_add_the_pointer_in_the_READMEs_words_then_delete_the_note()
    {
        var memory = new Fixture().Write();
        var pointer = "Before starting any task, read <path to this repo>/Start here.md and follow it.";

        memory["Set up.md"].Should().Contain(pointer).And.Contain("git init").And.Contain("Delete this note");
        memory["README.md"].Should().Contain(pointer);
        memory.Where(f => f.Key != "Set up.md").Should().OnlyContain(f => !f.Value.Contains("Set up"), "no note links to it");
    }

    [Theory]
    [InlineData(Committing.Agreed, true)]
    [InlineData(Committing.Asked, true)]
    [InlineData(Committing.Never, false)]
    public void The_setup_notes_first_commit_follows_the_committing_choice(Committing committing, bool commits)
    {
        var f = new Fixture();
        f.Tasks.ChooseCommitting(committing);

        var note = f.Write()["Set up.md"];

        note.Contains("Commit every file in the folder as one commit").Should().Be(commits);
        note.Contains("Don't commit: tell the user the memory is ready for its first commit.").Should().Be(!commits);
    }
}
