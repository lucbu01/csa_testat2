using System.Diagnostics;

internal class Program
{
    private static async Task Main()
    {
        var netcore = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "netcore");
        var zumo = Path.Combine(netcore, "ZumoApp", "ZumoApp.dll");
        var http = Path.Combine(netcore, "SimpleHttpServer", "SimpleHttpServer.dll");

        Console.WriteLine("Starting Processes... (Cancel with Ctrl+C)");

        using var processZumo = StartProcess("dotnet", zumo, "ZUMO");
        using var processHttp = StartProcess("dotnet", http, "HTTP");

        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            Console.WriteLine("\n[MAIN] Stopping...");

            StopProcessSafely(processZumo);
            StopProcessSafely(processHttp);
        };

        await Task.WhenAll(
            processZumo.WaitForExitAsync(),
            processHttp.WaitForExitAsync()
        );

        Console.WriteLine("[MAIN] All processes where cancelled!");
    }

    private static Process StartProcess(string command, string args, string logPrefix)
    {
        var info = new ProcessStartInfo(command, args)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var process = new Process { StartInfo = info };

        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine($"[{logPrefix}] {e.Data}");
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"[{logPrefix} ERR] {e.Data}");
                Console.ResetColor();
            }
        };

        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        return process;
    }

    private static void StopProcessSafely(Process p)
    {
        try
        {
            if (p != null && !p.HasExited) p.Kill(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MAIN] Error while killing process: {ex.Message}");
        }
    }
}