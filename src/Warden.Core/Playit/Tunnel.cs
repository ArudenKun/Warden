namespace Warden.Core.Playit;

public record Tunnel(
    string Id,
    string Status,
    bool InUse,
    string Region,
    string Type,
    string Protocol,
    int Port,
    string Host,
    string Domain,
    int RemotePort,
    string HostName,
    DateTime CreateAt
);
