using System;
using EnvDTE;
using Microsoft.Web.Administration;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using Process = System.Diagnostics.Process;

namespace VSElixir.Helpers
{
    public class WindowsService
    {
        private static readonly string[] PROCESS_NAMES = { "ktbs.services.rpchost", "kaigapi.rpchost", "payrollapi.rpchost" };
        private static readonly string[] EXCLUDED = {
            "csrss", "conhost", "svchost", "msbuild", "chrome", "msedge", "dllhost", "cmd", "iisexpress", "winlogon", "system", "smss", "idle"
        };
        public static List<WindowsProcessDetail> GetWorkerProcesses(OutputWindowPane pane)
        {
            try
            {
                var list = new List<WindowsProcessDetail>();
                var processes = Process.GetProcesses().Where(x => !EXCLUDED.Contains(x.ProcessName.ToLower())).ToList();//windows services has not window title
                foreach (var p in processes)
                {
                    try
                    {
                        list.Add(new WindowsProcessDetail
                        {
                            Id = p.Id,
                            ProcessName = p.ProcessName,
                            Path = p.MainModule?.FileName ?? string.Empty
                        });
                    }
                    catch (System.ComponentModel.Win32Exception) { }//ignore win32
                    catch (System.InvalidOperationException) { }//ignore
                }

                return list;
            }
            catch (Exception ex)
            {
                pane.WriteLine("EXCEPTION WHILE TRYING TO FIND WINDOWS PROCESSES!\nIs IDE running as admin?\n");
                return null;
            }
        }


    }

    public class WindowsProcessDetail
    {
        public int Id { get; set; }
        public string ProcessName { get; set; }
        public string Path { get; set; }
    }
}
