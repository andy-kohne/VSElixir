using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Globalization;

namespace VSElixir.Commands
{
    internal sealed class TitleCaseCommand
    {
        public const int CommandId = 2;

        private readonly Package _package;
        private IServiceProvider ServiceProvider => _package;

        public static TitleCaseCommand Instance { get; private set; }

        public static void Initialize(Package package)
        {
            Instance = new TitleCaseCommand(package);
        }

        private TitleCaseCommand(Package package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            _package = package;

            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandId = new CommandID(new Guid(VSElixirPackage.EditorCommandSetGuidString), CommandId);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandId);
                commandService.AddCommand(menuItem);
            }
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            var dte2 = (DTE2)ServiceProvider.GetService(typeof(DTE));
            var textSelection = dte2.ActiveWindow.Document.Selection as TextSelection;
            if (!string.IsNullOrEmpty(textSelection?.Text))
            {
                var textInfo = CultureInfo.CurrentCulture.TextInfo;
                textSelection.Text = textInfo.ToTitleCase(textSelection.Text);
            }
        }
    }
}