#addin nuget:?package=Cake.PowerShell&version=0.4.8

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

var mc = System.Text.RegularExpressions.Regex.Match(buildNumber, @"(?<bn>\d+)$");
var actualBuildNumber = mc.Groups["bn"].Value;

Task("Package")
    .Does(() =>
{
    EnsureDirectoryExists(artifactsDir);
    var nuGetPackSettings = new NuGetPackSettings
    {
        Id = packageId,
        OutputDirectory = artifactsDir,
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
    .WithCriteria(!string.IsNullOrEmpty(EnvironmentVariable("APPVEYOR_JOB_ID")))
    .IsDependentOn("Package")
    .Does(() => {

        var isTag = !string.IsNullOrEmpty(EnvironmentVariable("APPVEYOR_REPO_TAG")) && bool.Parse(EnvironmentVariable("APPVEYOR_REPO_TAG"));

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

Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);

string EnvironmentVariableOrDefault(string name, string defaultValue)
{
    var val = EnvironmentVariable(name);

    return val == null ? defaultValue : val;
}