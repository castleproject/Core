#!/bin/bash
# ****************************************************************************
# Copyright 2004-2025 Castle Project - http://www.castleproject.org/
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
shopt -s expand_aliases

dotnet --list-sdks

echo ---------------------------
echo Build
echo ---------------------------

dotnet build ./Castle.Core.slnx --configuration Release --tl:off

echo ---------------------------
echo Running NET8.0 Tests
echo ---------------------------

dotnet test ./src/Castle.Core.Tests           -f net8.0 -c Release --no-build -- NUnit.TestOutputXml="$PWD" NUnit.TestOutputXmlFileName="Net80TestResults"
dotnet test ./src/Castle.Core.Tests.WeakNamed -f net8.0 -c Release --no-build -- NUnit.TestOutputXml="$PWD" NUnit.TestOutputXmlFileName="Net80WeakNamedTestResults"
 
# Ensure that all test runs produced a protocol file:
if [[ !( -f Net80TestResults.xml &&
         -f Net80WeakNamedTestResults.xml ) ]]; then
    echo "Incomplete test results. Some test runs might not have terminated properly. Failing the build."
    exit 1
fi

NET80_FAILCOUNT=$(grep -F "One or more child tests had errors" Net80TestResults.xml Net80WeakNamedTestResults.xml | wc -l)
if [ $NET80_FAILCOUNT -ne 0 ]
then
    echo "Net8.0 Tests have failed, failing the build"
    exit 1
fi

echo ---------------------------
echo Running NET9.0 Tests
echo ---------------------------

dotnet test ./src/Castle.Core.Tests           -f net9.0 -c Release --no-build -- NUnit.TestOutputXml="$PWD" NUnit.TestOutputXmlFileName="Net90TestResults"
dotnet test ./src/Castle.Core.Tests.WeakNamed -f net9.0 -c Release --no-build -- NUnit.TestOutputXml="$PWD" NUnit.TestOutputXmlFileName="Net90WeakNamedTestResults"

# Ensure that all test runs produced a protocol file:
if [[ !( -f Net90TestResults.xml &&
         -f Net90WeakNamedTestResults.xml ) ]]; then
    echo "Incomplete test results. Some test runs might not have terminated properly. Failing the build."
    exit 1
fi

NET90_FAILCOUNT=$(grep -F "One or more child tests had errors" Net90TestResults.xml Net90WeakNamedTestResults.xml | wc -l)
if [ $NET90_FAILCOUNT -ne 0 ]
then
    echo "Net9.0 Tests have failed, failing the build"
    exit 1
fi
