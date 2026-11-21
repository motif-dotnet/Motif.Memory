namespace Motif.Memory;

/// <summary>
/// The User port's commands: the setup choices, and writing the memory out.
/// </summary>
public interface ForDoing
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
    /// Chooses where to save the named way, such as by picking a folder.
    /// A folder that isn't empty is refused straight away.
    /// </summary>
    Task ChooseWhere(string way);

    /// <summary>
    /// Saves the memory the named way, where was chosen, which must then be
    /// chosen again to save again.
    /// </summary>
    Task Save(string way);

    void DismissProblem();
}
