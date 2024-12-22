using Shared;
using TcpClientProgram;

// Used articles:
// https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.connect?view=net-9.0&redirectedfrom=MSDN#system-net-sockets-socket-connect(system-net-ipaddress-system-int32)
// https://learn.microsoft.com/en-us/dotnet/api/system.net.dns.gethostbyname?view=net-9.0&redirectedfrom=MSDN#System_Net_Dns_GetHostByName_System_String_
// https://learn.microsoft.com/en-us/dotnet/api/system.uri.checkhostname?view=net-9.0#system-uri-checkhostname(system-string)
// https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.socket?view=net-9.0
// Based on task "ECHO TCP SERVER-CLIENT"
int port = Configs.DefaultPort;

Console.ForegroundColor = ConsoleColor.Gray;
Console.Write("Podaj adres IP lub nazwę domenową serwera: ");
string hostname = Console.ReadLine() ?? string.Empty;

if (string.IsNullOrWhiteSpace(hostname))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine("Nie podano adresu serwera");
    return;
}

if (hostname.IsHostnameValid())
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine("Podany adres serwera jest niepoprawny");
    return;
}

Console.Write($"Podaj port (domyślnie {port}): ");
if (int.TryParse(Console.ReadLine(), out int parsedPort))
{
    port = parsedPort;
}

new TcpClient(hostname, port).Start();
