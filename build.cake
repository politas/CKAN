#tool nuget:?package=ILRepack&version=2.0.10

using System.Text.RegularExpressions;

var target = Argument<string>("target", "Build");
var configuration = Argument<string>("configuration", "Debug");
var solution = Argument<string>("solution", "CKAN.sln");

Task("BuildDotNet")
    .Does(() =>
{
    DotNetBuild(solution, settings =>
    {
        settings.Properties["win32icon"] = new List<string> { "GUI/assets/ckan.ico" };

        if (IsStable())
        {
            settings.Properties["DefineConstants"] = new List<string> { "STABLE" };
        }
    });
});

Task("BuildCkan")
    .IsDependentOn("BuildDotNet")
    .Does(() =>
{
    var assemblyPaths = GetFiles($"Cmdline/bin/{configuration}/*.dll");
    assemblyPaths.Add($"Cmdline/bin/{configuration}/CKAN-GUI.exe");

    ILRepack("ckan.exe", $"Cmdline/bin/{configuration}/CmdLine.exe", assemblyPaths,
        new ILRepackSettings
        {
            Libs = new List<FilePath> { $"Cmdline/bin/{configuration}" },
            // TODO: Cannot use the "TargetPlaform"
            // Must wait until https://github.com/cake-build/cake/pull/692 is released.
            ArgumentCustomization = builder => {
                builder.Append("/targetplatform:v4");
                return builder;
            }
        }
    );
});

Task("BuildNetkan")
    .IsDependentOn("BuildDotNet")
    .Does(() =>
{
    ILRepack("netkan.exe", $"Netkan/bin/{configuration}/NetKAN.exe", GetFiles($"Netkan/bin/{configuration}/*.dll"),
        new ILRepackSettings
        {
            Libs = new List<FilePath> { $"Netkan/bin/{configuration}" }
        }
    );
});

Task("Build")
    .IsDependentOn("BuildCkan")
    .IsDependentOn("BuildNetkan")
    .Does(() => {});

RunTarget(target);

private bool IsStable()
{
    IEnumerable<string> output;
    if (StartProcess("git", new ProcessSettings { Arguments = "rev-parse --abbrev-ref HEAD", RedirectStandardOutput = true }, out output) == 0)
    {
        var branch = output.Single();
        return Regex.IsMatch(branch, @"(\b|_)stable(\b|_)") || Regex.IsMatch(branch, @"v\d+\.\d*[02468]$");
    }
    else
    {
        return false;
    }
}
