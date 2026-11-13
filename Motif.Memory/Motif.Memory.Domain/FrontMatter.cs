namespace Motif.Memory;

/// <summary>
/// The YAML between <c>---</c> lines at the very top of a note in Content/: only <c>key: value</c>
/// and <c>key: [a, b]</c>, which is all the content uses.
/// </summary>
public sealed class FrontMatter
{
    private readonly Dictionary<string, string> values;

    private FrontMatter(Dictionary<string, string> values) => this.values = values;

    public static readonly FrontMatter None = new([]);

    public bool IsEmpty => values.Count == 0;

    public bool Has(string key) => values.ContainsKey(key);

    public string Get(string key) => values.GetValueOrDefault(key, "");

    public bool Flag(string key) => Get(key) == "true";

    public IReadOnlyList<string> List(string key)
    {
        var value = Get(key).Trim();
        if (value.StartsWith('[') && value.EndsWith(']')) value = value[1..^1];
        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    /// <summary>
    /// Splits a note's text into its front matter and the rest, with line endings made <c>\n</c>.
    /// </summary>
    public static (FrontMatter Facts, string Body) Split(string text)
    {
        text = text.Replace("\r\n", "\n");
        if (!text.StartsWith("---\n")) return (None, text);
        var end = text.IndexOf("\n---\n", 3, StringComparison.Ordinal);
        if (end < 0) return (None, text);

        var values = new Dictionary<string, string>();
        foreach (var line in text[4..end].Split('\n'))
        {
            var colon = line.IndexOf(':');
            if (colon > 0) values[line[..colon].Trim()] = line[(colon + 1)..].Trim();
        }
        return (new FrontMatter(values), text[(end + 5)..].TrimStart('\n'));
    }
}
