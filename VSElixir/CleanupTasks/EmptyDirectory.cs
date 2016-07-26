using System;
using System.IO;
using EnvDTE;
using VSElixir.Helpers;

namespace VSElixir.CleanupTasks
{
    public static partial class CleanupTasks
    {
        public static void EmptyDir(string path, OutputWindowPane pane)
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
                            pane?.WriteLine(string.Empty);
                            hasErrored = true;
                        }
                        pane?.WriteLine($"ERROR deleting folder {p}; {ex.Message}",4);
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
                            pane?.WriteLine(string.Empty);
                            hasErrored = true;
                        }
                        pane?.WriteLine($"ERROR deleting file {f}; {ex.Message}",4);
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
