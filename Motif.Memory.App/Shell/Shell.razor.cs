using Microsoft.AspNetCore.Components;
using Motif.Suite;

namespace Motif.Memory.App;

/// <summary>
/// The Web App's one page: the setup choices, and saving the memory. All the
/// logic is behind the User port.
/// </summary>
public partial class Shell : IDisposable
{
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

    private static string Plural(int count, string one, string many) =>
        $"{count} {(count == 1 ? one : many)}";

    private static MarkupString Icon(SavingWay way) =>
        way.Icon == "folder" ? Icons.Folder : Icons.Download;

    private async Task ChooseWhere(SavingWay way) =>
        await Run(() => App.ChooseWhere(way.Name));

    private async Task Save(SavingWay way)
    {
        await Run(() => App.Save(way.Name));
        // The outcome is a sentence to pass on, so the toast offers it to
        // copy and stays until dismissed.
        if (App.Saved != SavedMemory.None)
            Toaster.Offer(App.Saved.Title, App.Saved.Steps, App.Saved.Tell);
    }

    /// <summary>
    /// Runs a command that may go wrong.
    /// What went wrong is its outcome, so it's a toast, as in every Motif tool.
    /// </summary>
    private async Task Run(Func<Task> command)
    {
        busy = true;
        try
        {
            await command();
        }
        finally
        {
            busy = false;
        }
        if (App.Problem != Problem.None)
        {
            Toaster.Problem(App.Problem.Title, [App.Problem.Text]);
            App.DismissProblem();
        }
    }

    // Dragging a stage: the one picked up, and the one it's over. The up and
    // down buttons do the same without a mouse.
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
    /// Marks the stage picked up, and the edge of the stage it's over that it
    /// would land beyond. Nothing lands on the pinned stage.
    /// </summary>
    private string DragMark(StageChoice stage)
    {
        if (stage.Pinned) return "pinned";
        if (dragged is not { } picked) return "";
        if (stage.Id == picked) return "dragging";
        if (stage.Id != over) return "";
        var ids = App.Stages.Select(s => s.Id).ToList();
        return ids.IndexOf(picked) < ids.IndexOf(stage.Id)
            ? "drop-below"
            : "drop-above";
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
