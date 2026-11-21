namespace Motif.Memory;

public sealed record CommittingChoice(
    Committing Value, string Label, string Line, bool Chosen);
