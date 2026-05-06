using System.Runtime.CompilerServices;
using ZumoLib;

namespace ZumoApp {
    public class ZumoProtocol
    {

        public static void Write(string content)
        {
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string targetDir = Path.Combine(homePath, ".zumo");
            string filePath = Path.Combine(targetDir, "protocol.txt");
            try 
            {
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                // 4. In die Datei schreiben (hängt Text an, falls Datei schon existiert)
                string timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                File.WriteAllText(filePath, $"// Lino Meyer, Luca Bucher\n// {timestamp}\n{content}");

                Console.WriteLine($"Erfolgreich gespeichert unter: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Schreiben der Datei: {ex.Message}");
            }
        }
    }
}
