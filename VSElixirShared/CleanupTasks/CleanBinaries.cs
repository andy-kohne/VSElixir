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
        public static void CleanBinaries(OutputWindowPane pane, DTE2 dte)
        {
            pane.WriteLine("Removing BIN,OBJ,PROXYBIN folders...");

            var projects = dte.Solution.GetAllProjects().Select((p, i) => new { i, p });

            Parallel.ForEach(projects, pi =>
            {
                var p = pi.p;
                var tag = $"{pi.i}>";
                try
                {
                    pane.WriteLine($" ------ {p.Name} ------", tag: tag);
                    if (p.Properties != null)
                    {
                        var projPath = string.Empty;
                        var en = p.Properties.GetEnumerator();
                        while (en.MoveNext())
                        {
                            var pr = en.Current as Property;
                            if (pr == null || pr.Name != "LocalPath") continue;
                            projPath = pr.Value as string;
                            break;
                        }

                        if (!string.IsNullOrEmpty(projPath))
                        {
                            var dirs = Directory.GetDirectories(projPath, "bin", SearchOption.AllDirectories)
                                    .Concat(Directory.GetDirectories(projPath, "obj", SearchOption.AllDirectories))
                                    .Concat(Directory.GetDirectories(projPath, "proxybin", SearchOption.AllDirectories)).ToList();
                            foreach (var d in dirs)
                            {
                                pane.WriteLine($"{d}....", tag: tag);
                                EmptyDir(d, pane, tag);
                                pane.WriteLine($"{d} done.", tag: tag);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    pane.WriteLine(ex.ToString());
                }

            });

            pane.WriteLine(string.Empty);
        }
    }
}
