namespace Motif.Memory.BrowserSpecs;

/// <summary>
/// The browser's folder picker, the one part of the Folder Fitting a spec
/// can't drive, stood in for with folders in the browser's private storage
/// (the origin private file system). They're real folder handles, so the
/// real Fitting, files.js and the calls between them all run as they do in
/// a folder on disk. A spec says what the next pick does: hand back a
/// folder, or fail as the browser does when the picker is closed
/// (AbortError) or editing is refused (NotAllowedError).
/// </summary>
public static class Folders
{
    /// <summary>
    /// Run before the app starts. With nothing said, a pick hands back an
    /// empty folder named "memory".
    /// </summary>
    public const string Picker = """
        (() => {
          const picks = [];
          window.motifPicks = picks;
          Object.defineProperty(window, "showDirectoryPicker", {
            configurable: true,
            value: async () => {
              const next = picks.shift() ?? { folder: "memory" };
              if (next.error) throw new DOMException(next.error, next.error);
              const root = await navigator.storage.getDirectory();
              return root.getDirectoryHandle(next.folder, { create: true });
            },
          });
        })();
        """;

    /// <summary>
    /// A browser with no folder picker, as Firefox and Safari are.
    /// </summary>
    public const string NoPicker = """
        Object.defineProperty(window, "showDirectoryPicker", {
          configurable: true, value: undefined,
        });
        """;

    /// <summary>The next pick hands back this folder.</summary>
    public static Task WillPick(IPage page, string folder) =>
        page.EvaluateAsync("f => window.motifPicks.push({ folder: f })",
            folder);

    /// <summary>The next pick is the picker closed.</summary>
    public static Task WillDecline(IPage page) => WillFail(page, "AbortError");

    /// <summary>The next pick is a folder the browser may not edit.</summary>
    public static Task WillRefuse(IPage page) =>
        WillFail(page, "NotAllowedError");

    private static Task WillFail(IPage page, string error) =>
        page.EvaluateAsync("e => window.motifPicks.push({ error: e })", error);

    /// <summary>Puts a file at this path in the folder.</summary>
    public static Task Put(IPage page, string folder, string path) =>
        page.EvaluateAsync("""
            async ([folder, path]) => {
              const root = await navigator.storage.getDirectory();
              let dir = await root.getDirectoryHandle(folder, { create: true });
              const parts = path.split("/");
              for (const part of parts.slice(0, -1))
                dir = await dir.getDirectoryHandle(part, { create: true });
              await dir.getFileHandle(parts.at(-1), { create: true });
            }
            """, new[] { folder, path });

    /// <summary>
    /// Every file in the folder, nested ones too, by its path, such as
    /// "Roles/Tester.md", with its text.
    /// </summary>
    public static async Task<Dictionary<string, string>> Files(
        IPage page, string folder) =>
        await page.EvaluateAsync<Dictionary<string, string>>("""
            async folder => {
              const files = {};
              const walk = async (dir, prefix) => {
                for await (const [name, entry] of dir.entries()) {
                  if (entry.kind === "directory")
                    await walk(entry, prefix + name + "/");
                  else
                    files[prefix + name] = await (await entry.getFile()).text();
                }
              };
              const root = await navigator.storage.getDirectory();
              const top = await root.getDirectoryHandle(
                folder, { create: true });
              await walk(top, "");
              return files;
            }
            """, folder);
}
