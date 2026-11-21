namespace Motif.Memory.Adapters;

/// <summary>
/// A failure in the Folder Seam, of one of its own kinds. The message is the
/// platform's own, for saying why.
/// </summary>
public sealed class FolderException(FolderFailure failure, string message)
    : Exception(message)
{
    public FolderFailure Failure { get; } = failure;
}
