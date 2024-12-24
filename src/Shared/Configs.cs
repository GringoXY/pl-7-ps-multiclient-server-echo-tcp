namespace Shared;

/// <summary>
/// Shared configuration for server and the client
/// </summary>
public static class Configs
{
    public const int DefaultPort = 7;
    public const int DefaultMessageBytesLength = 1_024;
    public const string ServerBusyErrorMessage = "SERVER BUSY";
    public const int MaxConnections = 3;
    public const string ExitClientCommand = "exit";
}
