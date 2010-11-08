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
IF "%1" == "" goto no_nothing

IF /i "%1" == "NET40" (SET FrameworkVersion=v4.0)
IF /i "%1" == "NET40" (SET BuildConfigKey=NET40)

IF /i "%1" == "NET35" (SET FrameworkVersion=v3.5)
IF /i "%1" == "NET35" (SET BuildConfigKey=NET35)

IF /i "%1" == "MONO26" (SET FrameworkVersion=v3.5)
IF /i "%1" == "MONO26" (SET BuildConfigKey=MONO26)

IF /i "%1" == "MONO28" (SET FrameworkVersion=v3.5)
IF /i "%1" == "MONO28" (SET BuildConfigKey=MONO28)


IF /i "%1" == "SL3" (SET FrameworkVersion=v3.0)
IF /i "%1" == "SL3" (SET BuildConfigKey=SL30)
IF /i "%1" == "SL30" (SET FrameworkVersion=v3.0)
IF /i "%1" == "SL30" (SET BuildConfigKey=SL30)

IF /i "%1" == "SL4" (SET FrameworkVersion=v4.0)
IF /i "%1" == "SL4" (SET BuildConfigKey=SL40)
IF /i "%1" == "SL40" (SET FrameworkVersion=v4.0)
IF /i "%1" == "SL40" (SET BuildConfigKey=SL40)

IF "%2" == "" goto no_target_and_config
SET BuildTarget=%2

IF "%3" == "" goto no_config
SET BuildConfiguration=%3
goto build

:no_nothing
SET FrameworkVersion=v4.0
SET BuildConfigKey=NET40
SET BuildTarget=RunAllTests
SET BuildConfiguration=NET40-Release
goto build

:no_target_and_config
SET BuildTarget=RunAllTests
SET BuildConfiguration=%BuildConfigKey%-Release
goto build

:no_config
SET BuildConfiguration=%BuildConfigKey%-Release
goto build

:build
echo Framework version is: %FrameworkVersion%
echo Build Target is: %BuildTarget%
echo Building configuration: %BuildConfiguration%

SET __MSBUILD_EXE__=%windir%\microsoft.net\framework\v4.0.30319\msbuild.exe

@echo on
%__MSBUILD_EXE__% /m "%~dp0Build.proj" /p:Platform="Any CPU" /p:BuildConfigKey=%BuildConfigKey% /p:TargetFrameworkVersion=%FrameworkVersion% /ToolsVersion:4.0  /property:Configuration=%BuildConfiguration% /t:%BuildTarget%
@echo off

IF %ERRORLEVEL% NEQ 0 GOTO err
EXIT /B 0

:err
EXIT /B 1

:msbuild_not_configured
echo This project is not configured to be built with MSBuild.
echo Please use the NAnt script in the root folder of this project.
