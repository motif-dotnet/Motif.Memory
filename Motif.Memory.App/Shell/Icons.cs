using Microsoft.AspNetCore.Components;
using Motif.Suite;

namespace Motif.Memory.App;

/// <summary>
/// The app's few icons, drawn with the suite's <see cref="SuiteIcons.Icon"/> so they match its own.
/// </summary>
public static class Icons
{
    private static MarkupString I(string paths, string extra = "") => SuiteIcons.Icon(paths, extra);

    public static readonly MarkupString Folder = I("<path d=\"M3 7a2 2 0 012-2h4l2 2h8a2 2 0 012 2v8a2 2 0 01-2 2H5a2 2 0 01-2-2z\"/>");
    public static readonly MarkupString Download = I("<path d=\"M12 4v11M7 10l5 5 5-5M5 20h14\"/>");
    public static readonly MarkupString Up = I("<path d=\"M6 15l6-6 6 6\"/>");
    public static readonly MarkupString Down = I("<path d=\"M6 9l6 6 6-6\"/>");
    public static readonly MarkupString Warning = I("<path d=\"M12 3l10 18H2z\"/><path d=\"M12 10v5M12 18h.01\"/>");
}
