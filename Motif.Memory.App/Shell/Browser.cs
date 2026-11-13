using Microsoft.JSInterop;

namespace Motif.Memory.App;

/// <summary>
/// The Web App's own browser plumbing (wwwroot/js/app.js). Synchronous where WebAssembly allows it.
/// </summary>
public sealed class Browser(IJSRuntime js)
{
    private IJSInProcessObjectReference? module;

    public async Task Load() =>
        module ??= await js.InvokeAsync<IJSInProcessObjectReference>("import", "./js/app.js");

    private IJSInProcessObjectReference M => module ?? throw new InvalidOperationException("app.js isn't loaded yet.");

    public string Theme() => M.Invoke<string>("theme");
    public void SetTheme(string theme) => M.InvokeVoid("setTheme", theme);
}
