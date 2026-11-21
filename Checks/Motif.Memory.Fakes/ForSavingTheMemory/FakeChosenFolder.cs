using Motif.Memory.Adapters;

namespace Motif.Memory.Fakes;

/// <summary>
/// A folder held in memory, as the fake folder picker hands it back. Writing a
/// file can be set to fail.
/// </summary>
public sealed class FakeChosenFolder(string name) : ChosenFolder
{
    private readonly Dictionary<string, FolderException> refused = [];

    public string Name { get; } = name;

    /// <summary>
    /// Every file in the folder, by path, such as "Roles/Tester.md" or
    /// ".git/HEAD".
    /// </summary>
    public Dictionary<string, string> Files { get; } = [];

    /// <summary>
    /// Makes writing the file at this path fail, as this kind of failure, with
    /// the platform's message for it.
    /// </summary>
    public void Refuse(string path, FolderFailure failure, string message) =>
        refused[path] = new FolderException(failure, message);

    public Task<IReadOnlyList<string>> Names() =>
        Task.FromResult<IReadOnlyList<string>>(
            Files.Keys.Select(path => path.Split('/')[0]).Distinct().ToList());

    public Task Write(string path, string text)
    {
        if (refused.TryGetValue(path, out var error)) throw error;
        Files[path] = text;
        return Task.CompletedTask;
    }
}
