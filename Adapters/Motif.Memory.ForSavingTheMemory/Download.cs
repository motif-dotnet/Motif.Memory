namespace Motif.Memory.Adapters;

/// <summary>
/// The Download Seam: what the zip adapter needs from the browser,
/// one call. Every Fitting must behave as these comments say.
/// </summary>
public interface Download
{
    /// <summary>
    /// Has the browser save the bytes as a download with this name.
    /// </summary>
    Task Save(string name, byte[] bytes);
}
