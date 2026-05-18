using ZumoLib;

namespace ZumoApp;

public class ZumoLidar
{
    private static bool on;
    private static bool stop;

    /// <summary>
    ///     Schaltet das Lidar ein.
    /// </summary>
    public static void On()
    {
        if (on) return;
        Zumo.Instance.Lidar.SetPower(true);
        // Das Lidar muss zuerst ein wenig die Gegend scannen. 
        Console.Write("Init");
        for (var i = 0; i < 20; i++)
        {
            var p = Zumo.Instance.Lidar[45];
            Console.Write(".");
            Thread.Sleep(100);
        }

        on = true;
        Console.WriteLine();
    }

    /// <summary>
    ///     Schaltet die Beobachtung vor dem Zumo Roboter ein.
    /// </summary>
    /// <param name="distance">Distanz zum zu einem potentiellen Hindernis in mm.</param>
    public static void LookAt(short distance)
    {
        On();
        stop = false;
        Task.Run(() =>
        {
            while (!stop)
            {
                var p = Zumo.Instance.Lidar[0];
                if (p.Distance <= distance && p.Distance > 0)
                {
                    // Ein Hindernis wurde detektiert, höchste Zeit für den Notstopp.
                    Console.WriteLine(
                        $"Speed {Zumo.Instance.Lidar.Speed} °/sec \tDistance: {p.Distance / 1000f} m    ");
                    Zumo.Instance.Drive.Stop();
                    return;
                }

                Thread.Sleep(100);
            }
        });
    }

    /// <summary>
    ///     Schaltet das Lidar aus.
    /// </summary>
    public static void Off()
    {
        if (!on) return;
        on = false;
        stop = true;
        Zumo.Instance.Lidar.SetPower(false);
        // Der Notstopp muss für die nächste Fahrt wieder aufgehoben werden.
    }

    /// <summary>
    ///     Schaltet das Lidar aus.
    /// </summary>
    public static void StopLookAt()
    {
        stop = true;
        Off();
        // Der Notstopp muss für die nächste Fahrt wieder aufgehoben werden.
    }
}