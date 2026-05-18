using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ZumoApp;

public class ZumoServer
{
    private static bool finished;

    private static async Task Main()
    {
        var listener = new TcpListener(IPAddress.Any, 8888);
        listener.Start();
        Console.WriteLine($"Telnet-Server is listening on port {8888}...");

        try
        {
            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                Console.WriteLine($"[{client.Client.RemoteEndPoint}] client connected!");

                HandleTelnetCommand(client);
            }
        }
        finally
        {
            listener.Stop();
        }
    }

    private static void HandleTelnetCommand(TcpClient client)
    {
        using (client)
        using (var stream = client.GetStream())
        using (var reader = new StreamReader(stream, Encoding.ASCII))
        using (var writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true })
        {
            try
            {
                writer.NewLine = "\r\n";
                writer.WriteLine("Welcome to Zumo, select your drive:");
                while (!finished)
                {
                    writer.WriteLine("Press A or B or C and <Enter> to start or \"Exit\" to leave...");
                    var choise = reader.ReadLine();
                    var response = "wrong choise";
                    switch (choise?.ToUpper())
                    {
                        case "A":
                            response = ZumoDrives.ZumoDriveA();
                            break;
                        case "B":
                            response = ZumoDrives.ZumoDriveB();
                            break;
                        case "C":
                            response = ZumoDrives.ZumoDriveC();
                            break;
                        default:
                            finished = true;
                            response = "Exit";
                            break;
                    }

                    writer.WriteLine(response.ReplaceLineEndings("\r\n"));
                    if (response != "Exit") ZumoProtocol.Write(response);
                }
            }
            finally
            {
                ZumoLidar.Off();
            }
        }
    }
}