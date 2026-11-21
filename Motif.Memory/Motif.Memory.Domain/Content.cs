using System.Text.RegularExpressions;

namespace Motif.Memory;

/// <summary>
/// Content/ read: its notes, and the roles, stages, packs and temporary docs
/// found in them.
/// </summary>
public sealed partial class Content
{
    public const string StartNote = "Start here.md";
    public const string SetupNote = "Set up.md";
    public const string WorkflowNote = "Workflow/Workflow.md";
    public const string TemporaryDocsNote = "Workflow/Temporary Docs.md";
    public const string StepsHeading = "### The steps";

    private const string RolesFolder = "Roles/";

    public IReadOnlyList<Note> Notes { get; }
    public IReadOnlyList<Role> Roles { get; }
    public IReadOnlyList<Stage> Stages { get; }
    public IReadOnlyList<Pack> Packs { get; }
    public IReadOnlyList<TemporaryDoc> Docs { get; }

    /// <summary>
    /// Content errors, which a spec catches: tags that don't work, and role
    /// facts that don't add up.
    /// </summary>
    public IReadOnlyList<string> Problems { get; }

    [GeneratedRegex(
        @"^- (?<name>[^(\n]+?) \(`(?<file>[^`]+)`\)", RegexOptions.Multiline)]
    private static partial Regex DocBullet();

    [GeneratedRegex(@"^(?<n>\d+)\. (?<text>.*)$")]
    private static partial Regex Step();

    [GeneratedRegex(
        @"^\[(?<name>[^\]]+)\]\((?<href>[^)]+)\):?\s*(?<rest>.*)$")]
    private static partial Regex StartsWithLink();

    [GeneratedRegex(@"\[([^\]]*)\]\([^)]*\)")]
    private static partial Regex AnyLink();

    public Content(IEnumerable<ContentFile> files)
    {
        Notes = files
            .Select(f =>
            {
                var (facts, body) = FrontMatter.Split(f.Text);
                return new Note(f.Path.Replace('\\', '/'), facts, body);
            })
            .OrderBy(n => n.Path, StringComparer.Ordinal)
            .ToList();

        Docs = DocBullet().Matches(Body(TemporaryDocsNote))
            .Select(m => new TemporaryDoc(
                m.Groups["name"].Value.Trim(), m.Groups["file"].Value))
            .ToList();
        Roles = ReadRoles().ToList();
        Stages = ReadStages().ToList();
        Packs = ReadPacks().ToList();
        Problems = FindProblems().ToList();
    }

    public string Body(string path) =>
        Notes.FirstOrDefault(n => n.Path == path)?.Body ?? "";

    public TemporaryDoc Doc(string fileName) =>
        Docs.FirstOrDefault(d => d.FileName == fileName)
        ?? new TemporaryDoc(fileName, fileName);

    public static bool IsRoleNote(Note note) =>
        note.Path.StartsWith(RolesFolder) && note.Path.EndsWith(".md");

    public static bool IsPackNote(Note note) => note.Facts.Has("pack");

    /// <summary>
    /// Markdown as plain text, for the page: tags and link targets taken out.
    /// </summary>
    public static string Plain(string markdown) =>
        AnyLink().Replace(Tags.Strip(markdown), "$1").Replace("`", "").Trim();

    private IEnumerable<Role> ReadRoles()
    {
        var startLines = Body(StartNote).Split('\n')
            .Where(l => l.StartsWith("- ") && Links.ItemTarget(StartNote, l) != "")
            .ToDictionary(
                l => Links.ItemTarget(StartNote, l),
                l => Plain(StartsWithLink().Match(l[2..]).Groups["rest"].Value));

        foreach (var note in Notes.Where(IsRoleNote))
        {
            var name = note.Path[RolesFolder.Length..^".md".Length];
            var line = startLines.GetValueOrDefault(note.Path, "");
            if (line == "") line = Plain(JobLine(note.Body));
            yield return new Role(
                name,
                note.Path,
                note.Facts.Flag("core"),
                note.Facts.List("writes"),
                note.Facts.List("reads"),
                line);
        }
    }

    private static string JobLine(string body)
    {
        var lines = body.Split('\n');
        var job = Array.IndexOf(lines, "Job");
        return job >= 0 && job + 1 < lines.Length && lines[job + 1].StartsWith("- ")
            ? lines[job + 1][2..]
            : "";
    }

    /// <summary>
    /// The numbered steps under the Workflow's steps heading, in their
    /// default order.
    /// </summary>
    private IEnumerable<Stage> ReadStages()
    {
        var id = 0;
        foreach (var line in StepLines(Body(WorkflowNote)))
        {
            var text = Step().Match(line).Groups["text"].Value;
            var link = StartsWithLink().Match(text);
            var role = link.Success
                ? Roles.FirstOrDefault(r =>
                    r.Path == Links.Resolve(WorkflowNote, link.Groups["href"].Value))
                : null;
            var summary =
                role is not null ? link.Groups["rest"].Value
                : text.StartsWith("User:") ? text["User:".Length..]
                : text;
            yield return new Stage(++id, role?.Name ?? "", text, Plain(summary));
        }
    }

    /// <summary>
    /// The lines of the numbered list under the steps heading.
    /// </summary>
    public static IEnumerable<string> StepLines(string workflow) =>
        workflow.Split('\n')
            .SkipWhile(l => l != StepsHeading)
            .Skip(1)
            .TakeWhile(l => !l.StartsWith('#'))
            .Where(l => Step().IsMatch(l));

    private IEnumerable<Pack> ReadPacks()
    {
        foreach (var note in Notes.Where(IsPackNote))
        {
            var name = note.Facts.Get("pack");
            var guidelines = Guidelines.Read(note.Body)
                .Select(g => new Guideline(
                    $"{name}/{g.Index}", g.Section, Plain(g.Text)))
                .ToList();
            yield return new Pack(
                name, note.Path, note.Facts.Flag("optional"), guidelines);
        }
    }

    /// <summary>
    /// The names a tag can test, besides each role, pack and temporary doc.
    /// </summary>
    public static readonly IReadOnlyList<string> OtherConditions =
        ["workflow", "commit: agreed", "commit: asked", "commit: never"];

    public bool IsKnownCondition(string name) =>
        OtherConditions.Contains(name)
        || Roles.Any(r => r.Name == name)
        || Packs.Any(p => p.Name == name)
        || Docs.Any(d => d.FileName == name);

    private IEnumerable<string> FindProblems()
    {
        foreach (var note in Notes.Where(n => n.Path.EndsWith(".md")))
        {
            var tags = Tags.Problems(note.Path, note.Body, IsKnownCondition);
            foreach (var problem in tags) yield return problem;
            var when = note.Facts.Get("when");
            if (note.Facts.Has("when") && !IsKnownCondition(when))
                yield return $"{note.Path}: unknown condition \"{when}\" "
                    + "in its front matter";
        }

        foreach (var note in Notes.Where(n => IsRoleNote(n) && !n.Facts.Has("core")))
            yield return $"{note.Path}: a role note with no front matter";

        foreach (var role in Roles)
        {
            var named = role.Writes.Concat(role.Reads);
            foreach (var doc in named.Where(d => Docs.All(k => k.FileName != d)))
                yield return $"{role.Path}: {doc} has no bullet in "
                    + TemporaryDocsNote;
        }

        foreach (var role in Roles.Where(r =>
            r.InWorkflow && !r.Writes.Contains("PROGRESS.md")))
            yield return $"{role.Path}: a workflow role that doesn't write "
                + "PROGRESS.md";
    }
}
