@ECHO OFF

REM Setup the build file
IF EXIST nant.build (
	set BUILD_FILE=nant.build
) ELSE (
	set BUILD_FILE=Castle.build
)

REM Check for Mono install, update path to include mono libs
IF EXIST %WINDIR%\monobasepath.bat (
	CALL %WINDIR%\monobasepath.bat
	
	REM Remove quotes from path
	SET CLEAN_MONO_BASEPATH=!MONO_BASEPATH:"=!

	SET PATH="!CLEAN_MONO_BASEPATH!\bin\;!CLEAN_MONO_BASEPATH!\lib\;%path%"
)

REM echo PATH is %PATH%

nant.exe "-buildfile:%BUILD_FILE%" %1 %2 %3 %4 %5 %6 %7 %8
GOTO End

REM ------------------------------------------
REM Expand a string to a full path
REM ------------------------------------------
:FullPath
set RESULT=%~f1
goto :EOF

REM ------------------------------------------
REM Compute the current directory
REM given a path to this batch script.
REM ------------------------------------------
:ComputeBase
set RESULT=%~dp1
REM Remove the trailing \
set RESULT=%RESULT:~0,-1%
Call :FullPath %RESULT%
goto :EOF

:End
ENDLOCAL
EXIT /B 0
