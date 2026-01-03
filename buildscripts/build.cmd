@ECHO OFF
REM ****************************************************************************
REM Copyright 2004-2025 Castle Project - http://www.castleproject.org/
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
dotnet build ./tools/Explicit.NuGet.Versions/Explicit.NuGet.Versions.slnx
dotnet build --configuration %Configuration% || exit /b 1
.\tools\Explicit.NuGet.Versions\build\nev.exe ".\build" "castle."
GOTO test

:test

echo --------------------
echo Running NET462 Tests
echo --------------------

dotnet test src\Castle.Core.Tests           -f net462 -c %Configuration% --no-build -- NUnit.TestOutputXml="%CD%" NUnit.TestOutputXmlFileName="DesktopClrTestResults"          || exit /b 1
dotnet test src\Castle.Core.Tests.WeakNamed -f net462 -c %Configuration% --no-build -- NUnit.TestOutputXml="%CD%" NUnit.TestOutputXmlFileName="DesktopClrWeakNamedTestResults" || exit /b 1

echo ---------------------------
echo Running NET8.0 Tests
echo ---------------------------

dotnet test src\Castle.Core.Tests           -f net8.0 -c %Configuration% --no-build -- NUnit.TestOutputXml="%CD%" NUnit.TestOutputXmlFileName="Net80TestResults"               || exit /b 1
dotnet test src\Castle.Core.Tests.WeakNamed -f net8.0 -c %Configuration% --no-build -- NUnit.TestOutputXml="%CD%" NUnit.TestOutputXmlFileName="Net80WeakNamedTestResults"      || exit /b 1

echo ---------------------------
echo Running NET9.0 Tests
echo ---------------------------

dotnet test src\Castle.Core.Tests           -f net9.0 -c %Configuration% --no-build -- NUnit.TestOutputXml="%CD%" NUnit.TestOutputXmlFileName="Net90TestResults"               || exit /b 1
dotnet test src\Castle.Core.Tests.WeakNamed -f net9.0 -c %Configuration% --no-build -- NUnit.TestOutputXml="%CD%" NUnit.TestOutputXmlFileName="Net90WeakNamedTestResults"      || exit /b 1