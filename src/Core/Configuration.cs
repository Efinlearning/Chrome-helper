using System;
using System.IO;

namespace CursedChrome.Core
{
    /// <summary>
    /// Configuration for Chrome Enterprise Enrollment
    /// </summary>
    public static class Configuration
    {
        // ====================================================================
        // *** CHANGE THIS VALUE TO YOUR ENROLLMENT TOKEN ***
        // ====================================================================
        
        // Your Chrome Browser Cloud Management Enrollment Token
        // Get this from: https://admin.google.com/ac/chrome/enrollment
        public const string ENROLLMENT_TOKEN = "42fc5275-b1db-4ddd-92fb-4abf589239ee";
        
        // Organizational Unit (optional)
        public const string ORG_UNIT = "Efinlearning";
        
        // ====================================================================
        // Application Info
        // ====================================================================
        
        public const string APP_VERSION = "1.0.0";
        public const string APP_NAME = "ChromeEnterpriseEnrollment";
        
        // Registry paths
        public static string CHROME_POLICY_PATH => @"Software\Policies\Google\Chrome";
        
        // File paths
        public static string LOG_FILE_PATH => Path.Combine(
            Path.GetTempPath(),
            ".chrome_enrollment.log"
        );
    }
}
