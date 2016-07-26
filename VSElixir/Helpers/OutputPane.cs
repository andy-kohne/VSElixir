using EnvDTE;

namespace VSElixir.Helpers
{
    public static class OutputPane
    {
        public static void WriteLine(this OutputWindowPane pane, string message, int indent = 0)
        {
            pane.Write(message, indent);
            pane.OutputString("\n");
        }

        public static void Write(this OutputWindowPane pane, string message, int indent = 0)
        {
            pane.OutputString(new string(' ', indent * 2) + message);
        }

    }
}
