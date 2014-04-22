[xml] $settings = Get-Content 'Settings.proj'
$version_group = $settings.Project.PropertyGroup[1]

$major = $version_group.Project_Major
$minor = $version_group.Project_Minor
$build = $version_group.Project_Build

Write-Host "##teamcity[setParameter name='ReleaseVersion' value='$major.$minor.$build']"

