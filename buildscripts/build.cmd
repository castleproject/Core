@ECHO OFF
REM ****************************************************************************
REM Copyright 2004-2013 Castle Project - http://www.castleproject.org/
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

if "%1" == "" goto no_config 
if "%1" NEQ "" goto set_config 

:set_config
SET Configuration=%1
GOTO restore_packages

:no_config
SET Configuration=Release
GOTO restore_packages

:restore_packages
dotnet restore ./tools/Explicit.NuGet.Versions/Explicit.NuGet.Versions.csproj
dotnet restore ./src/Castle.Core/Castle.Core.csproj
dotnet restore ./src/Castle.Core.Tests/Castle.Core.Tests.csproj
dotnet restore ./src/Castle.Core.Tests.WeakNamed/Castle.Core.Tests.WeakNamed.csproj
dotnet restore ./src/Castle.Services.Logging.log4netIntegration/Castle.Services.Logging.log4netIntegration.csproj
dotnet restore ./src/Castle.Services.Logging.NLogIntegration/Castle.Services.Logging.NLogIntegration.csproj
dotnet restore ./src/Castle.Services.Logging.SerilogIntegration/Castle.Services.Logging.SerilogIntegration.csproj
GOTO build

:build
rem Should be the line below but because of https://github.com/Microsoft/msbuild/issues/1333 we needed to use msbuild instead.
rem dotnet build Castle.Core.sln -c %Configuration%
dotnet build ./tools/Explicit.NuGet.Versions/Explicit.NuGet.Versions.sln
msbuild /p:Configuration=%Configuration% || exit /b 1
msbuild /p:Configuration=%Configuration% /t:Pack || exit /b 1
.\tools\Explicit.NuGet.Versions\build\nev.exe ".\build" "castle."
GOTO test

:test

echo --------------------
echo Running NET461 Tests
echo --------------------

%UserProfile%\.nuget\packages\nunit.consolerunner\3.6.1\tools\nunit3-console.exe src/Castle.Core.Tests/bin/%Configuration%/net461/Castle.Core.Tests.exe --result=DesktopClrTestResults.xml;format=nunit3 || exit /b 1
%UserProfile%\.nuget\packages\nunit.consolerunner\3.6.1\tools\nunit3-console.exe src/Castle.Core.Tests.WeakNamed/bin/%Configuration%/net461/Castle.Core.Tests.WeakNamed.exe --result=DesktopClrWeakNamedTestResults.xml;format=nunit3 || exit /b 1

echo ---------------------------
echo Running NETCOREAPP1.1 Tests
echo ---------------------------

dotnet .\src\Castle.Core.Tests\bin\%Configuration%\netcoreapp1.1\Castle.Core.Tests.dll --result=NetCoreClrTestResults.xml;format=nunit3 || exit /b 1
dotnet .\src\Castle.Core.Tests.WeakNamed\bin\%Configuration%\netcoreapp1.1/Castle.Core.Tests.WeakNamed.dll --result=NetCoreClrWeakNamedTestResults.xml;format=nunit3 || exit /b 1
