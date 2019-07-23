#addin nuget:?package=Cake.PowerShell&version=0.4.8

using System.Text;
using System.Text.RegularExpressions;
using Cake.Powershell;

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var buildNumber = EnvironmentVariableOrDefault("APPVEYOR_BUILD_NUMBER", "0");


var buildOutputDir = Directory("../output");
var artifactsDir = buildOutputDir + Directory("artifacts");
var testDir =  Directory("../tests/Firefly.CrossPlatformZip.Tests.Unit");
var testResultsDir =   MakeAbsolute(testDir + Directory("TestResults"));
var coverageResultsDir = MakeAbsolute(testDir + Directory("CoverageResults"));
var appBundleZip = File("bundle.zip");

var packageId = "Firefly.CrossPlatformZip";
var packageTitle = "Cross Platfom ZIP module";
var packageVersion = "0.1";
var packageCopyright = $"Copyright (c) Firefly Consulting Ltd. {DateTime.Now.Year}";
var packageDescription = @"
This package provides a mechanism to create or extract ZIP files for operating systems other than that on which you run this code.
Specifically, handling path sepatator characters in the zip central directory and setting of attributes for achives targeting Unix-like O/S.
Solves problems such as an archive conatining Windows path separators will not extract correctly on Linux.
";

var mc = Regex.Match(buildNumber, @"(?<bn>\d+)$");
var actualBuildNumber = mc.Groups["bn"].Value;

Task("Package")
    .Does(() =>
{
    var nuGetPackSettings = new NuGetPackSettings
    {
        Id = packageId,
        Version = $"{packageVersion}.{buildNumber}",
        Copyright = packageCopyright,
        Title = packageTitle,
        Description = packageDescription,
        Properties = new Dictionary<string, string>
        {
            { "Configuration", configuration }
        }
    };

    NuGetPack($"../src/{packageId}/package.nuspec", nuGetPackSettings);
});

Task("Publish")
    .WithCriteria(!string.IsNullOrEmpty(EnvironmentVariable("APPVEYOR")))
    .IsDependentOn("Package")
    .Does(() => {

        var isTagPush = !string.IsNullOrEmpty(EnvironmentVariable("APPVEYOR_REPO_TAG")) && bool.Parse(EnvironmentVariable("APPVEYOR_REPO_TAG"));

        // Push AppVeyor artifact
        foreach(var package in GetFiles($"../**/{packageId}*.nupkg"))
        {
            Information($"Pushing {package} as AppVeyor artifact");

            StartPowershellScript("Push-AppveyorArtifact", new PowershellSettings
                {
                    Modules = new List<string> {
                        "build-worker-api"
                    },

                    FormatOutput = true,
                    LogOutput = true
                }
                .WithArguments(args =>
                {
                    args.AppendQuoted(package.ToString());
                })
            );
        }
    });

Task("SetBuildNumbers")
    .WithCriteria(!string.IsNullOrEmpty(EnvironmentVariable("APPVEYOR")))
    .Does(() => {

        var version = BuildVersionFromRepoTagName();
        var localBuildNumber = EnvironmentVariable("APPVEYOR_BUILD_NUMBER");
        var localBuildVersion = $"0.0.{localBuildNumber}";
        var localAssemblyVersion = "0.0.0.0";

        if (version != null)
        {
            localBuildNumber = version.Build.ToString();
            localBuildVersion = version.ToString();
            localAssemblyVersion = $"{version.ToString(2)}.0.0";
        }

        var sb = new StringBuilder();

        sb.AppendLine($"Set-AppveyorBuildVariable -Name LOCAL_BUILD_NUMBER -Value {localBuildNumber}")
            .AppendLine($"Set-AppveyorBuildVariable -Name LOCAL_BUILD_VERSION -Value {localBuildVersion}")
            .AppendLine($"Set-AppveyorBuildVariable -Name LOCAL_ASSEMBLY_VERSION -Value {localAssemblyVersion}");

        StartPowershellScript(sb.ToString(), new PowershellSettings
            {
                Modules = new List<string> {
                    "build-worker-api"
                },

                FormatOutput = true,
                LogOutput = true
            });
    });

Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);

string EnvironmentVariableOrDefault(string name, string defaultValue)
{
    var val = EnvironmentVariable(name);

    return val == null ? defaultValue : val;
}

Version BuildVersionFromRepoTagName()
{
    var tag = EnvironmentVariable("APPVEYOR_REPO_TAG_NAME");

    if (tag == null)
    {
        return null;
    }

    var m = Regex.Match(tag, @"^v(?<version>\d+\.\d+\.\d+)$", RegexOptions.IgnoreCase);

    return m.Success ? new Version(m.Groups["version"].Value) : null;
}