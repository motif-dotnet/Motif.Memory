using System.Text.RegularExpressions;

namespace Motif.Memory;

/// <summary>
/// Tags: HTML comment pairs in Content/ around text that's written only when a condition holds.
/// A tag on a line of its own wraps whole lines; a tag inside a line wraps part of it. No nesting.
/// </summary>
public static partial class Tags
{
    [GeneratedRegex(@"<!--\s*(?:if\s+(?<condition>.*?)|(?<end>end))\s*-->")]
    private static partial Regex Tag();

    /// <summary>
    /// Keeps the text whose conditions hold, drops the rest, and strips every tag.
    /// </summary>
    public static string Apply(string text, Func<string, bool> holds)
    {
        var kept = new List<string>();
        var dropping = false;
        foreach (var line in text.Split('\n'))
        {
            if (IsBlock(line, out var block))
            {
                dropping = block.IsIf && !Holds(block.Condition, holds);
                continue;
            }
            if (dropping) continue;
            kept.Add(Inline(line, holds));
        }
        return string.Join('\n', kept);
    }

    private static string Inline(string line, Func<string, bool> holds)
    {
        var result = new System.Text.StringBuilder();
        var keep = true;
        var at = 0;
        foreach (Match tag in Tag().Matches(line))
        {
            if (keep) result.Append(line, at, tag.Index - at);
            keep = !tag.Groups["condition"].Success || Holds(tag.Groups["condition"].Value, holds);
            at = tag.Index + tag.Length;
        }
        if (keep) result.Append(line, at, line.Length - at);
        return result.ToString();
    }

    /// <summary>
    /// Whether the line is only a tag, which then wraps whole lines.
    /// </summary>
    private static bool IsBlock(string line, out BlockTag block)
    {
        var match = Tag().Match(line);
        var isBlock = match.Success && match.Value == line.Trim();
        block = new BlockTag(isBlock && match.Groups["condition"].Success, isBlock ? match.Groups["condition"].Value : "");
        return isBlock;
    }

    private sealed record BlockTag(bool IsIf, string Condition);

    private static bool Holds(string condition, Func<string, bool> holds)
    {
        var (names, all) = Parse(condition);
        return all ? names.All(holds) : names.Any(holds);
    }

    private static (IReadOnlyList<string> Names, bool All) Parse(string condition) =>
        condition.Contains(" or ")
            ? (condition.Split(" or ", StringSplitOptions.TrimEntries), false)
            : (condition.Split(" and ", StringSplitOptions.TrimEntries), true);

    /// <summary>
    /// Content errors in one note: unknown conditions, tags left open, an end with no if, nesting.
    /// </summary>
    public static IEnumerable<string> Problems(string path, string text, Func<string, bool> known)
    {
        var lines = text.Split('\n');
        var open = 0; // the line of the block if that's open, or 0
        for (var i = 0; i < lines.Length; i++)
        {
            var n = i + 1;
            var tags = Tag().Matches(lines[i]);
            if (tags.Count == 0) continue;

            if (IsBlock(lines[i], out var block))
            {
                if (block.IsIf)
                {
                    if (open > 0) yield return $"{path}:{n}: an if inside the if on line {open}";
                    foreach (var problem in ConditionProblems(path, n, block.Condition, known)) yield return problem;
                    open = n;
                }
                else if (open == 0) yield return $"{path}:{n}: an end with no if";
                else open = 0;
                continue;
            }

            var inlineOpen = false;
            foreach (Match tag in tags)
            {
                if (tag.Groups["condition"].Success)
                {
                    if (inlineOpen) yield return $"{path}:{n}: an if inside another on the same line";
                    foreach (var problem in ConditionProblems(path, n, tag.Groups["condition"].Value, known)) yield return problem;
                    inlineOpen = true;
                }
                else if (!inlineOpen) yield return $"{path}:{n}: an end with no if";
                else inlineOpen = false;
            }
            if (inlineOpen) yield return $"{path}:{n}: an if left open at the end of the line";
        }
        if (open > 0) yield return $"{path}:{open}: an if left open";
    }

    private static IEnumerable<string> ConditionProblems(string path, int line, string condition, Func<string, bool> known)
    {
        if (condition.Contains(" or ") && condition.Contains(" and "))
        {
            yield return $"{path}:{line}: \"{condition}\" mixes and with or";
            yield break;
        }
        foreach (var name in Parse(condition).Names.Where(name => !known(name)))
            yield return $"{path}:{line}: unknown condition \"{name}\"";
    }

    /// <summary>
    /// Text with its tags taken out but everything kept, for showing a line on the page.
    /// </summary>
    public static string Strip(string text) => Tag().Replace(text, "");
}
