var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var buildNumber = Argument("buildNumber", 0);

var solution = File("../CrossPlatformZip.sln");
var buildDir = Directory(".");
var deployDir = Directory("../Deploy");
var sourceRoot = Directory("../src");

var buildOutputDir = Directory("../output");
var artifactsDir = buildOutputDir + Directory("artifacts");
var testDir =  Directory("../tests/Firefly.CrossPlatformZip.Tests.Unit");
var testResultsDir =   MakeAbsolute(testDir + Directory("TestResults"));
var coverageResultsDir = MakeAbsolute(testDir + Directory("CoverageResults"));
var appBundleZip = File("bundle.zip");

var packageId = "Firefly.CrossPlatformZip";
var packageTitle = "Cross Platfom ZIP module";
var packageVersion = "1.0";
var packageCopyright = $"Copyright (c) Firefly Consulting Ltd. {DateTime.Now.Year}";
var packageDescription = @"
This package provides a mechanism to create or extract ZIP files for operating systems other than that on which you run this code.
Specifically, handling path sepatator characters in the zip central directory and setting of attributes for achives targeting Unix-like O/S.
Solves problems such as an archive conatining Windows path separators will not extract correctly on Linux.
";

Task("Clean")
    .Does(() =>
{
    CleanDirectories("../src/**/bin/" + configuration);
    CleanDirectories(testResultsDir.ToString());
    CleanDirectory(buildOutputDir);
});


Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    foreach (var framework in new[] { "net472", "netcoreapp2.0"})
    {
        DotNetCoreBuild(solution.ToString(), new DotNetCoreBuildSettings{
            Configuration = configuration,
            OutputDirectory = buildOutputDir + Directory(framework),
            Framework = framework
        });
    }
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {

        foreach(var proj in GetFiles(testDir + File("**/*.csproj")))
        {
            DotNetCoreTest(proj.FullPath);
        }
    });

Task("Package")
    .IsDependentOn("Test")
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

Task("Default")
    .IsDependentOn("Package");

RunTarget(target);