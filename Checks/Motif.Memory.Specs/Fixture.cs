global using AwesomeAssertions;
using System.IO.Compression;
using Motif.Memory.Adapters;
using Motif.Memory.Fakes;

namespace Motif.Memory.Specs;

/// <summary>
/// The application wired up by hand, on the real content or content a spec makes up. It saves through
/// the real adapters, folder then zip as the Web App registers them, over a fake browser.
/// </summary>
public sealed class Fixture
{
    public static readonly ForReadingTheContentViaBundledContent RealContent = new();

    public FakeFolder Browser { get; }

    public FakeDownload Download { get; } = new();

    public MotifMemoryApplication App { get; }

    public ForDoing Tasks => App;

    public ForSeeing Queries => App;

    public Fixture() : this(RealContent)
    {
    }

    public Fixture(ForReadingTheContent content) : this(content, new FakeFolder())
    {
    }

    private Fixture(ForReadingTheContent content, FakeFolder browser)
    {
        Browser = browser;
        App = new MotifMemoryApplication(content,
        [
            new ForSavingTheMemoryViaFolder(browser),
            new ForSavingTheMemoryViaZipDownload(Download),
        ]);
        App.Start().GetAwaiter().GetResult();
    }

    public static Fixture Of(params (string Path, string Text)[] files) =>
        new(new FakeForReadingTheContent(files.Select(f => new ContentFile(f.Path, f.Text)).ToArray()));

    public static Fixture WithNoFolderPicker() =>
        new(RealContent, new FakeFolder { CanChooseFolder = false });

    /// <summary>
    /// The files in the zip last downloaded, by path.
    /// </summary>
    public IReadOnlyDictionary<string, string> Zipped
    {
        get
        {
            using var zip = new ZipArchive(new MemoryStream(Download.SavedBytes));
            return zip.Entries.ToDictionary(e => e.FullName, e => new StreamReader(e.Open()).ReadToEnd());
        }
    }

    public SavingWay Way(string name) => Queries.Ways.Single(w => w.Name == name);

    public static IEnumerable<string> RoleNames => new Content(RealContent.Files).Roles.Select(r => r.Name);

    public IEnumerable<string> NonCoreRoles => Queries.Roles.Where(r => !r.Core).Select(r => r.Name);

    /// <summary>
    /// Writes the memory as a zip, and returns its files by path.
    /// </summary>
    public IReadOnlyDictionary<string, string> Write()
    {
        Tasks.Save("Zip").GetAwaiter().GetResult();
        return Zipped;
    }

    public void OnlyTheCore()
    {
        foreach (var role in NonCoreRoles.ToList()) Tasks.TickRole(role, false);
    }

    public IReadOnlyList<string> StageNames => Queries.Stages.Select(s => s.Name).ToList();

    public StageChoice Stage(string name) => Queries.Stages.Single(s => s.Name == name);

    public void Move(string name, int by)
    {
        for (var i = 0; i < Math.Abs(by); i++)
            if (by < 0) Tasks.MoveStageUp(Stage(name).Id);
            else Tasks.MoveStageDown(Stage(name).Id);
    }

    /// <summary>
    /// The written Workflow's steps, without their numbers checked.
    /// </summary>
    public static IReadOnlyList<string> Steps(IReadOnlyDictionary<string, string> memory) =>
        Content.StepLines(memory[Content.WorkflowNote]).ToList();

    public static IEnumerable<string> BrokenLinks(IReadOnlyDictionary<string, string> memory) =>
        memory.Where(f => f.Key.EndsWith(".md"))
            .SelectMany(f => Links.Targets(f.Key, f.Value).Where(t => !memory.ContainsKey(t)).Select(t => $"{f.Key} -> {t}"));
}
