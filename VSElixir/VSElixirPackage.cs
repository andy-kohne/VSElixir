using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using VSElixir.Commands;
using VSElixir.Helpers;

namespace VSElixir
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("VSElixir", "Miscellaneous additions to VS", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class VSElixirPackage : Package
    {
        public const string PackageGuidString = "2224D3D0-49B5-4F23-B5C0-C36FFC0C6FE0";
        internal const string ToolsCommandSetGuidString = "17B6D526-DD11-450A-93E6-E3DEDD75BC23";
        internal const string EditorCommandSetGuidString = "F7F07ABC-BC08-4245-9F50-70DBB0C7E666";

        #region Package Members

        protected override void Initialize()
        {
            base.Initialize();
            _version = GetVersion();
            AddOutputPane();
            IisAutoAttachCommand.Initialize(this);
            ActivityLogCommand.Initialize(this);
            CleanupCommand.Initialize(this);
            TitleCaseCommand.Initialize(this);
        }

        #endregion

        private void AddOutputPane()
        {
            var dte2 = (DTE2)GetService(typeof(DTE));
            dte2.ToolWindows.OutputWindow.OutputWindowPanes.Add(OUTPUT_WINDOW_NAME);
        }

        private const string OUTPUT_WINDOW_NAME = "VSElixir";
        private string _version;

        public OutputWindowPane InitOutputPane()
        {
            var dte2 = (DTE2)GetService(typeof(DTE));
            dte2.ToolWindows.OutputWindow.Parent.Activate();
            foreach (OutputWindowPane pane in dte2.ToolWindows.OutputWindow.OutputWindowPanes)
            {
                if (pane.Name == OUTPUT_WINDOW_NAME)
                {
                    pane.Activate();
                    pane.Clear();
                    pane.WriteLine($"Extension Version {_version}\n");
                    return pane;
                }
            }
            return null;
        }

        private static string GetVersion()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var assemblyUri = new UriBuilder(assembly.CodeBase);
                var assemblyPath = Uri.UnescapeDataString(assemblyUri.Path);
                var assemblyDirectory = Path.GetDirectoryName(assemblyPath);
                var vsixManifestPath = Path.Combine(assemblyDirectory, "extension.vsixmanifest");

                var doc = new XmlDocument();
                doc.Load(vsixManifestPath);

                if (doc.DocumentElement == null || doc.DocumentElement.Name != "PackageManifest") return null;

                var metaData = doc.DocumentElement.ChildNodes.Cast<XmlElement>().First(x => x.Name == "Metadata");
                var identity = metaData.ChildNodes.Cast<XmlElement>().First(x => x.Name == "Identity");

                return identity.GetAttribute("Version");
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
