using System.Text.RegularExpressions;

namespace Motif.Memory;

/// <summary>
/// When the memory's own changes are committed.
/// </summary>
public enum Committing
{
    /// <summary>
    /// Once the user has agreed them: one commit per set of edits.
    /// </summary>
    Agreed,

    /// <summary>
    /// Only when the user asks.
    /// </summary>
    Asked,

    /// <summary>
    /// Never: the Developer commits.
    /// </summary>
    Never,
}

/// <summary>
/// The Developer's choices, every one with its default, and the memory they make from the content.
/// </summary>
public sealed partial class Setup
{
    private readonly HashSet<string> untickedRoles = [];
    private readonly HashSet<string> leftOutPacks = [];
    private readonly HashSet<string> untickedGuidelines = [];
    private readonly List<int> order;

    public Setup(Content content)
    {
        Content = content;
        order = content.Stages.Select(s => s.Id).ToList();
    }

    public Content Content { get; }

    public Committing Committing { get; set; } = Committing.Agreed;

    public bool IsTicked(Role role) => role.Core || !untickedRoles.Contains(role.Name);

    private bool IsTicked(string roleName) => Content.Roles.FirstOrDefault(r => r.Name == roleName) is { } role && IsTicked(role);

    /// <summary>
    /// Core roles can't be unticked. A role ticked again gets its stage back where it was in the default order.
    /// </summary>
    public void TickRole(string name, bool ticked)
    {
        if (Content.Roles.FirstOrDefault(r => r.Name == name) is not { Core: false } role || IsTicked(role) == ticked) return;
        if (!ticked)
        {
            untickedRoles.Add(name);
            order.RemoveAll(id => Stage(id).Role == name);
            return;
        }
        untickedRoles.Remove(name);
        foreach (var stage in Content.Stages.Where(s => s.Role == name))
        {
            var before = order.FindIndex(id => id > stage.Id);
            order.Insert(before < 0 ? order.Count : before, stage.Id);
        }
    }

    private Stage Stage(int id) => Content.Stages.First(s => s.Id == id);

    /// <summary>
    /// The stages of the roles ticked, and the user's, in the Developer's order.
    /// </summary>
    public IReadOnlyList<Stage> Stages => order.Select(Stage).ToList();

    /// <summary>
    /// The user's stage that ends the default order stays last: the work isn't finished until they've tried it.
    /// </summary>
    public bool IsPinned(Stage stage) => stage.IsUsers && stage.Id == Content.Stages[^1].Id;

    public void MoveStage(int id, int by)
    {
        var at = order.IndexOf(id);
        var to = at + by;
        if (at < 0 || to < 0 || to >= order.Count) return;
        if (IsPinned(Stage(id)) || IsPinned(Stage(order[to]))) return;
        order.RemoveAt(at);
        order.Insert(to, id);
    }

    public void MoveStageTo(int id, int onto)
    {
        if (order.Contains(id) && order.Contains(onto)) MoveStage(id, order.IndexOf(onto) - order.IndexOf(id));
    }

    /// <summary>
    /// The temporary docs a stage reads that no earlier stage writes.
    /// </summary>
    public IReadOnlyList<TemporaryDoc> Unwritten(Stage stage)
    {
        if (stage.IsUsers) return [];
        var earlier = Stages.TakeWhile(s => s.Id != stage.Id).Where(s => !s.IsUsers).Select(s => Role(s.Role)).ToList();
        return Role(stage.Role).Reads
            .Where(doc => !earlier.Any(r => r.Writes.Contains(doc)))
            .Select(Content.Doc)
            .ToList();
    }

    private Role Role(string name) => Content.Roles.First(r => r.Name == name);

    public bool IsTicked(Pack pack) => !pack.Optional || !leftOutPacks.Contains(pack.Name);

    /// <summary>
    /// Only optional packs can be left out.
    /// </summary>
    public void TickPack(string name, bool ticked)
    {
        if (Content.Packs.FirstOrDefault(p => p.Name == name) is not { Optional: true }) return;
        if (ticked) leftOutPacks.Remove(name);
        else leftOutPacks.Add(name);
    }

    public bool IsTicked(Guideline guideline) => !untickedGuidelines.Contains(guideline.Id);

    public void TickGuideline(string id, bool ticked)
    {
        if (ticked) untickedGuidelines.Remove(id);
        else untickedGuidelines.Add(id);
    }

    /// <summary>
    /// A pack is written if it's always written, or it's ticked with at least one guideline ticked.
    /// </summary>
    public bool IsWritten(Pack pack) => !pack.Optional || (IsTicked(pack) && pack.Guidelines.Any(IsTicked));

    public bool HasWorkflow => Content.Roles.Any(r => r.InWorkflow && IsTicked(r));

