using EnvDTE;
using System;
using System.IO;
using VSElixir.Helpers;

namespace VSElixir.CleanupTasks
{
    public static partial class CleanupTasks
    {
        public static void EmptyDir(string path, OutputWindowPane pane, string tag = null)
        {
            bool hasErrored = false;

            if (!Directory.Exists(path)) return;
            try
            {

                foreach (var p in Directory.GetDirectories(path))
                {
                    try
                    {
                        Directory.Delete(p, true);
                    }
                    catch (Exception ex)
                    {
                        if (!hasErrored)
                        {
                            hasErrored = true;
                        }
                        pane?.WriteLine($"ERROR deleting folder {p}; {ex.Message}", tag: tag);
                    }
                }

                foreach (var f in Directory.GetFiles(path))
                {
                    try
                    {
                        File.Delete(f);
                    }
                    catch (Exception ex)
                    {
                        if (!hasErrored)
                        {
                            hasErrored = true;
                        }
                        pane?.WriteLine($"ERROR deleting file {f}; {ex.Message}", tag: tag);
                    }
                }

            }
            catch (Exception)
            {
                // ignored
            }
        }

    }
}
