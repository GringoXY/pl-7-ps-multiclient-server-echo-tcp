using Shared;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace IterativeMultiClientTcpServerProgram;

// https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.listen?view=net-9.0&redirectedfrom=MSDN#System_Net_Sockets_Socket_Listen_System_Int32_
// https://stackoverflow.com/questions/19218589/tcp-server-with-multiple-clients
// https://stackoverflow.com/questions/49720212/multiple-clients-on-tcplistener-c-sharp-server-sending-data
internal sealed class IterativeMultiClientTcpServer(int port)
{
    private static readonly byte[] _serverBusyEncodedMessage = Encoding.UTF8.GetBytes(Configs.ServerBusyErrorMessage);
    private static readonly object _lockObject = new();
    private static readonly List<(Thread, Socket)> _connectedClients = [];
    private static int _clientId = 1;

    public int Port => port;

    public async Task Start()
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("Uruchamiam serwer...");

        try
        {
            IPEndPoint ipEndPoint = new(IPAddress.Any, Port);

            // Connecting to the server.
            // "using" automatically disposes the client.
            // We don't have to worry about properly closing the client.
            // Underneath `Close()` method uses `Dispose()`.
            using Socket socket = new(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            socket.Bind(ipEndPoint);
            // Must be called after `.Bind()` call.
            // Called before `.Bind()` throws `SocketException`!
            socket.Listen();
            Console.WriteLine("Uruchomiono serwer");
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine("Oczekiwanie na połączenie z klientami...");
            while (true)
            {
                Socket clientSocket = await socket.AcceptAsync();
                lock (_lockObject)
                {
                    if (_connectedClients.Count >= Configs.MaxConnections)
                    {
                        RejectClient(clientSocket);
                    }
                    else
                    {
                        Thread clientThread = new(() => HandleClient(clientSocket))
                        {
                            Name = $"#{_clientId}"
                        };
                        clientThread.Start();
                        _connectedClients.Add((clientThread, clientSocket));
                        _clientId += 1;
                        UpdateConnectionsDisplay();
                    }
                }
            }
        }
        catch (SocketException se)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Błąd gniazda serwera: {se.Message}");
            if (se.InnerException is not null)
            {
                Console.WriteLine($"Wyjątek wewnętrzny: {se.InnerException?.Message}");
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Błąd: {e.Message}");
            if (e.InnerException is not null)
            {
                Console.WriteLine($"Wyjątek wewnętrzny: {e.InnerException?.Message}");
            }
        }
        finally
        {
            foreach ((Thread clientThread, _) in _connectedClients)
            {
                clientThread.Join();
            }
        }

        Console.WriteLine("Zamykanie serwera. Naciśnij enter, aby zakończyć");
        Console.ReadKey();
    }

    private void RejectClient(Socket clientSocket)
    {
        clientSocket.Send(_serverBusyEncodedMessage);
        clientSocket.Close();
    }

    private void HandleClient(Socket clientSocket)
    {
        string clientId = Thread.CurrentThread.Name ?? string.Empty;

        try
        {
            // Console.ForegroundColor = ConsoleColor.Green;
            // Console.WriteLine($"Klient z IP: {clientSocket.RemoteEndPoint} o ID: {clientId} połączył się z serwerem!");
            Console.ForegroundColor = ConsoleColor.Gray;

            byte[] buffer = Encoding.ASCII.GetBytes("Witaj kliencie!");
            clientSocket.Send(buffer);
            int bytes;

            while ((bytes = clientSocket.Receive(buffer)) > 0)
            {
                string receivedClientMessage = Encoding.ASCII.GetString(buffer, 0, bytes);
                Console.WriteLine($"Wiadomość od klienta {Thread.CurrentThread.Name} o długości {bytes} bajtów: {receivedClientMessage}");

                Console.WriteLine($"Odsyłanie wiadomości do klienta {clientId}...");

                clientSocket.Send(buffer);
            }
        }
        catch (SocketException se)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Błąd gniazda klienta: {se.Message}");
            if (se.InnerException is not null)
            {
                Console.WriteLine($"Wyjątek wewnętrzny: {se.InnerException?.Message}");
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Błąd: {e.Message}");
            if (e.InnerException is not null)
            {
                Console.WriteLine($"Wyjątek wewnętrzny: {e.InnerException?.Message}");
            }
        }
        finally
        {
            lock (_lockObject)
            {
                _connectedClients.Remove((Thread.CurrentThread, clientSocket));
                UpdateConnectionsDisplay();
            }

            Console.WriteLine($"Połączenie z klientem {clientId} zamknięte");

            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }

    private void UpdateConnectionsDisplay()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;

        Console.SetCursorPosition(80, 10);
        Console.WriteLine("Połączeni klienci:");
        foreach ((Thread clientThread, Socket clientSocket) in _connectedClients)
        {
            Console.SetCursorPosition(80, 10 + _connectedClients.IndexOf((clientThread, clientSocket)) + 1);
            Console.WriteLine($"{clientThread.Name} {clientSocket.RemoteEndPoint}");
        }

        Console.ForegroundColor = ConsoleColor.Gray;
    }
}
