using Shared;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace TcpClientProgram;

internal sealed class TcpClient(string hostname, int port)
{
    public int Port { get; private set; } = port;
    public string Hostname { get; private set; } = hostname;

    public void Start()
    {
        try
        {
            // Throws "ArgumentOutOfRangeException" when port exceeds range.
            IPAddress[] ipAddresses = Dns.GetHostAddresses(Hostname);
            IPEndPoint ipEndPoint = new(ipAddresses[0].MapToIPv4(), Port);

            // Connecting to the server.
            // "using" automatically disposes the client.
            // We don't have to worry about properly closing the client.
            // Underneath `Close()` method uses `Dispose()`.
            using Socket socket = new(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.IP);

            socket.Connect(ipEndPoint);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Połączono z serwerem!");
            Console.ForegroundColor = ConsoleColor.Gray;

            byte[] data = new byte[Configs.DefaultMessageBytesLength];
            int bytes;

            bytes = socket.Receive(data);
            string receivedServerMessage = Encoding.ASCII.GetString(data, 0, bytes);
            Console.WriteLine($"Wiadomość z serwera: {receivedServerMessage}");

            if (string.Equals(receivedServerMessage, Configs.ServerBusyErrorMessage, StringComparison.InvariantCultureIgnoreCase) == false)
            {
                string messageToSend;

                Console.Write("Wyślij wiadomość do serwera: ");
                while ((messageToSend = Console.ReadLine()).Equals("exit", StringComparison.OrdinalIgnoreCase) == false)
                {
                    byte[] encodedMessage = Encoding.ASCII.GetBytes(messageToSend);

                    socket.Send(encodedMessage);

                    Console.WriteLine($"Wysłano wiadomość o długości {encodedMessage.Length} bajtów: {messageToSend}");

                    Console.Write("Wyślij wiadomość do serwera: ");
                }
            }
        }
        catch (SocketException se)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"Błąd gniazda: {se.Message}");
            if (se.InnerException is not null)
            {
                Console.Error.WriteLine($"Wyjątek wewnętrzny: {se.InnerException?.Message}");
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"Nieznany błąd: {e.Message}");
            if (e.InnerException is not null)
            {
                Console.Error.WriteLine($"Wyjątek wewnętrzny: {e.InnerException?.Message}");
            }
        }

        Console.WriteLine("Zamykanie klienta. Naciśnij enter, aby zakończyć");
        Console.ReadKey();
    }
}
