using EnvDTE;
using Microsoft.Web.Administration;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using Process = System.Diagnostics.Process;

namespace VSElixir.Helpers
{
    internal static class Iis
    {
        public static List<WorkerProcessDetail> GetWorkerProcesses(OutputWindowPane pane)
        {
            try
            {
                var workerProcessInfo = new List<WorkerProcessDetail>();
                var workerProcesses = Process.GetProcessesByName("w3wp");
                foreach (var workerProcess in workerProcesses)
                {
                    var query = $"Select CommandLine from Win32_Process Where ProcessID = '{workerProcess.Id}'";
                    using (var searcher = new ManagementObjectSearcher(new ObjectQuery(query)))
                    {
                        var objectCollection = searcher.Get();
                        var item =
                            (from ManagementBaseObject o in objectCollection
                             select Regex.Match(o["commandLine"].ToString(), "-ap \"([^\"]+)\""))
                                .FirstOrDefault(m => m.Success);
                        if (item != null)
                        {
                            workerProcessInfo.Add(new WorkerProcessDetail
                            {
                                ProcessId = workerProcess.Id,
                                AppPool = item.Groups[1].Value
                            });
                        }
                    }
                }
                return workerProcessInfo;
            }
            catch
            {
                pane.WriteLine("EXCEPTION WHILE TRYING TO FIND IIS WORKER PROCESSES!\nIs IDE running as admin?\n");
                return null;
            }
        }

        public static List<AppPoolMapping> FindIisPaths(OutputWindowPane pane)
        {
            try
            {
                var ret = new List<AppPoolMapping>();
                var serverManager = new ServerManager();
                foreach (var site in serverManager.Sites)
                {
                    ret.AddRange(site.Applications.Select(app => new AppPoolMapping
                    {
                        AppPoolName = app.ApplicationPoolName,
                        PhysicalPaths = app.VirtualDirectories.Select(v => v.PhysicalPath).ToList(),
                    }));
                }
                return ret;
            }
            catch
            {
                pane.WriteLine("EXCEPTION WHILE TRYING TO FIND IIS PATHS!\nIs IDE running as admin?\n");
                return null;
            }
        }

        public class WorkerProcessDetail
        {
            public int ProcessId { get; set; }
            public string AppPool { get; set; }
        }

        public class AppPoolMapping
        {
            public string AppPoolName { get; set; }
            public List<string> PhysicalPaths { get; set; }
        }

    }
}
