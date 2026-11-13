using System.Text.RegularExpressions;

namespace Motif.Memory;

/// <summary>
/// The relative Markdown links between a memory's notes. Links in code aren't links.
/// </summary>
public static partial class Links
{
    [GeneratedRegex(@"\[[^\]]*\]\(([^)\s]+)\)")]
    private static partial Regex Link();

    [GeneratedRegex(@"^\s*(?:[-*+]|\d+\.)\s+\[[^\]]*\]\(([^)\s]+)\)")]
    private static partial Regex ItemStartingWithLink();

    [GeneratedRegex(@"`[^`\n]*`")]
    private static partial Regex CodeSpan();

    /// <summary>
    /// The paths, from the memory's root, of the notes a note links to.
    /// </summary>
    public static IEnumerable<string> Targets(string notePath, string text)
    {
        var inFence = false;
        foreach (var line in text.Split('\n'))
        {
            if (line.TrimStart().StartsWith("```")) inFence = !inFence;
            if (inFence) continue;
            foreach (Match link in Link().Matches(CodeSpan().Replace(line, "")))
                if (Resolve(notePath, link.Groups[1].Value) is { Length: > 0 } target) yield return target;
        }
    }

    /// <summary>
    /// The path, from the memory's root, of the note a list item starts with a link to, or "" if it doesn't.
    /// </summary>
    public static string ItemTarget(string notePath, string line)
    {
        var match = ItemStartingWithLink().Match(line);
        return match.Success ? Resolve(notePath, match.Groups[1].Value) : "";
    }

    /// <summary>
    /// A relative link's target from the memory's root, or "" for a link off the memory (a web address, an anchor).
    /// </summary>
    public static string Resolve(string notePath, string href)
    {
        if (href.Contains(':') || href.StartsWith('#') || href.StartsWith('/')) return "";
        var hash = href.IndexOf('#');
        if (hash >= 0) href = href[..hash];

        var parts = notePath.Split('/').SkipLast(1).ToList();
        foreach (var part in Uri.UnescapeDataString(href).Split('/'))
        {
            if (part == "..") { if (parts.Count > 0) parts.RemoveAt(parts.Count - 1); }
            else if (part is not ("." or "")) parts.Add(part);
        }
        return string.Join('/', parts);
    }
}
