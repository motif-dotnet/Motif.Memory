using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Motif.Memory;
using Motif.Memory.Adapters;
using Motif.Memory.App;
using Motif.Suite;

// The composition root: the only place that knows which adapters are used.
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<Folder, JsInteropFolder>();
builder.Services.AddSingleton<Download, JsInteropDownload>();
// The ways of saving, offered in the order registered, where the browser can:
// the first is the default. A folder comes first, since it's where the memory
// will live; the zip is there for every browser.
builder.Services.AddSingleton<ForSavingTheMemory, ForSavingTheMemoryViaFolder>();
builder.Services.AddSingleton<ForSavingTheMemory, ForSavingTheMemoryViaZipDownload>();
builder.Services.AddSingleton(sp => new MotifMemoryApplication(
    new BundledContentMemoryContent(),
    sp.GetServices<ForSavingTheMemory>()));
builder.Services.AddSingleton<Browser>();
builder.Services.AddSingleton<Toaster>();

var host = builder.Build();
await host.Services.GetRequiredService<MotifMemoryApplication>().Start();
await host.RunAsync();
