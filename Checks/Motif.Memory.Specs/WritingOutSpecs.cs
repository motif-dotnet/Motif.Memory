using Motif.Memory.Adapters;
using Motif.Memory.Fakes;

namespace Motif.Memory.Specs;

/// <summary>
/// Writing the memory to a folder or a zip, and the setup note the agent finishes it with.
/// </summary>
public class WritingOutSpecs
{
    [Fact]
    public void A_folder_is_offered_first_then_the_zip()
    {
        var f = new Fixture();

        f.Queries.Ways.Select(w => w.Name).Should().Equal("Folder", "Zip");
        f.Queries.Unavailable.Should().BeEmpty();
    }

    [Fact]
    public void A_browser_with_no_folder_picker_is_offered_only_the_zip_and_told_why()
    {
        var f = Fixture.WithNoFolderPicker();

        f.Queries.Ways.Select(w => w.Name).Should().Equal("Zip");
        f.Queries.Unavailable.Should().Be("This browser can't write to a folder.");
    }

    [Fact]
    public void Before_a_folder_is_picked_its_way_asks_for_one()
    {
        var f = new Fixture();

        var folder = f.Way("Folder");

        folder.Where.Should().BeEmpty();
        folder.Choose.Should().Be("Pick a folder…");
        folder.Save.Should().Be("Write to a folder");
        folder.CanSave.Should().BeFalse();
        folder.Hint.Should().Be("Pick the folder at the top first.");
    }

    [Fact]
    public async Task Once_a_folder_is_picked_its_way_names_it()
    {
        var f = new Fixture();

        await f.Tasks.ChooseWhere("Folder");

        var folder = f.Way("Folder");
        folder.Where.Should().Be("memory");
        folder.Choose.Should().Be("Change…");
        folder.Save.Should().Be("Write to “memory”");
        folder.CanSave.Should().BeTrue();
        folder.Hint.Should().BeEmpty();
    }

    [Fact]
    public async Task The_zip_has_nowhere_to_choose_and_is_always_ready()
    {
        var f = new Fixture();

        await f.Tasks.ChooseWhere("Zip");

        var zip = f.Way("Zip");
        zip.About.Should().Be("Downloaded as agent-memory.zip, to unzip into a folder that stays put.");
        zip.Choose.Should().BeEmpty();
        zip.Save.Should().Be("Download zip");
        zip.CanSave.Should().BeTrue();
        f.Queries.Problem.Should().Be(Problem.None);
    }

    [Fact]
    public async Task A_folder_that_isnt_picked_is_titled_so()
    {
        var f = new Fixture();
        f.Browser.Chosen.Files["notes.md"] = "";

        await f.Tasks.ChooseWhere("Folder");

        f.Queries.Problem.Title.Should().Be("The folder wasn't picked");

        f.Tasks.DismissProblem();
        f.Queries.Problem.Title.Should().BeEmpty();
    }

    [Fact]
    public async Task A_memory_that_isnt_written_is_titled_so()
    {
        var f = new Fixture();

        await f.Tasks.Save("Folder");

        f.Queries.Problem.Title.Should().Be("The memory wasn't written");
    }

    [Fact]
    public async Task A_zip_downloaded_says_to_unzip_it_then_tell_the_agent()
    {
        var f = new Fixture();

        await f.Tasks.Save("Zip");

        f.Queries.Saved.Should().Be(new SavedMemory(
            "Memory downloaded",
            "Unzip agent-memory.zip into a folder that stays put. To finish, tell your agent:",
            "Read Set up.md in this folder and follow it."));
    }

    [Fact]
    public async Task A_folder_picked_is_named_before_anything_is_written()
    {
        var f = new Fixture();

        await f.Tasks.ChooseWhere("Folder");

        f.Way("Folder").Where.Should().Be("memory");
        f.Queries.Problem.Should().Be(Problem.None);
        f.Browser.Chosen.Files.Should().BeEmpty();
    }

    [Fact]
    public async Task A_folder_that_isnt_empty_is_refused_as_soon_as_its_picked()
    {
        var f = new Fixture();
        f.Browser.Chosen.Files["notes.md"] = "";

        await f.Tasks.ChooseWhere("Folder");

        f.Way("Folder").Where.Should().BeEmpty();
        f.Queries.Problem.Text.Should().Contain("“memory” isn't empty").And.Contain("nothing in it but .git");

        f.Tasks.DismissProblem();
        f.Queries.Problem.Should().Be(Problem.None);
    }

    [Fact]
    public async Task A_cancelled_picker_keeps_the_folder_already_picked()
    {
        var f = new Fixture();
        await f.Tasks.ChooseWhere("Folder");
        f.Browser.ChoosingFails(FolderFailure.Declined);

        await f.Tasks.ChooseWhere("Folder");

        f.Way("Folder").Where.Should().Be("memory");
        f.Queries.Problem.Should().Be(Problem.None);
    }

    [Fact]
    public async Task A_folder_that_cant_be_picked_gets_a_problem()
    {
        var f = new Fixture();
        f.Browser.ChoosingFails(FolderFailure.Failed);

        await f.Tasks.ChooseWhere("Folder");

        f.Way("Folder").Where.Should().BeEmpty();
        f.Queries.Problem.Text.Should().Contain("can't be opened").And.Contain("Try another folder.");
    }

    [Fact]
    public async Task A_folder_the_browser_wont_allow_editing_says_so()
    {
        var f = new Fixture();
        f.Browser.ChoosingFails(FolderFailure.Refused);

        await f.Tasks.ChooseWhere("Folder");

        f.Queries.Problem.Text.Should().Contain("The browser didn't allow editing files in that folder.");
    }

