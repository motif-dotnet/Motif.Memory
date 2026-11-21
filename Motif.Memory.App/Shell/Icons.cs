using Microsoft.AspNetCore.Components;
using static Motif.Suite.SuiteIcons;

namespace Motif.Memory.App;

/// <summary>
/// The app's few icons, drawn with the suite's
/// <see cref="Motif.Suite.SuiteIcons.Icon"/> so they match its own.
/// </summary>
public static class Icons
{
    public static readonly MarkupString Folder = Icon(
        "<path d=\"M3 7a2 2 0 012-2h4l2 2h8a2 2 0 012 2v8a2 2 0 01-2 2H5"
        + "a2 2 0 01-2-2z\"/>");

    public static readonly MarkupString Download =
        Icon("<path d=\"M12 4v11M7 10l5 5 5-5M5 20h14\"/>");

    public static readonly MarkupString Up =
        Icon("<path d=\"M6 15l6-6 6 6\"/>");

    public static readonly MarkupString Down =
        Icon("<path d=\"M6 9l6 6 6-6\"/>");

    public static readonly MarkupString Warning = Icon(
        "<path d=\"M12 3l10 18H2z\"/><path d=\"M12 10v5M12 18h.01\"/>");
}
