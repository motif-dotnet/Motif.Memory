namespace Motif.Memory;

/// <summary>
/// Motif Memory itself: the setup choices, and writing the memory out.
/// </summary>
public sealed class MotifMemoryApplication : UserTasks, UserQueries
{
    public const string ZipName = "agent-memory.zip";

    private readonly MemoryFiles files;
    private readonly Setup setup;

    public MotifMemoryApplication(MemoryContent content, MemoryFiles files)
    {
        this.files = files;
        setup = new Setup(new Content(content.Files));
    }

    public IReadOnlyList<RoleChoice> Roles =>
        setup.Content.Roles
            .OrderBy(r => r.Core)
            .ThenBy(r => StartNoteOrder(r))
            .Select(r => new RoleChoice(r.Name, r.Line, setup.IsTicked(r), r.Core))
            .ToList();

    /// <summary>
    /// Roles come in the start note's order, which is the order they're used in.
    /// </summary>
    private int StartNoteOrder(Role role)
    {
        var at = setup.Content.Body(Content.StartNote).IndexOf($"]({role.Path.Replace(" ", "%20")})", StringComparison.Ordinal);
        return at < 0 ? int.MaxValue : at;
    }

    public IReadOnlyList<StageChoice> Stages
    {
        get
        {
            var stages = setup.Stages;
            return stages
                .Select((s, i) =>
                {
                    var pinned = setup.IsPinned(s);
                    var down = !pinned && i < stages.Count - 1 && !setup.IsPinned(stages[i + 1]);
                    return new StageChoice(s.Id, s.Name, s.Summary, s.IsUsers, Warning(s), pinned, !pinned && i > 0, down);
                })
                .ToList();
        }
    }

    private string Warning(Stage stage)
    {
        var docs = setup.Unwritten(stage);
        if (docs.Count == 0) return "";
        var names = string.Join(" and ", docs.Select(d => $"the {char.ToLowerInvariant(d.Name[0])}{d.Name[1..]}"));
        return $"Reads {names}, which no earlier stage writes.";
    }

    public IReadOnlyList<PackChoice> Packs =>
        setup.Content.Packs
            .OrderBy(p => p.Optional)
            .ThenBy(p => p.Name, StringComparer.Ordinal)
            .Select(p => new PackChoice(p.Name, p.Optional, setup.IsTicked(p),
                p.Guidelines
                    .GroupBy(g => g.Section)
                    .Select(s => new GuidelineSection(s.Key, s.Select(g => new GuidelineChoice(g.Id, g.Text, setup.IsTicked(g))).ToList()))
                    .ToList()))
            .ToList();

    public Committing Committing => setup.Committing;

    public bool CanPickFolder => files.CanPickFolder;

    public Written Written { get; private set; } = Written.Not;

    public string FolderName { get; private set; } = "";

    public string SetupNote => Content.SetupNote;

    public string Problem { get; private set; } = "";

    public void TickRole(string name, bool ticked) => setup.TickRole(name, ticked);

    public void MoveStageUp(int id) => setup.MoveStage(id, -1);

    public void MoveStageDown(int id) => setup.MoveStage(id, +1);

    public void MoveStageTo(int id, int onto) => setup.MoveStageTo(id, onto);

    public void TickPack(string name, bool ticked) => setup.TickPack(name, ticked);

    public void TickGuideline(string id, bool ticked) => setup.TickGuideline(id, ticked);

    public void ChooseCommitting(Committing committing) => setup.Committing = committing;

    public async Task WriteToFolder()
    {
        Problem = "";
        Written = Written.Not;
        switch (await files.WriteToFolder(setup.Write()))
        {
            case FolderWrite.Written written:
                FolderName = written.FolderName;
                Written = Written.ToFolder;
                break;
            case FolderWrite.NotEmpty notEmpty:
                Problem = $"“{notEmpty.FolderName}” isn't empty, so nothing was written. Pick an empty folder, or a fresh git repo with nothing in it but .git.";
                break;
            case FolderWrite.Failed failed:
                Problem = $"Writing to the folder failed. {failed.Reason} Try another folder, or download the zip.";
                break;
        }
    }

    public async Task DownloadZip()
    {
        Problem = "";
        Written = Written.Not;
        await files.DownloadZip(ZipName, setup.Write());
        FolderName = "";
        Written = Written.AsZip;
    }

    public void DismissProblem() => Problem = "";
}
