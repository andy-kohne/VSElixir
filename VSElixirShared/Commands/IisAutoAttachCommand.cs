using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Threading;
using VSElixir.Helpers;

namespace VSElixir.Commands
{
    internal sealed class IisAutoAttachCommand
    {
        public const int CommandId = 2;

        private readonly Package _package;
        private IServiceProvider ServiceProvider => _package;

        public static IisAutoAttachCommand Instance {  get; private set; }

        public static void Initialize(Package package)
        {
            Instance = new IisAutoAttachCommand(package);
        }

        private IisAutoAttachCommand(Package package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            _package = package;

            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandId = new CommandID(new Guid(VSElixirPackage.ToolsCommandSetGuidString), CommandId);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandId);
                commandService.AddCommand(menuItem);
            }
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            // run on a new thread to keep VS responsive
            new System.Threading.Thread(AttachDebuggerToIis).Start();
        }

        private static int _running;

        public void AttachDebuggerToIis()
        {
            if (Interlocked.CompareExchange(ref _running, 1, 0) == 1) return;

            OutputWindowPane pane = null;
            try
            {
                pane = ((VSElixirPackage)_package).InitOutputPane();
                var ide = (DTE2)ServiceProvider.GetService(typeof(DTE));

                //// ensure current instance isn't already debugging
                //if (_applicationObject.Debugger.CurrentMode != dbgDebugMode.dbgDesignMode)
                //{
                //    WriteMessage("Debugger already attached!");
                //    return;
                //}

                // get the current solution
                var slnName = Path.GetFileNameWithoutExtension(ide.Solution.FullName);
                if (String.IsNullOrEmpty(slnName))
                {
                    pane.WriteLine("No solution active");
                    return;
                }
                pane.WriteLine($"Solution: {slnName}");

                // find physical paths in IIS
                pane.WriteLine("Looking for virtual directory physical paths...");
                var iisItems = Iis.FindIisPaths(pane);
                if (iisItems == null) return;

                if (!iisItems.Any())
                {
                    pane.WriteLine("NONE FOUND!");
                    return;
                }

                // find any projects sited in iis
                pane.WriteLine("Looking for projects sited in IIS...");
                var projects = ide.Solution.GetAllProjects()
                        .Where(p => p.Kind != EnvDTE.Constants.vsProjectKindUnmodeled)
                        .Where(p => iisItems.Any(i => !string.IsNullOrWhiteSpace(p.FullName) && i.PhysicalPaths.Contains(Path.GetDirectoryName(p.FullName), StringComparer.OrdinalIgnoreCase)))
                        .Select(p => new
                        {
                            ProjectName = p.Name,
                            AppPoolName = iisItems.First(i => i.PhysicalPaths.Contains(Path.GetDirectoryName(p.FullName), StringComparer.OrdinalIgnoreCase)).AppPoolName,
                        }).ToList();
                if (!projects.Any())
                {
                    pane.WriteLine("NONE FOUND!");
                    return;
                }
                foreach (var p in projects)
                    pane.WriteLine($"   {p.ProjectName} - {p.AppPoolName}");

                // find all the W3WP processes
                pane.WriteLine("Looking for W3WP processes...");
                var wps = Iis.GetWorkerProcesses(pane);
                if (wps == null) return;

                pane.WriteLine($"Found {wps.Count} w3wp processes");
                foreach (var wp in wps)
                    pane.WriteLine($"  {wp.AppPool} - {wp.ProcessId}");


                foreach (var appPoolName in projects.Select(p => p.AppPoolName).Distinct())
                {
                    pane.WriteLine($"Looking for {appPoolName} worker processes, used by {projects.First(o => string.Equals(o.AppPoolName, appPoolName, StringComparison.OrdinalIgnoreCase)).ProjectName}");
                    // attempt to match app pool name from worker process to project
                    var workerProcess = wps.SingleOrDefault(o => string.Equals(o.AppPool, appPoolName, StringComparison.OrdinalIgnoreCase));
                    if (workerProcess == null)
                    {
                        pane.WriteLine($"  Unable to determine the correct worker process for {appPoolName}!");
                        continue;
                    }
                    pane.WriteLine($"  Target worker process for {workerProcess.ProcessId}: {appPoolName}");

                    // look for the process thru the debugger
                    var en = ide.Debugger.LocalProcesses.GetEnumerator();
                    pane.WriteLine("  Searching for matching local process...");
                    while (en.MoveNext())
                    {
                        if (((Process2)en.Current).ProcessID != workerProcess.ProcessId) continue;
                        try
                        {
                            pane.WriteLine("  Attaching debugger...");
                            ((Process2)en.Current).Attach();
                        }
                        catch (Exception ex)
                        {
                            pane.WriteLine($"  ERROR: {ex}");
                        }
                        break;
                    }
                }
                pane.WriteLine("Done.");
            }
            catch (Exception ex)
            {
                pane?.OutputString("ERROR: " + ex);
            }
            finally
            {
                Interlocked.Exchange(ref _running, 0);
            }

        }

    }
}
