using Microsoft.JSInterop;

namespace Motif.Memory.Adapters;

/// <summary>
/// A folder picked through <see cref="JsInteropFolder"/>: a reference
/// to the browser's handle for it.
/// </summary>
internal sealed class JsInteropChosenFolder(
    Lazy<Task<IJSObjectReference>> module,
    IJSObjectReference folder,
    string name) : ChosenFolder
{
    public string Name { get; } = name;

    public async Task<IReadOnlyList<string>> Names() =>
        JsInteropFolder.Thrown(
            await (await module.Value).InvokeAsync<Listed>("names", folder))
        .Names;

    public async Task Write(string path, string text) =>
        JsInteropFolder.Thrown(
            await (await module.Value).InvokeAsync<Done>(
                "write", folder, path, text));

    private sealed record Listed(string Error, string Message, string[] Names)
        : JsInteropFolder.Attempt(Error, Message);

    private sealed record Done(string Error, string Message)
        : JsInteropFolder.Attempt(Error, Message);
}