    /// <summary>
    /// Whether a tag's condition, or a note's <c>when:</c>, holds for these choices.
    /// </summary>
    public bool Holds(string condition) => condition switch
    {
        "workflow" => HasWorkflow,
        "commit: agreed" => Committing == Committing.Agreed,
        "commit: asked" => Committing == Committing.Asked,
        "commit: never" => Committing == Committing.Never,
        _ when Content.Docs.Any(d => d.FileName == condition) =>
            Content.Roles.Any(r => IsTicked(r) && r.Writes.Contains(condition)),
        _ when Content.Packs.FirstOrDefault(p => p.Name == condition) is { } pack => IsWritten(pack),
        _ => IsTicked(condition),
    };

    private bool IsWritten(Note note)
    {
        if (Content.IsRoleNote(note)) return IsTicked(note.Path["Roles/".Length..^".md".Length]);
        if (Content.IsPackNote(note)) return IsWritten(Content.Packs.First(p => p.Path == note.Path));
        return !note.Facts.Has("when") || Holds(note.Facts.Get("when"));
    }

    /// <summary>
    /// The memory these choices make: the notes chosen, each tailored to the choices.
    /// </summary>
    public IReadOnlyList<MemoryFile> Write()
    {
        var notes = Content.Notes.Where(IsWritten).ToList();
        var written = notes.Select(n => n.Path).ToHashSet();
        return notes
            .Select(n => new MemoryFile(n.Path, n.Path.EndsWith(".md") ? Tailor(n, written) : n.Body))
            .ToList();
    }

    private string Tailor(Note note, HashSet<string> written)
    {
        var text = note.Body;
        if (Content.IsPackNote(note))
        {
            var pack = Content.Packs.First(p => p.Path == note.Path);
            text = Guidelines.Keep(text, index => IsTicked(pack.Guidelines[index - 1]));
        }
        if (note.Path == Content.WorkflowNote) text = Reorder(text);
        text = Tags.Apply(text, Holds);
        text = DropItemsLinkingNowhere(note.Path, text, written);
        if (Content.IsPackNote(note)) text = Guidelines.DropEmptySections(text);
        if (note.Path == Content.WorkflowNote) text = Renumber(text);
        return CloseUp(text);
    }

    [GeneratedRegex(@"^\d+\. ")]
    private static partial Regex StepNumber();

    /// <summary>
    /// The Workflow's steps, in the Developer's order.
    /// </summary>
    private string Reorder(string workflow)
    {
        var lines = workflow.Split('\n').ToList();
        var heading = lines.IndexOf(Content.StepsHeading);
        if (heading < 0) return workflow;
        var first = lines.FindIndex(heading, l => StepNumber().IsMatch(l));
        var last = first;
        while (last + 1 < lines.Count && StepNumber().IsMatch(lines[last + 1])) last++;
        lines.RemoveRange(first, last - first + 1);
        lines.InsertRange(first, Stages.Select(s => $"0. {s.Text}"));
        return string.Join('\n', lines);
    }

    private static string Renumber(string workflow)
    {
        var lines = workflow.Split('\n');
        var inSteps = false;
        var n = 0;
        for (var i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith('#')) inSteps = lines[i] == Content.StepsHeading;
            else if (inSteps && StepNumber().IsMatch(lines[i])) lines[i] = StepNumber().Replace(lines[i], $"{++n}. ");
        }
        return string.Join('\n', lines);
    }

    /// <summary>
    /// A list item that starts with a link to a note that isn't written goes, with any lines indented under it.
    /// </summary>
    private static string DropItemsLinkingNowhere(string path, string text, HashSet<string> written)
    {
        var kept = new List<string>();
        var dropIndent = -1;
        foreach (var line in text.Split('\n'))
        {
            var indent = line.Length - line.TrimStart().Length;
            if (dropIndent >= 0 && line.Trim() != "" && indent > dropIndent) continue;
            dropIndent = -1;
            var target = Links.ItemTarget(path, line);
            if (target != "" && !written.Contains(target))
            {
                dropIndent = indent;
                continue;
            }
            kept.Add(line);
        }
        return string.Join('\n', kept);
    }

    /// <summary>
    /// Blank lines left behind by dropped text close up to one, and the note ends with one line break.
    /// </summary>
    private static string CloseUp(string text)
    {
        var lines = new List<string>();
        foreach (var line in text.Split('\n'))
            if (line.Trim() != "" || (lines.Count > 0 && lines[^1].Trim() != "")) lines.Add(line);
        while (lines.Count > 0 && lines[^1].Trim() == "") lines.RemoveAt(lines.Count - 1);
        return string.Join('\n', lines) + "\n";
    }
}
