namespace ZumoApp;

public class ZumoProtocol
{
    public static void Write(string content)
    {
        var homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var targetDir = Path.Combine(homePath, ".zumo");
        var filePath = Path.Combine(targetDir, "protocol.txt");
        try
        {
            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

            // 4. In die Datei schreiben (hängt Text an, falls Datei schon existiert)
            var timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            File.WriteAllText(filePath, $"// Lino Meyer, Luca Bucher\n// {timestamp}\n{content}");

            Console.WriteLine($"Successfully saved: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Schreiben der Datei: {ex.Message}");
        }
    }
}