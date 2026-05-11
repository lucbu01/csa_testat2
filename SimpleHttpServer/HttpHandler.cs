using System.Net.Sockets;
using System.IO;
using System.Net;

namespace SimpleHttpServer {

    public class HttpHandler(HttpListenerContext client)
    {
        public async Task Do() {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".zumo", "protocol.txt");
            try
            {
                if (!File.Exists(filePath))
                {
                    client.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    client.Response.StatusDescription = "File not found";
                    client.Response.Close();
                    return;

                }

                var fileBytes = await File.ReadAllBytesAsync(filePath);

                client.Response.ContentType = "text/plain";
                client.Response.ContentLength64 = fileBytes.Length;

                await client.Response.OutputStream.WriteAsync(fileBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler: {ex.Message}");
                client.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            finally
            {
                client.Response.Close();
            }
        }
    }
}
