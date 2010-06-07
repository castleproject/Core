@ECHO OFF
REM ****************************************************************************
REM Copyright 2004-2010 Castle Project - http://www.castleproject.org/
REM Licensed under the Apache License, Version 2.0 (the "License");
REM you may not use this file except in compliance with the License.
REM You may obtain a copy of the License at
REM 
REM     http://www.apache.org/licenses/LICENSE-2.0
REM 
REM Unless required by applicable law or agreed to in writing, software
REM distributed under the License is distributed on an "AS IS" BASIS,
REM WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
REM See the License for the specific language governing permissions and
REM limitations under the License.
REM ****************************************************************************

IF NOT EXIST %~dp0..\Settings.proj GOTO msbuild_not_configured

REM Set Framework version based on passed in parameter
IF "%1" == "" SET FrameworkVersion=v4.0.30319
IF "%1" == "NET40" (SET FrameworkVersion=v4.0.30319)
IF "%1" == "NET40" (SET BuildConfigKey=NET40)

IF "%1" == "NET35" (SET FrameworkVersion=v3.5)
IF "%1" == "NET35" (SET BuildConfigKey=NET35)

IF "%1" == "SL3" (SET FrameworkVersion=v3.0)
IF "%1" == "SL3" (SET BuildConfigKey=SL3)

IF "%1" == "SL4" (SET FrameworkVersion=v4.0)
IF "%1" == "SL4" (SET BuildConfigKey=SL4)

REM Set the build target, if not specified set it to "Package" target.
IF "%2" == "" (SET BuildTarget=Package) ELSE (SET BuildTarget=%2)

REM Set solution name to handle build for .NET / Silverlight
IF "%3" == "" (SET SolutionName=Castle.Core) ELSE (SET SolutionName=%3)

REM Write variables to console
echo Framework version is: %FrameworkVersion%
echo Build Target is: %BuildTarget%
echo Building solution: %SolutionName%

REM Always uses the MSBuild 4.0
SET __MSBUILD_EXE__=%windir%\microsoft.net\framework\v4.0.30319\msbuild.exe

REM Call the MSBuild to build the project
@echo on
%__MSBUILD_EXE__% /m "%~dp0Build.proj" /property:SolutionName=%SolutionName% /property:BuildConfigKey=%BuildConfigKey% /p:TargetFrameworkVersion=%FrameworkVersion% /ToolsVersion:4.0 /t:%BuildTarget% /property:Configuration=Release
@echo off

IF %ERRORLEVEL% NEQ 0 GOTO err
EXIT /B 0

:err
EXIT /B 1

:msbuild_not_configured
echo This project is not configured to be built with MSBuild.
echo Please use the NAnt script in the root folder of this project.