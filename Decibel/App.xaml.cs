using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace Decibel
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Process currentProcess = Process.GetCurrentProcess();

            var runningInstances = Process.GetProcessesByName(currentProcess.ProcessName)
                .Where(p => p.Id != currentProcess.Id);

            foreach (var instance in runningInstances)
            {
                try
                {
                    instance.Kill();

                    instance.WaitForExit(3000);
                }
                catch { }
            }
            base.OnStartup(e);
        }
    }

}
