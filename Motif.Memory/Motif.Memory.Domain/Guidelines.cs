namespace Motif.Memory;

/// <summary>
/// The guidelines in a pack's note: each top-level bullet under a <c>###</c> section is one,
/// with any indented lines after it.
/// </summary>
public static class Guidelines
{
    /// <param name="Index">Its place among the note's guidelines, from 1.</param>
    public sealed record Found(int Index, string Section, string Text);

    public static IEnumerable<Found> Read(string body)
    {
        var section = "";
        var index = 0;
        foreach (var line in body.Split('\n'))
        {
            if (line.StartsWith("### ")) section = line[4..].Trim();
            else if (section != "" && line.StartsWith("- ")) yield return new Found(++index, section, line[2..]);
        }
    }

    /// <summary>
    /// The note without the guidelines whose indexes aren't kept.
    /// </summary>
    public static string Keep(string body, Func<int, bool> kept)
    {
        var lines = new List<string>();
        var section = "";
        var index = 0;
        var dropping = false;
        foreach (var line in body.Split('\n'))
        {
            if (dropping && line.Length > 0 && char.IsWhiteSpace(line[0])) continue;
            dropping = false;
            if (line.StartsWith("### ")) section = line;
            else if (section != "" && line.StartsWith("- ") && !kept(++index))
            {
                dropping = true;
                continue;
            }
            lines.Add(line);
        }
        return string.Join('\n', lines);
    }

    /// <summary>
    /// Drops each <c>###</c> section left with no guidelines, heading and all.
    /// </summary>
    public static string DropEmptySections(string body)
    {
        var lines = body.Split('\n');
        var result = new List<string>();
        for (var i = 0; i < lines.Length; i++)
        {
            if (!lines[i].StartsWith("### "))
            {
                result.Add(lines[i]);
                continue;
            }
            var end = i + 1;
            while (end < lines.Length && !lines[end].StartsWith('#')) end++;
            if (lines[(i + 1)..end].Any(l => l.StartsWith("- "))) result.AddRange(lines[i..end]);
            i = end - 1;
        }
        return string.Join('\n', result);
    }
}
