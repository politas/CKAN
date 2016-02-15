var target = Argument<string>("target", "Build");
var configuration = Argument<string>("configuration", "Debug");
var solution = Argument<string>("solution", "CKAN.sln");

Task("BuildDotNet")
    .Does(() =>
{
    DotNetBuild(solution);
});

Task("Build")
    .IsDependentOn("BuildDotNet")
    .Does(() => {});

RunTarget(target);
