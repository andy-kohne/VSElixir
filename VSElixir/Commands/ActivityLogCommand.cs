using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace VSElixir.Commands
{
    internal sealed class ActivityLogCommand
    {
        public const int CommandId = 3;

        private readonly Package _package;
        private IServiceProvider ServiceProvider => _package;

        public static ActivityLogCommand Instance { get; private set; }

        public static void Initialize(Package package)
        {
            Instance = new ActivityLogCommand(package);
        }

        private ActivityLogCommand(Package package)
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
            var dte2 = (DTE2)ServiceProvider.GetService(typeof(DTE));
            var path = ActivityLog.LogFilePath;

            if (File.Exists(path))
                dte2.Documents.Open(path);
            else
                VsShellUtilities.ShowMessageBox(
                    ServiceProvider,
                    path + " not found",
                    "VSElixir",
                    OLEMSGICON.OLEMSGICON_WARNING,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
