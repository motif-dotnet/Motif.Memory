using Microsoft.JSInterop;

namespace Motif.Memory.Adapters;

/// <summary>
/// The real Download Fitting: a link to the bytes, clicked, through
/// wwwroot/download.js.
/// </summary>
public sealed class JsInteropDownload(IJSRuntime js)
    : Download, IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> module = new(() => js
        .InvokeAsync<IJSObjectReference>(
            "import", "./_content/Motif.Memory.ForSavingTheMemory/download.js")
        .AsTask());

    public async Task Save(string name, byte[] bytes) =>
        await (await module.Value).InvokeVoidAsync("save", name, bytes);

    public async ValueTask DisposeAsync()
    {
        if (module.IsValueCreated) await (await module.Value).DisposeAsync();
    }
}
