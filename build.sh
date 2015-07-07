#!/bin/bash

pause() {
	read -n1 -r -p "Press any key to continue..." key
	echo ""
}

CONFIG="Mono43-Release"

MD=$(which mdtool)

SRC=$(pwd)

cd $SRC

rm -fr Lib
rm -fr Bin

echo "BUILD SAMPLES"
$MD build -c:$CONFIG Castle.Core.sln

echo "COMPLETE"
pause
