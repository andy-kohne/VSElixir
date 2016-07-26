using VSElixir.Helpers;
using EnvDTE;
using EnvDTE80;
using System;
using System.IO;

namespace VSElixir.CleanupTasks
{
    public static partial class CleanupTasks
    {
        public static void CleanWebsiteCache(OutputWindowPane pane, DTE2 dte)
        {
            var websiteCacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\WebSiteCache");
            pane.WriteLine("Cleaning WebSiteCache...");

            foreach (var p in Directory.GetDirectories(websiteCacheFolder))
            {
                pane.Write(p + "...", 1);
                EmptyDir(p, pane);
                pane.WriteLine("done.", 1);
            }
            EmptyDir(websiteCacheFolder, pane);

            pane.WriteLine(string.Empty);
        }
    }
}
