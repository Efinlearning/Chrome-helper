using System;
using System.IO;

namespace CursedChrome.Core
{
    public static class Logger
    {
        private static bool _initialized = false;
        private static bool _debugMode = false;
        private static string _logFile;
        private static readonly object _lockObject = new object();
        
        public static void Initialize(bool debugMode)
        {
            _debugMode = debugMode;
            _logFile = Configuration.LOG_FILE_PATH;
            _initialized = true;
            
            if (_debugMode)
            {
                Log("Logger initialized in DEBUG mode");
            }
        }
        
        public static void Log(string message)
        {
            if (!_initialized)
                return;
            
            try
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
                
                if (_debugMode)
                {
                    Console.WriteLine(logEntry);
                }
                
                lock (_lockObject)
                {
                    File.AppendAllText(_logFile, logEntry + Environment.NewLine);
                }
            }
            catch
            {
                // Silent failure
            }
        }
        
        public static void LogError(string message, Exception ex = null)
        {
            if (ex != null)
            {
                Log($"ERROR: {message} - {ex.Message}");
                if (_debugMode)
                {
                    Log($"Stack trace: {ex.StackTrace}");
                }
            }
            else
            {
                Log($"ERROR: {message}");
            }
        }
        
        public static void LogWarning(string message)
        {
            Log($"WARNING: {message}");
        }
        
        public static void DeleteLogFile()
        {
            try
            {
                if (File.Exists(_logFile))
                {
                    File.Delete(_logFile);
                }
            }
            catch { }
        }
    }
}
