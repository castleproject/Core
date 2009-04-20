@echo off
call build.cmd
if errorlevel 1 goto end
echo **************************************************************
echo The binaries can be found in the build\net-3.5\release folder.
echo **************************************************************

:end
pause
