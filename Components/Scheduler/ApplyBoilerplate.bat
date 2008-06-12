@echo off
REM Applies license boilerplate to all code files.

setlocal
set BOILERPLATE_DIR=%~dp0

echo Adding missing license boilerplate...
call :APPLY_BOILERPLATE "*.cs" "%BOILERPLATE_DIR%\CSLicenseBoilerplate.txt"
call :APPLY_BOILERPLATE "*.sql" "%BOILERPLATE_DIR%\SQLLicenseBoilerplate.txt"

echo.
exit /b 0


:APPLY_BOILERPLATE
set PATTERN=%~1
set LICENSE_FILE=%~2

for /R "." %%V in (%PATTERN%) do (
  findstr /R /C:"Copyright .* Castle Project - http://www\.castleproject\.org/" "%%V" >nul
  if errorlevel 1 (
    echo - %%V
    REM Use sed to strip out UTF8 byte order marks before adding the boilerplate.
    type "%LICENSE_FILE%" "%%V" 2>nul | sed -e "s/\xEF\xBB\xBF//" > "%%V.new"
    if errorlevel 1 (
      echo     Error applying boilerplate!
    ) else (
      move /Y "%%V.new" "%%V" >nul
      if errorlevel 1 (
        echo     Error replacing source file!
      )
    )
  )
)
goto :EOF

