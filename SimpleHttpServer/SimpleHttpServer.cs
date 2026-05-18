using System.Net;

namespace SimpleHttpServer;

public class SimpleHttpServer
{
    public static async Task Main()
    {
        const string url = "http://*:8889/";

        using var listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Start();
        Console.WriteLine($"Http server started and listening on port {url} - Wait for connection...");

        while (true)
        {
            var context = await listener.GetContextAsync();
            Console.WriteLine("client connected!");
            var httpHandler = new HttpHandler(context);
            ThreadPool.QueueUserWorkItem(httpHandler.Do);
        }
    }
}