using Antelcat.AutoGen.ComponentModel.Diagnostic;
using Microsoft.Extensions.Logging;

namespace Warden.Core.Playit;

[AutoExtractInterface]
internal class PlayitManager : IPlayitManager
{
    public PlayitManager(ILogger<PlayitManager> logger) { }

    public void CreateTunnel() { }

    public void DeleteTunnel() { }

    public void ClearTunnels() { }

    public void GetTunnel() { }

    public void StartTunnel() { }

    public void StopTunnel() { }
}
