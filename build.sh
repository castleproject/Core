#!/bin/bash

# ****************************************************************************
# Copyright 2004-2013 Castle Project - http://www.castleproject.org/
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

dotnet restore ./buildscripts/BuildScripts.csproj

dotnet restore ./src/Castle.Core/Castle.Core-VS2017.csproj

dotnet restore ./src/Castle.Services.Logging.log4netIntegration/Castle.Services.Logging.log4netIntegration-VS2017.csproj

dotnet restore ./src/Castle.Services.Logging.NLogIntegration/Castle.Services.Logging.NLogIntegration-VS2017.csproj

dotnet restore ./src/Castle.Services.Logging.SerilogIntegration/Castle.Services.Logging.SerilogIntegration-VS2017.csproj

dotnet restore ./src/Castle.Core.Tests/Castle.Core.Tests-VS2017.csproj

# Linux/Darwin

OSNAME=$(uname -s)

echo "OSNAME: $OSNAME"

echo --------------------
echo Running NET461 Tests
echo --------------------

xbuild /p:Configuration=NET45-Release /t:RunAllTests buildscripts/Build.proj

echo ---------------------------
echo Running NETCOREAPP1.1 Tests
echo ---------------------------

dotnet build ./src/Castle.Core.Tests/Castle.Core.Tests-VS2017.csproj /p:Configuration=Release /p:OsName=$OSNAME

dotnet ./src/Castle.Core.Tests/bin/Release/netcoreapp1.1/Castle.Core.Tests.dll --result=NetCoreClrTestResults.xml;format=nunit3

# Build Failure 

MONO_FAILCOUNT=$(grep -F "One or more child tests had errors" build/NET45/NET45-Release/bin/test-results/nunit-results.xml | wc -l)

if [ $MONO_FAILCOUNT -ne 0 ]
    then
        echo "Mono Tests have failed, failing the build"
        exit 1 
    fi
