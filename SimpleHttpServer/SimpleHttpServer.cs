using System.Net.Sockets;
using System.IO;
using System.Net;

namespace SimpleHttpServer {

    public class SimpleHttpServer {

        public static async Task Main() {
            const string url = "http://*:8889/";
            
            using var listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine($"Server gestartet auf {url} - Warte auf Verbindung...");

            while (true)
            {
                var context = await listener.GetContextAsync();
                Console.WriteLine("Client verbunden!");
                await new HttpHandler(context).Do();
            }
        }
    }
}
