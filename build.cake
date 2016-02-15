var target = Argument<string>("target", "Build");
var configuration = Argument<string>("configuration", "Debug");

Task("Build")
    .Does(() =>
{
    Information("Running Build");
});

RunTarget(target);
