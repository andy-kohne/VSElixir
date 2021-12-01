using VSElixir.Helpers;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
                var paths = new List<string>();

                if (!string.IsNullOrEmpty(solpath))
                {
                    paths.AddRange(Directory.GetDirectories(solpath, "packages", SearchOption.AllDirectories));
                }

                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (!string.IsNullOrEmpty(localAppData))
                {
                    paths.Add(Path.Combine(localAppData, @"NuGet\Cache"));
                }

                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                if (!string.IsNullOrEmpty(userProfile))
                {
                    paths.Add(Path.Combine(userProfile, @".nuget\packages"));
                }

                Parallel.ForEach(paths.Select((path, index) => new { Path = path, Index = index }), pathitem =>
                {
                    var tag = $"{pathitem.Index}>";
                    pane.WriteLine($" ------ {pathitem.Path} ------", tag: tag);
                    EmptyDir(pathitem.Path, pane, tag);
                    pane.WriteLine($" done.", tag: tag);
                });

            }
            catch (Exception ex)
            {
                pane.WriteLine(ex.ToString());
            }

            pane.WriteLine(string.Empty);
        }
    }
}
