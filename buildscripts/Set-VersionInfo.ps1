<#

.SYNOPSIS

Extracts build version information from Settings.proj and the
environment. Then constructs several flavors of version strings and
uses them, along with the current year to

  - update buildscripts/CommonAssemblyInfo.cs,
  - update all project.json files, and
  - emit TeamCity directives so it can know about the values

#>

[xml] $settings = Get-Content 'Settings.proj'
$version_group = $settings.Project.PropertyGroup[1]

$major = $version_group.Project_Major
$minor = $version_group.Project_Minor
$patch = $version_group.Project_Build
$build_number = [int]$env:build_number

if ( $env:release_build -eq "true" )
{
    $version_suffix = "$env:version_suffix"
}
else
{
    $version_suffix = "-ci{0:00000}" -f $build_number
}

$assembly_version = "$major.0.0";
$assembly_file_version = "$major.$minor.$patch"
$assembly_informational_version = "$assembly_file_version$version_suffix"
$year = (Get-Date).Year

Write-Host "##teamcity[setParameter name='ReleaseVersion' value='$assembly_informational_version']"
Write-Host "##teamcity[setParameter name='CurrentYear' value='$year']"

$common_assembly_info_file = "buildScripts\CommonAssemblyInfo.cs"
$content = (Get-Content -Raw -Path $common_assembly_info_file)

$content = $content.Replace('Copyright (c) 2004-$CurrentYear$', "Copyright (c) 2004-$year")
$content = $content.Replace("AssemblyVersion(`"0.0.0`")", "AssemblyVersion(`"$assembly_version`")")
$content = $content.Replace("AssemblyFileVersion(`"0.0.0`")", "AssemblyFileVersion(`"$assembly_file_version`")")
$content = $content.Replace("AssemblyInformationalVersion(`"0.0.0`")", "AssemblyInformationalVersion(`"$assembly_informational_version`")")

$content | Set-Content -Encoding UTF8 $common_assembly_info_file

# Change version in every project.json, except the tests, because
# the tests have assembly versions hard-coded.
# Inspired by https://github.com/c3-ls/CastleWindsor-MsExtensionsIntegration/blob/d2c5d361d25fc3260ca8fcaa36a4f34fa81ad7b9/build-set-version.ps1
Get-ChildItem -Filter project.json -Recurse |
    Where-Object { ! $_.Directory.Name.EndsWith(".Tests") } |
    ForEach-Object {
        $json = (Get-Content -Raw -Path $_.FullName | ConvertFrom-Json)

        $json.version = $assembly_informational_version

        $json | ConvertTo-Json -Depth 99 | Set-Content -Encoding UTF8 $_.FullName
    }
