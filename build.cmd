@echo off

REM ****************************************************************************
REM This script is intented for people who consider themselves powerusers.
REM ****************************************************************************

%~dp0SharedLibs\build\NAnt\bin\NAnt.exe -t:net-3.5 -D:nunit-console=%~dp0SharedLibs\build\NUnit\bin\nunit-console.exe -buildfile:default.build %*
