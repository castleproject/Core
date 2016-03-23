$frameworkVersion = "1.0.0-rc1-update1"
$frameworkArchitecture = "x86"

$dnxHome = Join-Path (Get-Location) ".dnx"
$dnxPath = Join-Path $dnxHome "bin"

Write-Host "Downloading dnvm to: $dnxPath"
if (!(Test-Path $dnxPath)) { md $dnxPath | Out-Null }

$dnvmPs1Path = Join-Path $dnxPath "dnvm.ps1"
$dnvmCmdPath = Join-Path $dnxPath "dnvm.cmd"

$wc = New-Object System.Net.WebClient
$wc.DownloadFile('https://raw.githubusercontent.com/aspnet/Home/dev/dnvm.ps1', $dnvmPs1Path)
$wc.DownloadFile('https://raw.githubusercontent.com/aspnet/Home/dev/dnvm.cmd', $dnvmCmdPath)

Write-Host "Downloading dnx"

$env:PATH = ("$dnxPath;" + $env:PATH)

dnvm install $frameworkVersion -runtime coreclr -arch $frameworkArchitecture

dnx --version

Write-Host "Downloading packages"

dnu restore

Write-Host "Building"

$env:Path = ((Join-Path $dnxHome "runtimes\dnx-coreclr-win-$frameworkArchitecture.$frameworkVersion\bin") + ";" + $env:Path)

dnu build src/Castle.Core src/Castle.Core.Tests --configuration Release --out build/NETCORE

Write-Host "Running tests"

cd src/Castle.Core.Tests
dnx test

exit $LASTEXITCODE
