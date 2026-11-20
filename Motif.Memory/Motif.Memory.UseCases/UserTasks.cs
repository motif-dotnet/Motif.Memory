namespace Motif.Memory;

/// <summary>
/// The User port's commands: the setup choices, and writing the memory out.
/// </summary>
public interface UserTasks
{
    /// <summary>
    /// Core roles stay ticked.
    /// </summary>
    void TickRole(string name, bool ticked);

    void MoveStageUp(int id);

    void MoveStageDown(int id);

    /// <summary>
    /// The stage takes the other's place, and the stages between shift by one to make room.
    /// </summary>
    void MoveStageTo(int id, int onto);

    /// <summary>
    /// Only optional packs can be left out.
    /// </summary>
    void TickPack(string name, bool ticked);

    void TickGuideline(string id, bool ticked);

    void ChooseCommitting(Committing committing);

    /// <summary>
    /// A folder that isn't empty is refused straight away, before the memory is written.
    /// </summary>
    Task PickFolder();

    /// <summary>
    /// Writes to the folder picked, which must then be picked again to write again.
    /// </summary>
    Task WriteToFolder();

    Task DownloadZip();

    void DismissProblem();
}
