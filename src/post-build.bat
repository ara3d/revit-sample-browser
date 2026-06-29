@echo off
setlocal

set "AddinsDir=%AppData%\Autodesk\Revit\Addins\2025"
set "TargetDir=%AddinsDir%\Ara3D\RevitSampleBrowser"
set "OutputDir=%~1"

if "%OutputDir%"=="" (
  echo Usage: post-build.bat ^<output-directory^>
  exit /b 1
)

if not exist "%AddinsDir%" mkdir "%AddinsDir%"
if not exist "%TargetDir%" mkdir "%TargetDir%"

copy /Y "%~dp0Ara3D.RevitSampleBrowser.addin" "%AddinsDir%\Ara3D.RevitSampleBrowser.addin" >nul
robocopy "%OutputDir%" "%TargetDir%" /E /NFL /NDL /NJH /NJS /NP
if %ERRORLEVEL% LEQ 7 exit /b 0
exit /b %ERRORLEVEL%
