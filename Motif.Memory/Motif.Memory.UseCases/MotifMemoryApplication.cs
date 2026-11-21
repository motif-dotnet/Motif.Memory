namespace Motif.Memory;

/// <summary>
/// Motif Memory itself: the setup choices, and saving the memory.
/// Call <see cref="Start"/> once at startup, so the ways of saving this
/// browser can use are known before the page shows.
/// </summary>
public sealed class MotifMemoryApplication : ForDoing, ForSeeing
{
    private readonly IReadOnlyList<ForSavingTheMemory> ways;
    private readonly Setup setup;
    private IReadOnlyList<ForSavingTheMemory> available = [];

    /// <param name="ways">Offered in this order, where the browser can.
    /// </param>
    public MotifMemoryApplication(
        ForReadingTheContent content, IEnumerable<ForSavingTheMemory> ways)
    {
        this.ways = ways.ToList();
        setup = new Setup(new Content(content.Files));
    }

    public async Task Start()
    {
        var can = new List<ForSavingTheMemory>();
        var cant = new List<string>();
        foreach (var way in ways)
        {
            var why = await way.Unavailable();
            if (why == "") can.Add(way);
            else cant.Add(why);
        }
        available = can;
        Unavailable = string.Join(" ", cant);
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

    public string WhyNoWorkflow =>
        Stages.All(s => s.IsUsers)
            ? "No role with a stage is ticked, so the memory has no workflow."
            : "";

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

    public IReadOnlyList<CommittingChoice> Committings =>
    [
        Choice(Committing.Agreed, "Once agreed",
            "One commit for each set of changes you agree, pushed if the "
            + "memory has a remote."),
        Choice(Committing.Asked, "Only when asked",
            "The agent commits when you tell it to."),
        Choice(Committing.Never, "Never",
            "You commit the memory yourself."),
    ];

    private CommittingChoice Choice(
        Committing value, string label, string line) =>
        new(value, label, line, value == setup.Committing);

    public IReadOnlyList<SavingWay> Ways =>
        available.Select(w => w.Way).ToList();

    public string Unavailable { get; private set; } = "";

    public SavedMemory Saved { get; private set; } = SavedMemory.None;

    public string SetupNote => Content.SetupNote;

    public Problem Problem { get; private set; } = Problem.None;

    public void TickRole(string name, bool ticked) => setup.TickRole(name, ticked);

    public void MoveStageUp(int id) => setup.MoveStage(id, -1);

    public void MoveStageDown(int id) => setup.MoveStage(id, +1);

    public void MoveStageTo(int id, int onto) => setup.MoveStageTo(id, onto);

    public void TickPack(string name, bool ticked) => setup.TickPack(name, ticked);

    public void TickGuideline(string id, bool ticked) => setup.TickGuideline(id, ticked);

    public void ChooseCommitting(Committing committing) => setup.Committing = committing;

    public async Task ChooseWhere(string way)
    {
        DismissProblem();
        if (await Way(way).ChooseWhere() is Choosing.Refused refused)
            Problem = new Problem(refused.Title, refused.Problem);
    }

    public async Task Save(string way)
    {
        DismissProblem();
        Saved = SavedMemory.None;
        switch (await Way(way).Save(setup.Write()))
        {
            // The last steps are the agent's, through the setup note, so the
            // outcome says how to start them.
            case Saving.Saved saved:
                Saved = new SavedMemory(
                    saved.Title,
                    $"{saved.Steps} To finish, tell your agent:".TrimStart(),
                    $"Read {SetupNote} in this folder and follow it.");
                break;
            case Saving.Refused refused:
                Problem = new Problem(
                    "The memory wasn't written", refused.Problem);
                break;
        }
    }

    private ForSavingTheMemory Way(string name) =>
        available.Single(w => w.Way.Name == name);

    public void DismissProblem() => Problem = Problem.None;
}
