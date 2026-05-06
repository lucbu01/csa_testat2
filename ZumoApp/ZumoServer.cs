using ZumoLib;

namespace ZumoApp {
    public class ZumoServer {
        private static bool finished = false;
        static void Main() {
            try
            {
                ZumoLidar.On();
                while (!finished)
                {
                    Console.WriteLine("Press A or B or C and <Enter> to start...");
                    ZumoLidar.LookAt(200);
                    string choise = Console.ReadLine();
                    string response = "wrong choise";
                    switch (choise.ToUpper())
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
                    ZumoLidar.StopLookAt();
                    Console.WriteLine(response);
                }
            }
            finally
            {
                ZumoLidar.Off();
            }
        }
    }
}
