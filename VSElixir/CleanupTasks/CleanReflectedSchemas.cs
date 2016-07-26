using VSElixir.Helpers;
using EnvDTE;
using EnvDTE80;
using System;
using System.IO;

namespace VSElixir.CleanupTasks
{
    public static partial class CleanupTasks
    {
        public static void CleanReflectedSchemas(OutputWindowPane pane, DTE2 dte)
        {
            var path =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Microsoft\VisualStudio\12.0\ReflectedSchemas");

            pane.WriteLine("Removing Reflected Schemas...");

            foreach (var p in new [] { path})
            {
                pane.Write(p + "...", 1);
                EmptyDir(p, pane);
                pane.WriteLine("done.", 1);
            }

            pane.WriteLine(string.Empty);
        }
    }
}
