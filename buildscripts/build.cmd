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

IF "%FrameworkVersion%" == "" SET FrameworkVersion=v2.0.50727
IF "%Framework35Version%" == "" SET Framework35Version=v3.5

REM Select the correct MSBuild version
IF "%FrameworkVersion%" == "v2.0.50727" SET __MSBUILD_EXE__=%windir%\microsoft.net\framework\v3.5\msbuild.exe
IF "%FrameworkVersion%" == "v4.0.21006" SET __MSBUILD_EXE__=%windir%\microsoft.net\framework\v4.0.21006\msbuild.exe

IF DEFINED CLICKTOBUILD GOTO quick
IF "%~1" == "" GOTO quick

ECHO ON
%__MSBUILD_EXE__% /m "%~dp0Build.proj" /p:TargetFrameworkVersion=%Framework35Version% %*
@ECHO OFF
@GOTO end

:quick
IF "%FrameworkVersion%" == "v2.0.50727" SET __OUTDIR__=%~dp0..\build\.NETFramework-v3.5\Release\
IF "%FrameworkVersion%" == "v4.0.21006" SET __OUTDIR__=%~dp0..\build\.NETFramework-v4.0\Release\
ECHO ON
%__MSBUILD_EXE__% /m "%~dp0Build.proj" /t:CleanAll;BuildProject /p:OutputPath=%__OUTDIR__% /p:Configuration=Release /p:Platform=AnyCPU /p:TargetFrameworkVersion=%Framework35Version%
@ECHO OFF
IF "%CLICKTOBUILD%" == "1" EXIT /B %ERRORLEVEL%

:end
IF %ERRORLEVEL% NEQ 0 GOTO err
EXIT /B 0

:err
EXIT /B 1

:msbuild_not_configured
echo This project is not configured to be built with MSBuild.
echo Please use the NAnt script in the root folder of this project.