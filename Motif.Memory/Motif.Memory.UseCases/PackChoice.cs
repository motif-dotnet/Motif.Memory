namespace Motif.Memory;

public sealed record PackChoice(
    string Name,
    bool Optional,
    bool Ticked,
    IReadOnlyList<GuidelineSection> Sections)
{
    public int Count => Sections.Sum(s => s.Guidelines.Count);

    public int TickedCount =>
        Sections.Sum(s => s.Guidelines.Count(g => g.Ticked));
}

public sealed record GuidelineSection(
    string Name, IReadOnlyList<GuidelineChoice> Guidelines);

public sealed record GuidelineChoice(string Id, string Text, bool Ticked);
