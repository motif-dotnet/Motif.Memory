namespace Motif.Memory;

/// <summary>
/// The For reading the content port: the role and pack texts the memory is made from.
/// </summary>
public interface ForReadingTheContent
{
    IReadOnlyList<ContentFile> Files { get; }
}
