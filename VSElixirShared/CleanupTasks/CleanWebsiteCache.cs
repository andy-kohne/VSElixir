using EnvDTE;
using EnvDTE80;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VSElixir.Helpers;

namespace VSElixir.CleanupTasks
{
    public static partial class CleanupTasks
    {
        public static void CleanWebsiteCache(OutputWindowPane pane, DTE2 dte)
        {
            var websiteCacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\WebSiteCache");
            pane.WriteLine("Cleaning WebSiteCache...");

            Parallel.ForEach(
                Directory.GetDirectories(websiteCacheFolder).Select((path, index) => new {Path = path, Index = index}),
                pathitem =>
                {
                    var tag = $"{pathitem.Index}>";
                    pane.WriteLine($" ------ {pathitem.Path} ------", tag: tag);
                    EmptyDir(pathitem.Path, pane, tag);
                    pane.WriteLine($" done.", tag: tag);
                });

            pane.WriteLine(string.Empty);
        }
    }
}
