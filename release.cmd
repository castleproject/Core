@echo off
nant -t:net-1.1 -f:release.build package-net-1.1 %*
nant -t:net-2.0 -f:release.build package-net-2.0 %*
