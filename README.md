# Chrome Enterprise Enrollment Tool

Simple tool to enroll Chrome browser into your organization's Cloud Management.

## What It Does

- Sets Chrome Browser Cloud Management enrollment token
- Configures Chrome to be managed by your organization
- NO extension installation
- NO C2 communication
- Just enrollment

## Configuration

Edit `src/Core/Configuration.cs`:
```csharp
public const string ENROLLMENT_TOKEN = "YOUR-TOKEN-HERE";
public const string ORG_UNIT = "YourOrganization";
```

## Build
```batch
cd build
build.bat
```

## Usage
```batch
# Normal mode (with confirmation dialog)
ChromeEnterpriseEnrollment.exe

# Silent mode (no dialogs)
ChromeEnterpriseEnrollment.exe /silent

# Debug mode (show console)
ChromeEnterpriseEnrollment.exe /debug
```

## Verify Enrollment

1. Open Chrome
2. Go to `chrome://policy`
3. Should show: "This browser is managed"
4. Find: `CloudManagementEnrollmentToken`
5. Value should match your token

## Registry Changes
```
HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Google\Chrome
- CloudManagementEnrollmentToken = "42fc5275-b1db-4ddd-92fb-4abf589239ee"
- CloudManagementEnrollmentMandatory = 1
```

## Removal
```batch
reg delete "HKLM\SOFTWARE\Policies\Google\Chrome" /v CloudManagementEnrollmentToken /f
reg delete "HKLM\SOFTWARE\Policies\Google\Chrome" /v CloudManagementEnrollmentMandatory /f
```

Then restart Chrome.
