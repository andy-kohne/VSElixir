using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VSElixir.Helpers;

namespace VSElixir.CleanupTasks
{
    public static partial class CleanupTasks
    {
        public static void CleanTempFiles(OutputWindowPane pane, DTE2 dte)
        {
            var paths = new List<string>();

            paths.Add(Environment.GetEnvironmentVariable("temp", EnvironmentVariableTarget.Machine));

            pane.WriteLine("Removing Temporary Files...");

            Parallel.ForEach(paths.Select((path, index) => new {Path = path, Index = index}), pathitem =>
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
