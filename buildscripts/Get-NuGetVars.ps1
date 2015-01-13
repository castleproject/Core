[xml] $settings = Get-Content 'Settings.proj'
$version_group = $settings.Project.PropertyGroup[1]

$major = $version_group.Project_Major
$minor = $version_group.Project_Minor
$build = $version_group.Project_Build

if ( $env:release_build -eq "true" )
{
    $version_suffix = ""
}
else
{
    $version_suffix = "-ci{0:00000}" -f [int]$env:build_number
}

Write-Host "##teamcity[setParameter name='ReleaseVersion' value='$major.$minor.$build$version_suffix']"

$year = (Get-Date).Year

Write-Host "##teamcity[setParameter name='CurrentYear' value='$year']"