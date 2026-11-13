namespace Motif.Memory.Adapters;

/// <summary>
/// Content/, embedded in this assembly as resources, each named by its path in the folder.
/// </summary>
public sealed class BundledContentMemoryContent : MemoryContent
{
    private const string Prefix = "Content/";

    public IReadOnlyList<ContentFile> Files { get; } = Read();

    private static List<ContentFile> Read()
    {
        var assembly = typeof(BundledContentMemoryContent).Assembly;
        return assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith(Prefix, StringComparison.Ordinal))
            .Select(name =>
            {
                using var reader = new StreamReader(assembly.GetManifestResourceStream(name)!);
                return new ContentFile(name[Prefix.Length..].Replace('\\', '/'), reader.ReadToEnd());
            })
            .ToList();
    }
}
