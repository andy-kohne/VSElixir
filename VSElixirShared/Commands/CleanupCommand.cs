using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using System.Windows;
using DelbinOptions = VSElixir.Dialog.DelbinOptions;

namespace VSElixir.Commands
{
    internal sealed class CleanupCommand
    {
        public const int CommandId = 4;

        private readonly Package _package;
        private IServiceProvider ServiceProvider => _package;

        public static CleanupCommand Instance { get; private set; }

        public static void Initialize(Package package)
        {
            Instance = new CleanupCommand(package);
        }

        private CleanupCommand(Package package)
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

        private Dictionary<string, bool> _options = null;

        private void MenuItemCallback(object sender, EventArgs e)
        {
            if (_running != 0) return;

            var dte2 = ServiceProvider.GetService(typeof(DTE)) as DTE2;
            var dialog = new DelbinOptions
            {
                Owner = Application.Current.MainWindow,
                SolutionLoaded = dte2?.Solution?.IsOpen ?? false
            };

            dialog.Options = _options;

            var result = dialog.ShowDialog();

            if (result.GetValueOrDefault())
            {
                _options = dialog.Options;

                // determine what items to run
                var tasks = new List<CleanupTask>();

                if (dialog.chkResetIIS.IsChecked.GetValueOrDefault()) tasks.Add(CleanupTasks.CleanupTasks.IisReset);
                if (dialog.chkKillWebDev.IsChecked.GetValueOrDefault()) tasks.Add(CleanupTasks.CleanupTasks.KillWebDev);

                if (dialog.chkPackages.IsChecked.GetValueOrDefault()) tasks.Add(CleanupTasks.CleanupTasks.CleanPackages);
                if (dialog.chkTempAsp.IsChecked.GetValueOrDefault()) tasks.Add(CleanupTasks.CleanupTasks.CleanTempAspNetFiles);
                if (dialog.chkTemp.IsChecked.GetValueOrDefault()) tasks.Add(CleanupTasks.CleanupTasks.CleanTempFiles);
                if (dialog.chkReflectedSchemas.IsChecked.GetValueOrDefault()) tasks.Add(CleanupTasks.CleanupTasks.CleanReflectedSchemas);
                if (dialog.chkBin.IsChecked.GetValueOrDefault()) tasks.Add(CleanupTasks.CleanupTasks.CleanBinaries);
                if (dialog.chkWebsiteCache.IsChecked.GetValueOrDefault()) tasks.Add(CleanupTasks.CleanupTasks.CleanWebsiteCache);

                if (dialog.chkBuild.IsChecked.GetValueOrDefault()) tasks.Add(CleanupTasks.CleanupTasks.BuildSolution);

                // run on a new thread to keep VS responsive
                new System.Threading.Thread(RunCleanupTasks).Start(tasks);
            }

        }

        private int _running;

        private delegate void CleanupTask(OutputWindowPane pane, DTE2 dte);


        private void RunCleanupTasks(object parameter)
        {
            var tasks = parameter as IEnumerable<CleanupTask>;
            if (tasks == null) return;

            if (Interlocked.CompareExchange(ref _running, 1, 0) == 1) return;

            OutputWindowPane pane = null;

            try
            {
                pane = ((VSElixirPackage)_package).InitOutputPane();

                // Get an instance of currently running Visual Studio IDE.
                var dte2 = ServiceProvider.GetService(typeof(DTE)) as DTE2;

                pane.OutputString("Beginning cleanup:\n\n");

                foreach (var t in tasks)
                    t(pane, dte2);

                pane.OutputString("\nComplete.\n");
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
