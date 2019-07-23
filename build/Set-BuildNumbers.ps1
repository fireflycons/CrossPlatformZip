$tag = $env:APPVEYOR_REPO_TAG_NAME
$localBuildNumber = $env:APPVEYOR_BUILD_NUMBER
$localBuildVersion = "0.0.$($localBuildNumber)"
$localAssemblyVersion = "0.0.0.0"

$version = $null

if ($tag -and $tag -match '^v(?<version>\d+\.\d+\.\d+)$')
{
    $version = [Version]$Matches.version
}

if ($version)
{
    $localBuildNumber = $version.Build.ToString()
    $localBuildVersion = $version.ToString()
    $localAssemblyVersion = "$(version.ToString(2)).0.0"
}

Set-AppveyorBuildVariable -Name LOCAL_BUILD_NUMBER -Value $localBuildNumber
Set-AppveyorBuildVariable -Name LOCAL_BUILD_VERSION -Value $localBuildVersion
Set-AppveyorBuildVariable -Name LOCAL_ASSEMBLY_VERSION -Value $localAssemblyVersion

