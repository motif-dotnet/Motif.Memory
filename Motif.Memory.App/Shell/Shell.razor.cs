using Microsoft.AspNetCore.Components;
using Motif.Suite;

namespace Motif.Memory.App;

/// <summary>
/// The Web App's one page: the setup choices, then the next steps. All the logic is behind the User port.
/// </summary>
public partial class Shell : IDisposable
{
    private static readonly (Committing Value, string Label, string Line)[] Committings =
    [
        (Committing.Agreed, "Once agreed", "One commit for each set of changes you agree, pushed if the memory has a remote."),
        (Committing.Asked, "Only when asked", "The agent commits when you tell it to."),
        (Committing.Never, "Never", "You commit the memory yourself."),
    ];

    private bool started;
    private bool busy;
    private bool helpOpen;
    private string menu = "";
    private string theme = "system";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await Browser.Load();
        theme = Browser.Theme();
        Toaster.Changed += Refresh;
        started = true;
        StateHasChanged();
    }

    private static bool Checked(ChangeEventArgs e) => e.Value is true;

    private static string Plural(int count, string one, string many) => $"{count} {(count == 1 ? one : many)}";

    private async Task WriteToFolder() => await Writing(App.WriteToFolder);

    private async Task DownloadZip() => await Writing(App.DownloadZip);

    private async Task Writing(Func<Task> write)
    {
        busy = true;
        try
        {
            await write();
        }
        finally
        {
            busy = false;
        }
        // What went wrong is the outcome of a command, so it's a toast, as in every Motif tool.
        if (App.Problem != "")
        {
            Toaster.Problem("The memory wasn't written", [App.Problem]);
            App.DismissProblem();
        }
        // The last steps are the agent's, through the setup note, so the outcome says how to start them.
        // That's a sentence to pass on, so the toast offers it to copy and stays until dismissed.
        var tell = $"Read {App.SetupNote} in this folder and follow it.";
        if (App.Written == Written.ToFolder)
            Toaster.Offer($"Memory written to “{App.FolderName}”", "To finish, tell your agent:", tell);
        else if (App.Written == Written.AsZip)
            Toaster.Offer("Memory downloaded", $"Unzip {MotifMemoryApplication.ZipName} into a folder that stays put. To finish, tell your agent:", tell);
    }

    // Dragging a stage: the one picked up, and the one it's over. The up and down buttons do the same without a mouse.
    private int? dragged;
    private int? over;

    private void Drag(int id) => dragged = id;

    private void DragOver(int id) => over = id;

    private void Drop(int id)
    {
        if (dragged is { } stage) App.MoveStageTo(stage, id);
        EndDrag();
    }

    private void EndDrag()
    {
        dragged = null;
        over = null;
    }

    /// <summary>
    /// Marks the stage picked up, and the edge of the stage it's over that it would land beyond. Nothing lands on the pinned stage.
    /// </summary>
    private string DragMark(StageChoice stage)
    {
        if (stage.Pinned) return "pinned";
        if (dragged is not { } picked) return "";
        if (stage.Id == picked) return "dragging";
        if (stage.Id != over) return "";
        var ids = App.Stages.Select(s => s.Id).ToList();
        return ids.IndexOf(picked) < ids.IndexOf(stage.Id) ? "drop-below" : "drop-above";
    }

    private void Toggle(string name)
    {
        menu = menu == name ? "" : name;
        helpOpen = false;
    }

    private void ToggleHelp()
    {
        helpOpen = !helpOpen;
        menu = "";
    }

    private void CloseMenus() => menu = "";

    private void SetTheme(string value)
    {
        theme = value;
        Browser.SetTheme(value);
        menu = "";
    }

    private void Refresh() => InvokeAsync(StateHasChanged);

    public void Dispose() => Toaster.Changed -= Refresh;
}
