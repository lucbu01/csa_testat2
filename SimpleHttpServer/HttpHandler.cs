using System.Net;

namespace SimpleHttpServer;

public class HttpHandler(HttpListenerContext client)
{
    public void Do(object state)
    {
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".zumo",
            "protocol.txt");
        try
        {
            if (client.Request.HttpMethod != "GET")
            {
                client.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                client.Response.StatusDescription = "Method not allowed";
                client.Response.Close();
                return;
            }

            if (!File.Exists(filePath) || client.Request.RawUrl != "/")
            {
                client.Response.StatusCode = (int)HttpStatusCode.NotFound;
                client.Response.StatusDescription = "File not found";
                client.Response.Close();
                return;
            }

            var fileBytes = File.ReadAllBytes(filePath);

            client.Response.ContentType = "text/plain";
            client.Response.ContentLength64 = fileBytes.Length;

            client.Response.OutputStream.Write(fileBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            client.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
        finally
        {
            client.Response.Close();
        }
    }
}