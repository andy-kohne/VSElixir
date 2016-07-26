using VSElixir.Helpers;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.IO;

namespace VSElixir.CleanupTasks
{
    public static partial class CleanupTasks
    {
        public static void CleanPackages(OutputWindowPane pane, DTE2 dte)
        {
            pane.WriteLine("Removing packages...");

            try
            {
                var sol = dte.Solution.FileName;
                var solpath = Path.GetDirectoryName(sol);
                var dirs = new List<string>();

                if (!string.IsNullOrEmpty(solpath))
                {
                    dirs.AddRange(Directory.GetDirectories(solpath, "packages", SearchOption.AllDirectories));
                }

                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (!string.IsNullOrEmpty(localAppData))
                {
                    dirs.Add(Path.Combine(localAppData, @"NuGet\Cache"));
                }

                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                if (!string.IsNullOrEmpty(userProfile))
                {
                    dirs.Add(Path.Combine(userProfile, @".nuget\packages"));
                }

                foreach (var p in dirs)
                {
                    pane.Write(p + "...", 1);
                    EmptyDir(p, pane);
                    pane.WriteLine("done.", 1);
                }

            }
            catch (Exception ex)
            {
                pane.WriteLine(ex.ToString());
            }

            pane.WriteLine(string.Empty);
        }
    }
}
