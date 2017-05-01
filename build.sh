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

if [ ! -d "./.dotnet" ]; then

    sudo apt-get -qq update

    sudo add-apt-repository ppa:ubuntu-toolchain-r/test -y

    sudo apt-get install -y libunwind8

    wget -O - https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0/scripts/obtain/dotnet-install.sh | bash -s -- -i ./.dotnet/

fi

#RUNTIME="ubuntu.12.04-x64"
#RUNTIME="ubuntu.16.04-x64"
#RUNTIMEOPTS=" --runtime $RUNTIME"
RUNTIMEOPTS=""

./.dotnet/dotnet restore ./buildscripts/BuildScripts.csproj "$RUNTIMEOPS"

./.dotnet/dotnet restore ./src/Castle.Core/Castle.Core-VS2017.csproj "$RUNTIMEOPS"

./.dotnet/dotnet restore ./src/Castle.Services.Logging.log4netIntegration/Castle.Services.Logging.log4netIntegration-VS2017.csproj "$RUNTIMEOPS"

./.dotnet/dotnet restore ./src/Castle.Services.Logging.NLogIntegration/Castle.Services.Logging.NLogIntegration-VS2017.csproj "$RUNTIMEOPS"

./.dotnet/dotnet restore ./src/Castle.Services.Logging.SerilogIntegration/Castle.Services.Logging.SerilogIntegration-VS2017.csproj "$RUNTIMEOPS"

./.dotnet/dotnet restore ./src/Castle.Core.Tests/Castle.Core.Tests-VS2017.csproj "$RUNTIMEOPS"

# Linux/Darwin

OSNAME=$(uname -s)

echo "OSNAME: $OSNAME"

./.dotnet/dotnet build ./src/Castle.Core.Tests/Castle.Core.Tests-VS2017.csproj /p:Configuration=Release /p:OsName=$OSNAME

echo ---------------------------
echo Running NETCOREAPP1.1 Tests
echo ---------------------------

./.dotnet/dotnet ./src/Castle.Core.Tests/bin/Release/netcoreapp1.1/Castle.Core.Tests.dll 
