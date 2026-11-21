namespace Motif.Memory.Fakes;

/// <summary>
/// Content a spec makes up, for format rules the real content doesn't use.
/// </summary>
public sealed class FakeForReadingTheContent(params ContentFile[] files) : ForReadingTheContent
{
    public IReadOnlyList<ContentFile> Files { get; } = files;
}
