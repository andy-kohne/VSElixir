using VSElixir.Helpers;
using EnvDTE;
using EnvDTE80;

namespace VSElixir.CleanupTasks
{
    public static partial class CleanupTasks
    {
        public static void BuildSolution(OutputWindowPane pane, DTE2 dte)
        {
            pane.WriteLine("Building solution...");

            if (!dte.Solution.IsOpen)
            {
                pane.WriteLine("No solution loaded!", 1);
                return;
            }

            pane.Write("Initiating...  ", 1);
            dte.Solution.SolutionBuild.Build(false);
            pane.WriteLine("done.");
        }
    }
}
