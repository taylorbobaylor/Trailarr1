using System;
using System.Diagnostics;

namespace NzbDrone.Test.Dummy;

public class DummyApp
{
    public const string DUMMY_PROCCESS_NAME = "Radarr.Test.Dummy";

    private static void Main(string[] args)
    {
        var process = Process.GetCurrentProcess();
        var mainModule = process.MainModule;
        var fileName = mainModule?.FileName ?? "Unknown";

        Console.WriteLine("Dummy process. ID:{0} Name:{1} Path:{2}", process.Id, process.ProcessName, fileName);
        Console.ReadLine();
    }
}
