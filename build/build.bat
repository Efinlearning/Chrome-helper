@echo off
REM ============================================================================
REM Build Script for Chrome Enterprise Enrollment Tool
REM ============================================================================

echo.
echo ========================================================================
echo          Chrome Enterprise Enrollment Tool - Build Script
echo ========================================================================
echo.

REM Find MSBuild
set MSBUILD_PATH=
for /f "tokens=*" %%i in ('dir /b /s "%ProgramFiles(x86)%\Microsoft Visual Studio\*\MSBuild.exe" 2^>nul') do (
    set MSBUILD_PATH=%%i
    goto :found_msbuild
)

:found_msbuild
if "%MSBUILD_PATH%"=="" (
    echo ERROR: MSBuild not found!
    echo Please install Visual Studio or .NET Framework SDK
    pause
    exit /b 1
)

echo [*] Using MSBuild: %MSBUILD_PATH%
echo.

REM Build
echo [*] Building project...
"%MSBUILD_PATH%" CursedChrome-EnterpriseOnly.csproj /t:Build /p:Configuration=Release /v:minimal

if %errorlevel% equ 0 (
    echo.
    echo ========================================================================
    echo          Build Successful!
    echo ========================================================================
    echo.
    echo [+] Executable: bin\Release\ChromeEnterpriseEnrollment.exe
    for %%A in ("bin\Release\ChromeEnterpriseEnrollment.exe") do echo [+] Size: %%~zA bytes
    echo.
) else (
    echo.
    echo ERROR: Build failed!
    echo.
)

pause
