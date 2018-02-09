using EnvDTE;

namespace VSElixir.Helpers
{
    public static class OutputPane
    {
        public static void WriteLine(this OutputWindowPane pane, string message, int indent = 0, string tag = null)
        {
            pane.OutputString(Format(tag, message, indent) + '\n');
        }

        public static void Write(this OutputWindowPane pane, string message, int indent = 0, string tag = null)
        {
            pane.OutputString(Format(tag, message, indent));
        }

        private static string Format(string tag, string message, int indent) => $"{new string(' ', indent * 2)}{tag}{message}";
    }
}
