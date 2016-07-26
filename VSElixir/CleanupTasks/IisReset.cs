using VSElixir.Helpers;
using EnvDTE;
using EnvDTE80;

namespace VSElixir.CleanupTasks
{
    public static partial class CleanupTasks
    {
        public static void IisReset(OutputWindowPane pane, DTE2 dte)
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo =
                {
                    FileName = "cmd",
                    Arguments = "/C iisreset",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
            pane.Write("Resetting IIS...");
            process.Start();
            process.WaitForExit();
            pane.WriteLine(" done.");

            pane.WriteLine(string.Empty);
        }

    }
}
