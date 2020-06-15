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
GOTO build

:no_config
SET Configuration=Release
GOTO build

:build
dotnet build ./tools/Explicit.NuGet.Versions/Explicit.NuGet.Versions.sln
dotnet build --configuration %Configuration% || exit /b 1
.\tools\Explicit.NuGet.Versions\build\nev.exe ".\build" "castle."
GOTO test

:test

echo --------------------
echo Running NET461 Tests
echo --------------------

%UserProfile%\.nuget\packages\nunit.consolerunner\3.11.1\tools\nunit3-console.exe src/Castle.Core.Tests/bin/%Configuration%/net461/Castle.Core.Tests.exe --result=DesktopClrTestResults.xml;format=nunit3 || exit /b 1
%UserProfile%\.nuget\packages\nunit.consolerunner\3.11.1\tools\nunit3-console.exe src/Castle.Core.Tests.WeakNamed/bin/%Configuration%/net461/Castle.Core.Tests.WeakNamed.exe --result=DesktopClrWeakNamedTestResults.xml;format=nunit3 || exit /b 1

echo ---------------------------
echo Running NETCOREAPP3.1 Tests
echo ---------------------------

dotnet .\src\Castle.Core.Tests\bin\%Configuration%\netcoreapp3.1\Castle.Core.Tests.dll --result=NetCoreClrTestResults.xml;format=nunit3 || exit /b 1
dotnet .\src\Castle.Core.Tests.WeakNamed\bin\%Configuration%\netcoreapp3.1/Castle.Core.Tests.WeakNamed.dll --result=NetCoreClrWeakNamedTestResults.xml;format=nunit3 || exit /b 1
