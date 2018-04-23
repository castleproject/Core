#!/bin/bash
# ****************************************************************************
# Copyright 2004-2017 Castle Project - http://www.castleproject.org/
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
# 
#     http://www.apache.org/licenses/LICENSE-2.0
# 
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
# ****************************************************************************

DOTNETPATH=$(which dotnet)
if [ ! -f "$DOTNETPATH" ]; then
	echo "Please install Microsoft/netcore from: https://www.microsoft.com/net/core"
	exit 1
fi
MONOPATH=$(which mono)

if [ ! -f "$MONOPATH" ]; then
	echo "Please install Xamarin/mono from: http://www.mono-project.com/docs/getting-started/install/"
	exit 1
fi

# This installs the latest build of the dotnet targetting pack net461 via NuGet for linux desktop clr builds.
# The package source can be removed once Microsoft.TargetingPack.NETFramework.v4.6.1 comes out of alpha on NuGet.
dotnet restore ./buildscripts/BuildScripts.csproj -s https://dotnet.myget.org/F/dotnet-core/api/v3/index.json
dotnet restore ./src/Castle.Core/Castle.Core.csproj
dotnet restore ./src/Castle.Services.Logging.log4netIntegration/Castle.Services.Logging.log4netIntegration.csproj
dotnet restore ./src/Castle.Services.Logging.NLogIntegration/Castle.Services.Logging.NLogIntegration.csproj
dotnet restore ./src/Castle.Services.Logging.SerilogIntegration/Castle.Services.Logging.SerilogIntegration.csproj
dotnet restore ./src/Castle.Core.Tests/Castle.Core.Tests.csproj

# Linux/Darwin
OSNAME=$(uname -s)
echo "OSNAME: $OSNAME"

dotnet build ./src/Castle.Core.Tests/Castle.Core.Tests.csproj /p:Configuration=Release || exit 1

echo --------------------
echo Running NET461 Tests
echo --------------------

./src/Castle.Core.Tests/bin/Release/net461/Castle.Core.Tests.exe --result=DesktopClrTestResults.xml;format=nunit3

echo ---------------------------
echo Running NETCOREAPP1.1 Tests
echo ---------------------------

dotnet ./src/Castle.Core.Tests/bin/Release/netcoreapp1.1/Castle.Core.Tests.dll --result=NetCoreClrTestResults.xml;format=nunit3

# Unit test failure
NETCORE_FAILCOUNT=$(grep -F "One or more child tests had errors" NetCoreClrTestResults.xml | wc -l)
if [ $NETCORE_FAILCOUNT -ne 0 ]
then
    echo "NetCore Tests have failed, failing the build"
    exit 1
fi

MONO_FAILCOUNT=$(grep -F "One or more child tests had errors" DesktopClrTestResults.xml | wc -l)
if [ $MONO_FAILCOUNT -ne 0 ]
then
    echo "DesktopClr Tests have failed, failing the build"
    exit 1
fi
