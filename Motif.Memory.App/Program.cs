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

builder.Services.AddSingleton<BrowserFilesMemoryFiles>();
builder.Services.AddSingleton(sp => new MotifMemoryApplication(
    new BundledContentMemoryContent(),
    sp.GetRequiredService<BrowserFilesMemoryFiles>()));
builder.Services.AddSingleton<Browser>();
builder.Services.AddSingleton<Toaster>();

var host = builder.Build();
await host.Services.GetRequiredService<BrowserFilesMemoryFiles>().Start();
await host.RunAsync();
