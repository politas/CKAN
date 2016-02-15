var target = Argument<string>("target", "Build");
var configuration = Argument<string>("configuration", "Debug");
var solution = Argument<string>("solution", "CKAN.sln");

Task("BuildDotNet")
    .Does(() =>
{
    DotNetBuild(solution);
});

Task("BuildCkan")
    .IsDependentOn("BuildDotNet")
    .Does(() =>
{
    var ilrepackArgs = new List<FilePath>
    {
        "/target:exe",
        "/out:ckan.exe",
        "Cmdline/bin/Debug/CmdLine.exe",
        "Cmdline/bin/Debug/CKAN-GUI.exe",
        "Cmdline/bin/Debug/ChinhDo.Transactions.dll",
        "Cmdline/bin/Debug/CKAN.dll",
        "Cmdline/bin/Debug/CommandLine.dll",
        "Cmdline/bin/Debug/ICSharpCode.SharpZipLib.dll",
        "Cmdline/bin/Debug/log4net.dll",
        "Cmdline/bin/Debug/Newtonsoft.Json.dll",
        "Cmdline/bin/Debug/INIFileParser.dll",
        "Cmdline/bin/Debug/CurlSharp.dll"
    };

    // TODO: http://cakebuild.net/dsl/ilrepack
    StartProcess("Core/packages/ILRepack.1.25.0/tools/ILRepack.exe", string.Join(" ", ilrepackArgs));
});

Task("BuildNetkan")
    .IsDependentOn("BuildDotNet")
    .Does(() =>
{
    var ilrepackArgs = new List<FilePath>
    {
        "/target:exe",
        "/out:netkan.exe",
        "Netkan/bin/Debug/NetKAN.exe",
        "Cmdline/bin/Debug/ChinhDo.Transactions.dll",
        "Cmdline/bin/Debug/CKAN.dll",
        "Cmdline/bin/Debug/CommandLine.dll",
        "Cmdline/bin/Debug/ICSharpCode.SharpZipLib.dll",
        "Cmdline/bin/Debug/log4net.dll",
        "Cmdline/bin/Debug/Newtonsoft.Json.dll",
        "Cmdline/bin/Debug/INIFileParser.dll",
        "Cmdline/bin/Debug/CurlSharp.dll"
    };

    // TODO: http://cakebuild.net/dsl/ilrepack
    StartProcess("Core/packages/ILRepack.1.25.0/tools/ILRepack.exe", string.Join(" ", ilrepackArgs));
});

Task("Build")
    .IsDependentOn("BuildCkan")
    .IsDependentOn("BuildNetkan")
    .Does(() => {});

RunTarget(target);
