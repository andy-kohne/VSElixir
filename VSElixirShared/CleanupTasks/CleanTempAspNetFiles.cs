using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web.Configuration;
using VSElixir.Helpers;

namespace VSElixir.CleanupTasks
{
    public static partial class CleanupTasks
    {

        public static void CleanTempAspNetFiles(OutputWindowPane pane, DTE2 dte)
        {
            var paths = new List<string>();

            // default paths
            var net_base = Path.GetFullPath(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), @"..\.."));
            paths.Add(string.Concat(net_base, @"\Framework\", RuntimeEnvironment.GetSystemVersion(), @"\Temporary ASP.NET Files"));
            paths.Add(string.Concat(net_base, @"\Framework64\", RuntimeEnvironment.GetSystemVersion(), @"\Temporary ASP.NET Files"));

            // custom path
            var config = WebConfigurationManager.OpenWebConfiguration("");
            var configSection = (CompilationSection)config.GetSection("system.web/compilation");
            if (!string.IsNullOrEmpty(configSection.TempDirectory))
                paths.Add(configSection.TempDirectory);

            pane.WriteLine("Removing Temporary ASP.Net Files... ");

            Parallel.ForEach(paths.Select((p,i) => new { Index = i, Path = p}), pathitem =>
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
