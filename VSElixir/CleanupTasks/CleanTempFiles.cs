using VSElixir.Helpers;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;

namespace VSElixir.CleanupTasks
{
    public static partial class CleanupTasks
    {
        public static void CleanTempFiles(OutputWindowPane pane, DTE2 dte)
        {
            var paths = new List<string>();

            paths.Add(Environment.GetEnvironmentVariable("temp", EnvironmentVariableTarget.Machine));

            pane.WriteLine("Removing Temporary Files...");

            foreach (var p in paths)
            {
                pane.Write(p + "...", 1);
                EmptyDir(p, pane);
                pane.WriteLine("done.", 1);
            }

            pane.WriteLine(string.Empty);
        }
    }
}
