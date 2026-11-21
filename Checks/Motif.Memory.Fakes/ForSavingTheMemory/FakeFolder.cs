using Motif.Memory.Adapters;

namespace Motif.Memory.Fakes;

/// <summary>
/// A Folder Fitting with no browser. Choosing hands back <see cref="Chosen"/>,
/// or fails as set up with <see cref="ChoosingFails"/>.
/// </summary>
public sealed class FakeFolder : Folder
{
    private FolderException? failure;

    /// <summary>
    /// Whether a folder can be chosen here: false for a browser with no
    /// folder picker.
    /// </summary>
    public bool CanChooseFolder { get; set; } = true;

    /// <summary>
    /// The folder choosing hands back.
    /// </summary>
    public FakeChosenFolder Chosen { get; set; } = new("memory");

    /// <summary>
    /// Makes every choice from now on fail, as this kind of failure, with the
    /// platform's message for it.
    /// </summary>
    public void ChoosingFails(
        FolderFailure kind, string message = "The picker failed") =>
        failure = new FolderException(kind, message);

    public Task<bool> CanChoose() => Task.FromResult(CanChooseFolder);

    public Task<ChosenFolder> Choose() =>
        failure is null
            ? Task.FromResult<ChosenFolder>(Chosen)
            : throw failure;
}
