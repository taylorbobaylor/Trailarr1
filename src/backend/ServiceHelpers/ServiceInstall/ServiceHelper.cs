using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;

namespace ServiceInstall;

public static class ServiceHelper
{
    private static string RadarrExe
    {
        get
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var directory = new FileInfo(assemblyLocation).Directory?.FullName
                ?? throw new InvalidOperationException("Unable to determine assembly directory");
            return Path.Combine(directory, "Radarr.Console.exe");
        }
    }

    private static bool IsAnAdministrator()
    {
        var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static void Run(string arg)
    {
        if (!File.Exists(RadarrExe))
        {
            Console.WriteLine("Unable to find Radarr.Console.exe in the current directory.");
            return;
        }

        if (!IsAnAdministrator())
        {
            Console.WriteLine("Access denied. Please run as administrator.");
            return;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = RadarrExe,
            Arguments = arg,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.OutputDataReceived += OnDataReceived;
        process.ErrorDataReceived += OnDataReceived;

        process.Start();

        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        process.WaitForExit();
    }

    private static void OnDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null)
        {
            Console.WriteLine(e.Data);
        }
    }
}
