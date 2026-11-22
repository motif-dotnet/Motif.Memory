using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Motif.Memory.BrowserSpecs;

[CollectionDefinition(Name)]
public sealed class SiteCollection : ICollectionFixture<Site>
{
    public const string Name = "Site";
}

/// <summary>
/// The app as it's deployed: published in Release (trimmed), served as
/// static files, and opened in a real browser, with the real composition
/// root and the real Fittings. Each page gets a fresh browser profile, so
/// preferences and folders start empty. Nothing leaves the machine: every
/// request beyond the site (such as the visit counter) is blocked.
/// </summary>
public sealed class Site : IAsyncLifetime
{
    private readonly string repo = RepoRoot();
    private WebApplication? server;
    private IPlaywright? playwright;
    private IBrowser? browser;

    public string Url { get; private set; } = "";

    public async Task InitializeAsync()
    {
        // MOTIF_SITE is a folder the app is already published to, as CI
        // publishes it once for the specs and the deploy. Otherwise the
        // specs publish it themselves.
        var site = Environment.GetEnvironmentVariable("MOTIF_SITE");
        if (string.IsNullOrEmpty(site))
        {
            site = Path.Combine(AppContext.BaseDirectory, "site");
            await Publish(site);
        }
        server = Serve(Path.Combine(site, "wwwroot"));
        await server.StartAsync();
        Url = server.Services.GetRequiredService<IServer>()
            .Features.GetRequiredFeature<IServerAddressesFeature>()
            .Addresses.Single() + "/";

        playwright = await Playwright.CreateAsync();
        // The Edge every Windows machine has, so there's nothing to download.
        // MOTIF_BROWSER picks another channel, such as "chrome".
        browser = await playwright.Chromium.LaunchAsync(new()
        {
            Channel = Environment.GetEnvironmentVariable("MOTIF_BROWSER")
                ?? "msedge",
        });
    }

    /// <summary>
    /// A browser profile of its own, at this window width. Its folder picker
    /// hands out folders in the browser's private storage (see
    /// <see cref="Folders"/>), or, with no picker, it's a browser that has
    /// none, as Firefox and Safari are.
    /// </summary>
    public async Task<IBrowserContext> Profile(
        int width = 1280, bool picker = true)
    {
        var profile = await browser!.NewContextAsync(new()
        {
            ViewportSize = new() { Width = width, Height = 812 },
        });
        await profile.AddInitScriptAsync(
            picker ? Folders.Picker : Folders.NoPicker);
        await profile.RouteAsync("**/*", Route);
        return profile;
    }

    /// <summary>Opens the app and waits for it to start.</summary>
    public async Task<IPage> Open(IBrowserContext profile)
    {
        var page = await profile.NewPageAsync();
        await Go(page);
        return page;
    }

    public async Task Go(IPage page)
    {
        await page.GotoAsync(Url);
        await Started(page);
    }

    public static Task Started(IPage page) =>
        page.Locator("header.appbar").WaitForAsync();

    private async Task Route(IRoute route)
    {
        if (route.Request.Url.StartsWith(Url, StringComparison.Ordinal))
            await route.ContinueAsync();
        else
            await route.AbortAsync();
    }

    private async Task Publish(string output)
    {
        var app = Path.Combine(repo, "Motif.Memory.App");
        using var publish = Process.Start(new ProcessStartInfo(
            "dotnet", ["publish", app, "-c", "Release", "-o", output])
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        })!;
        var log = publish.StandardOutput.ReadToEndAsync();
        var errors = publish.StandardError.ReadToEndAsync();
        await publish.WaitForExitAsync();
        if (publish.ExitCode != 0)
            throw new InvalidOperationException(
                "Publishing the app failed:\n" + await log + await errors);
    }

    private static WebApplication Serve(string root)
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        var app = builder.Build();
        var files = new PhysicalFileProvider(root);
        app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = files });
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = files,
            ContentTypeProvider = new FileExtensionContentTypeProvider(),
            ServeUnknownFileTypes = true,
        });
        return app;
    }

    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(dir.FullName, "Motif.Memory.sln")))
            dir = dir.Parent ?? throw new InvalidOperationException(
                "The specs must run inside the Motif.Memory repo.");
        return dir.FullName;
    }

    public async Task DisposeAsync()
    {
        if (browser is not null) await browser.DisposeAsync();
        playwright?.Dispose();
        if (server is not null) await server.DisposeAsync();
    }
}
