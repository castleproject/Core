#!/usr/bin/sh

iisreset

# make sure that cvs environment variables are setup correctly
export CVS_RSH=ssh
export CVSROOT=:ext:corts@cvs.sourceforge.net:/cvsroot/nvelocity

# build needed directories
mkdir -p ../dist
rm -rf ../tmp
mkdir -p ../tmp
cd ../tmp
cvs co NVelocity 
cd NVelocity

# remove unneeded directories for the release
rm -rf docs

# make sure that /bin files are executable
cd bin
chmod a+x *.exe
chmod a+x *.dll
cd NUnit
chmod a+x *.exe
chmod a+x *.dll
cd ../NAnt
chmod a+x *.exe
chmod a+x *.dll
cd ../NDoc
chmod a+x *.exe
chmod a+x *.dll

# make binaries and docs
cd ../../src
make release
cd ../dist
mv *.zip ../../../dist/

# cleanup
cd ../../../src
rm -rf ../tmp
