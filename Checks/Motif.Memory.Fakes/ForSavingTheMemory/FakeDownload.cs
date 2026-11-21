using Motif.Memory.Adapters;

namespace Motif.Memory.Fakes;

/// <summary>
/// A Download Fitting with no browser: it keeps the last download.
/// </summary>
public sealed class FakeDownload : Download
{
    public string SavedName { get; private set; } = "";

    public byte[] SavedBytes { get; private set; } = [];

    public Task Save(string name, byte[] bytes)
    {
        SavedName = name;
        SavedBytes = bytes;
        return Task.CompletedTask;
    }
}
