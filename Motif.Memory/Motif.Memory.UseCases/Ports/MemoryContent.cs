namespace Motif.Memory;

/// <summary>
/// The Memory Content port: the role and pack texts the memory is made from.
/// </summary>
public interface MemoryContent
{
    IReadOnlyList<ContentFile> Files { get; }
}
