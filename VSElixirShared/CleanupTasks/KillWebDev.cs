using VSElixir.Helpers;
using EnvDTE;
using EnvDTE80;

namespace VSElixir.CleanupTasks
{
    public static partial class CleanupTasks
    {
        public static void KillWebDev(OutputWindowPane pane, DTE2 dte)
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo =
                {
                    FileName = "cmd",
                    Arguments = "/C taskkill /im WebDev.WebServer40.EXE",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
            pane.Write("Killing WebDev.WebServer40...");
            process.Start();
            process.WaitForExit();
            pane.WriteLine(" done.");

            pane.WriteLine(string.Empty);
        }
    }
}
