using Microsoft.JSInterop;

namespace Motif.Memory.Adapters;

/// <summary>
/// The real Folder Fitting: the File System Access API, through
/// wwwroot/files.js.
/// </summary>
public sealed class JsInteropFolder(IJSRuntime js)
    : Folder, IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> module = new(() => js
        .InvokeAsync<IJSObjectReference>(
            "import", "./_content/Motif.Memory.ForSavingTheMemory/files.js")
        .AsTask());

    public async Task<bool> CanChoose() =>
        await (await module.Value).InvokeAsync<bool>("canPick");

    public async Task<ChosenFolder> Choose()
    {
        var picked = Thrown(
            await (await module.Value).InvokeAsync<Picked>("pick"));
        return new JsInteropChosenFolder(module, picked.Folder, picked.Name);
    }

    /// <summary>
    /// Throws the error files.js handed back, if there is one.
    /// </summary>
    internal static T Thrown<T>(T result) where T : Attempt =>
        result.Error == ""
            ? result
            : throw new FolderException(Failure(result.Error), result.Message);

    /// <summary>
    /// The Seam's kind for the browser's name for an error.
    /// </summary>
    private static FolderFailure Failure(string error) => error switch
    {
        "AbortError" => FolderFailure.Declined,
        "NotAllowedError" => FolderFailure.Refused,
        _ => FolderFailure.Failed,
    };

    internal abstract record Attempt(string Error, string Message);

    private sealed record Picked(
        string Error, string Message, IJSObjectReference Folder, string Name)
        : Attempt(Error, Message);

    public async ValueTask DisposeAsync()
    {
        if (module.IsValueCreated) await (await module.Value).DisposeAsync();
    }
}
