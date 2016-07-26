using System;
using System.IO;
using System.Linq;
using VSElixir.Helpers;
using EnvDTE;
using EnvDTE80;

namespace VSElixir.CleanupTasks
{

    public static partial class CleanupTasks
    {
        public static void CleanBinaries(OutputWindowPane pane, DTE2 dte)
        {
            pane.WriteLine("Removing BIN,OBJ,PROXYBIN folders...");

            var projects = dte.Solution.GetAllProjects();

            foreach (var p in projects)
            {
                try
                {
                    pane.WriteLine(p.Name, 1);
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
                                pane.Write(d + "...", 1);
                                EmptyDir(d, pane);
                                pane.WriteLine("done.", 1);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    pane.WriteLine(ex.ToString());
                }
            }

            pane.WriteLine(string.Empty);
        }
    }
}
