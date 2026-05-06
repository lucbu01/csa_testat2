//    _____                            ____        __          __
//   /__  /  __  ______ ___  ____     / __ \____  / /_  ____  / /_
//     / /  / / / / __ `__ \/ __ \   / /_/ / __ \/ __ \/ __ \/ __/
//    / /__/ /_/ / / / / / / /_/ /  / _, _/ /_/ / /_/ / /_/ / /_
//   /____/\__,_/_/ /_/ /_/\____/  /_/ |_|\____/_.___/\____/\__/
//   (c) Hochschule Luzern T&A ========== www.hslu.ch ============
//

using System.Globalization;

namespace ZumoLib;

public class Drive : ComDevice
{
    private readonly object locker = new();
    private readonly EventWaitHandle wh = new AutoResetEvent(false);
    private bool stop;

    public Drive(ICom com) : base(com, 0x24)
    {
        TurnCalib(115); // 100 entspricht keine Korrektur (=1.00)
    }

    public string Response { get; set; } = "start";
    private event EventHandler DriveFinished;

    /// <summary>
    ///     Fährt eine Strecke gerade aus und wartet bis die Fahrt fertig ist.
    /// </summary>
    /// <param name="length">die zu fahrende Strecke in mm (negativer Wert => rückwärts)</param>
    /// <param name="speed"></param>
    /// <param name="acceleration"></param>
    /// <param name="offset">Korrekturfaktor in 0.1mm/s</param>
    public void Track(short length, ushort speed, ushort acceleration, sbyte offset = 0)
    {
        lock (locker)
        {
            if (stop)
                return;
            var msg = SetRequest($"C{length:X4}{speed:X4}{acceleration:X4}{offset:X2}");
            Response = string.Concat(Response, msg + "\n");
        }

        wh.WaitOne();
    }

    /// <summary>
    ///     Dreht an Ort und Stelle und wartet bis das Drehen fertig ist.
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="speed"></param>
    /// <param name="acceleration"></param>
    public void Turn(short angle, ushort speed, ushort acceleration)
    {
        lock (locker)
        {
            if (stop) return;
            var msg = SetRequest($"A{angle:X4}{speed:X4}{acceleration:X4}");
            Response = string.Concat(Response, msg + "\n");
        }

        wh.WaitOne();
    }

    /// <summary>
    ///     Dreht an Ort und Stelle und wartet bis das Drehen fertig ist.
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="speed"></param>
    /// <param name="acceleration"></param>
    /// <param name="factor">Korrekturfaktor Istwinkel zu Sollwinkel</param>
    public void Turn(short angle, ushort speed, ushort acceleration, short factor)
    {
        lock (locker)
        {
            if (stop) return;
            var msg = SetRequest($"B{factor:X4}");
            msg = SetRequest($"A{angle:X4}{speed:X4}{acceleration:X4}");
            Response = string.Concat(Response, msg + "\n");
        }

        wh.WaitOne();
    }

    /// <summary>
    ///     Setzt den Korrekturfaktor für den Fahrbefehl "An Ort drehen".
    ///     100 entspricht 1.00,
    ///     115 entspricht beispielweise einem Korrekturfaktor von 1.15 (Istwinkel zu Sollwinkel)
    /// </summary>
    /// <param name="factor">Korrekturfaktor Istwinkel zu Sollwinkel</param>
    public void TurnCalib(short factor)
    {
        var msg = SetRequest($"B{factor:X4}");
        Response = string.Concat(Response, msg + "\n");
    }

    /// <summary>
    ///     Liefert die restliche Distanz zurück, bis der Zumo anhält (Fahrbefehl fertig ist)
    /// </summary>
    /// <returns>Die Distanz in mm</returns>
    public int GetRemainingDistance()
    {
        var msg = GetRequest("2");
        var dist = int.Parse(msg.Substring(4), NumberStyles.HexNumber);
        Response = string.Concat(Response, msg + "\n");
        return dist;
    }

    /// <summary>
    ///     Liefert True zurück, solange ein Fahrbefehl ausgeführt wird
    /// </summary>
    /// <returns>true solange der Zumo fährt</returns>
    public bool IsRunning()
    {
        var msg = GetRequest("7");
        var running = byte.Parse(msg.Substring(4), NumberStyles.HexNumber) == 1;
        return running;
    }

    /// <summary>
    ///     Stoppt die Fahrt.
    /// </summary>
    public void Stop()
    {
        lock (locker)
        {
            stop = true;
            var msg = SetRequest("100000000");
            Response = string.Concat(Response, msg + "...Stop\n");
            wh.Set();
        }
    }

    /// <summary>
    ///     Gibt die Fahrt wieder frei.
    /// </summary>
    public void ResetStop()
    {
        stop = false;
    }

    protected override bool ProcessEvent(string message)
    {
        if (message == "5!24FF")
        {
            DriveFinished?.Invoke(this, EventArgs.Empty);
            wh.Set();
            return true;
        }

        return false;
    }
}