    [Theory]
    [InlineData("It's locked", "It's locked.")]
    [InlineData("It's locked.", "It's locked.")]
    [InlineData("Is it locked?", "Is it locked?")]
    [InlineData("It's locked!", "It's locked!")]
    public async Task The_browsers_reason_is_given_as_one_sentence(string message, string given)
    {
        var f = new Fixture();
        f.Browser.ChoosingFails(FolderFailure.Failed, message);

        await f.Tasks.ChooseWhere("Folder");

        f.Queries.Problem.Text.Should().Contain($" {given} Try another folder");
    }

    [Fact]
    public async Task A_fresh_git_repo_can_hold_the_memory()
    {
        var f = new Fixture();
        f.Browser.Chosen.Files[".git/HEAD"] = "ref: refs/heads/main";

        await f.Tasks.ChooseWhere("Folder");
        await f.Tasks.Save("Folder");

        f.Queries.Problem.Should().Be(Problem.None);
        f.Queries.Saved.Title.Should().Be("Memory written to “memory”");
    }

    [Fact]
    public async Task A_folder_written_says_so_and_where()
    {
        var f = new Fixture();
        await f.Tasks.ChooseWhere("Folder");

        await f.Tasks.Save("Folder");

        f.Queries.Saved.Should().Be(new SavedMemory(
            "Memory written to “memory”",
            "To finish, tell your agent:",
            "Read Set up.md in this folder and follow it."));
        f.Queries.Problem.Should().Be(Problem.None);
        f.Browser.Chosen.Files.Keys.Should().Contain(["Start here.md", f.Queries.SetupNote]);
    }

    [Fact]
    public async Task A_folder_written_to_must_be_picked_again_to_write_again()
    {
        var f = new Fixture();
        await f.Tasks.ChooseWhere("Folder");

        await f.Tasks.Save("Folder");

        f.Way("Folder").Where.Should().BeEmpty();
    }

    [Fact]
    public async Task Nothing_is_written_before_a_folder_is_picked()
    {
        var f = new Fixture();

        await f.Tasks.Save("Folder");

        f.Queries.Saved.Should().Be(SavedMemory.None);
        f.Queries.Problem.Text.Should().Contain("Pick a folder");
        f.Browser.Chosen.Files.Should().BeEmpty();
    }

    [Fact]
    public async Task A_folder_filled_after_its_picked_gets_a_problem_and_no_files()
    {
        var f = new Fixture();
        await f.Tasks.ChooseWhere("Folder");
        f.Browser.Chosen.Files["notes.md"] = "";

        await f.Tasks.Save("Folder");

        f.Queries.Saved.Should().Be(SavedMemory.None);
        f.Queries.Problem.Text.Should().Contain("“memory” isn't empty").And.Contain("nothing in it but .git").And.Contain("nothing was written");
        f.Browser.Chosen.Files.Keys.Should().Equal("notes.md");
    }

    [Fact]
    public async Task A_folder_that_cant_be_written_gets_a_problem()
    {
        var f = new Fixture();
        await f.Tasks.ChooseWhere("Folder");
        f.Browser.Chosen.Refuse("Start here.md", FolderFailure.Refused, "Not allowed");

        await f.Tasks.Save("Folder");

        f.Queries.Saved.Should().Be(SavedMemory.None);
        f.Queries.Problem.Text.Should().Contain("Writing to the folder failed").And.Contain("Try another folder.");
    }

    [Fact]
    public async Task A_file_that_cant_be_written_is_named_with_the_browsers_reason()
    {
        var f = new Fixture();
        await f.Tasks.ChooseWhere("Folder");
        f.Browser.Chosen.Refuse("Start here.md", FolderFailure.Failed, "The disk is full");

        await f.Tasks.Save("Folder");

        f.Queries.Problem.Text.Should().Contain("Start here.md: The disk is full.");
    }

    [Fact]
    public async Task A_failed_write_after_a_write_doesnt_report_the_earlier_write()
    {
        var f = new Fixture();
        await f.Tasks.ChooseWhere("Folder");
        await f.Tasks.Save("Folder");
        f.Browser.Chosen = new FakeChosenFolder("memory");
        f.Browser.Chosen.Refuse("Start here.md", FolderFailure.Refused, "Not allowed");
        await f.Tasks.ChooseWhere("Folder");

        await f.Tasks.Save("Folder");

        f.Queries.Saved.Should().Be(SavedMemory.None);
    }

    [Fact]
    public async Task A_zip_can_be_downloaded_even_with_no_folder_picker()
    {
        var f = Fixture.WithNoFolderPicker();

        await f.Tasks.Save("Zip");

        f.Queries.Ways.Select(w => w.Name).Should().Equal("Zip");
        f.Queries.Saved.Title.Should().Be("Memory downloaded");
        f.Download.SavedName.Should().Be("agent-memory.zip");
        f.Zipped.Keys.Should().Contain("Start here.md");
    }

    [Fact]
    public async Task Writing_keeps_the_choices()
    {
        var f = new Fixture();
        f.Tasks.TickRole("Tester", false);
        f.Move("Code Reviewer", -1);
        f.Tasks.ChooseCommitting(Committing.Never);
        var stages = f.StageNames;

        await f.Tasks.Save("Zip");

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

    [Fact]
    public void The_folder_says_what_it_must_be_and_that_nothing_is_written_outside_it()
    {
        var f = new Fixture();

        f.Way("Folder").About.Should().Be(
            "Where your memory will live. It must be empty, or a fresh git "
            + "repo with nothing in it but .git, and it becomes a git repo of "
            + "its own. Nothing is written outside it.");
    }
}
