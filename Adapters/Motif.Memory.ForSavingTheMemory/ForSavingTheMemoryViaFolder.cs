namespace Motif.Memory.Adapters;

/// <summary>
/// Saves the memory into a folder chosen through the Folder Seam, such as with
/// the browser's folder picker.
/// </summary>
public sealed class ForSavingTheMemoryViaFolder(Folder folders)
    : ForSavingTheMemory
{
    private const string PickAnEmptyFolder =
        "Pick an empty folder, or a fresh git repo with nothing in it but .git.";

    private const string TryAnother = "Try another folder.";

    /// <summary>
    /// The folder the memory will be written to, kept from picking it until
    /// it's written to once.
    /// </summary>
    private ChosenFolder? picked;

    public async Task<string> Unavailable() =>
        await folders.CanChoose()
            ? ""
            : "This browser can't write to a folder.";

    public SavingWay Way
    {
        get
        {
            var name = picked?.Name ?? "";
            return new SavingWay(
                Name: "Folder",
                Icon: "folder",
                About: "Where your memory will live. It must be empty, or a "
                    + "fresh git repo with nothing in it but .git, and it "
                    + "becomes a git repo of its own. Nothing is written "
                    + "outside it.",
                Choose: name == "" ? "Pick a folder…" : "Change…",
                Where: name,
                Save: name == "" ? "Write to a folder" : $"Write to “{name}”",
                Hint: name == "" ? "Pick the folder at the top first." : "");
        }
    }

    public async Task<Choosing> ChooseWhere()
    {
        ChosenFolder folder;
        try
        {
            folder = await folders.Choose();
        }
        // Only closing the picker is a cancel, and keeps the folder picked
        // before. A folder picked but refused write access must say so.
        catch (FolderException e)
            when (e.Failure == FolderFailure.Declined)
        {
            return new Choosing.Kept();
        }
        catch (FolderException e)
        {
            picked = null;
            return NotPicked(
                $"The folder can't be opened. {Reason(e)} {TryAnother}");
        }

        picked = null;
        if (!await IsEmpty(folder))
            return NotPicked(
                $"“{folder.Name}” isn't empty, so it can't hold the memory. "
                + PickAnEmptyFolder);
        picked = folder;
        return new Choosing.Chosen();
    }

    private static Choosing.Refused NotPicked(string problem) =>
        new("The folder wasn't picked", problem);

    public async Task<Saving> Save(IReadOnlyList<MemoryFile> files)
    {
        var folder = picked;
        // Whatever happens, the folder now has files in it, or may have.
        picked = null;
        if (folder is null)
            return new Saving.Refused(
                "Pick a folder to write the memory to first.");
        // It may have filled since it was picked.
        if (!await IsEmpty(folder))
            return new Saving.Refused(
                $"“{folder.Name}” isn't empty any more, so nothing was "
                + $"written. {PickAnEmptyFolder}");

        foreach (var file in files)
        {
            try
            {
                await folder.Write(file.Path, file.Text);
            }
            catch (FolderException e)
            {
                return new Saving.Refused(
                    $"Writing to the folder failed. {file.Path}: "
                    + $"{Sentence(e.Message)} {TryAnother}");
            }
        }
        return new Saving.Saved($"Memory written to “{folder.Name}”", "");
    }

    /// <summary>
    /// Empty, or a fresh git repo: nothing in it but .git.
    /// </summary>
    private static async Task<bool> IsEmpty(ChosenFolder folder) =>
        (await folder.Names()).All(name => name == ".git");

    private static string Reason(FolderException e) =>
        e.Failure == FolderFailure.Refused
            ? "The browser didn't allow editing files in that folder."
            : Sentence(e.Message);

    private static string Sentence(string text) =>
        text.EndsWith('.') || text.EndsWith('!') || text.EndsWith('?')
            ? text
            : text + ".";
}
