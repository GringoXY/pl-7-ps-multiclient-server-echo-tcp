using System.Net;

namespace TcpClientProgram;

internal static class Extensions
{
    public static bool IsHostnameValid(this string hostname)
        => IPAddress.TryParse(hostname, out IPAddress _) == false && Uri.CheckHostName(hostname) != UriHostNameType.Dns;
}
