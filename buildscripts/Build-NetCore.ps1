$dotnetHome = Join-Path (Get-Location) ".dotnet"
$dotnetPath = Join-Path $dotnetHome "bin"

Write-Host "Downloading install.ps1 to: $dotnetPath"
if (!(Test-Path $dotnetPath)) { md $dotnetPath | Out-Null }

$installPs1Path = Join-Path $dotnetPath "install.ps1"

$wc = New-Object System.Net.WebClient
$wc.DownloadFile('https://raw.githubusercontent.com/dotnet/cli/v1.0.0-preview2/scripts/obtain/dotnet-install.ps1', $installPs1Path)

$env:InstallDir = $dotnetHome
if (!(Test-Path env:\DOTNET_CLI_CHANNEL)) { $env:DOTNET_CLI_CHANNEL = "preview" }
if (!(Test-Path env:\DOTNET_CLI_VERSION)) { $env:DOTNET_CLI_VERSION = "1.0.0-preview2-003121" }

Write-Host "Downloading dotnet/cli"

. $installPs1Path -Channel $env:DOTNET_CLI_CHANNEL -InstallDir $dotnetHome -Version $env:DOTNET_CLI_VERSION

$env:PATH = ("$dotnetPath;" + $env:PATH)

dotnet --info

Write-Host "Downloading packages"

cd src
dotnet restore -v Minimal

Write-Host "Building"

cd Castle.Core
dotnet build --configuration Release --framework netstandard1.3 --output build/NETCORE
cd ../Castle.Services.Logging.NLogIntegration
dotnet build --configuration Release --framework netstandard1.3 --output build/NETCORE
cd ../Castle.Services.Logging.SerilogIntegration
dotnet build --configuration Release --framework netstandard1.3 --output build/NETCORE

Write-Host "Running tests"

cd ../Castle.Core.Tests
dotnet test

exit $LASTEXITCODE