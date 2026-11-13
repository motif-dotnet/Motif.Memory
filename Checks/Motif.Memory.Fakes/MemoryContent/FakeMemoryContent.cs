namespace Motif.Memory.Fakes;

/// <summary>
/// Content a spec makes up, for format rules the real content doesn't use.
/// </summary>
public sealed class FakeMemoryContent(params ContentFile[] files) : MemoryContent
{
    public IReadOnlyList<ContentFile> Files { get; } = files;
}
