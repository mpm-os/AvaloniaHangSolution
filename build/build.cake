#addin nuget:?package=Cake.Npm&version=0.17.0

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var TestResultsDirectory = Argument("TestResultsDirectory", "../");
var BuildPath= "../Outsystems.GuidedTour.sln";
var restore=Task("Restore-Packages")
    .Does(() =>
    {
        Information("Starting Restore");
        var settings = new DotNetCoreRestoreSettings
        {

            Interactive = true
        };
        DotNetCoreRestore(BuildPath,settings);
		NpmUpdate();
        Information("Ending Restore");
    });

var build = Task("Build")
    .Does(()=>
    {
        Information("Starting Build");
        var settings = new DotNetCoreMSBuildSettings();
        DotNetCoreMSBuild(BuildPath, settings.SetConfiguration(configuration));
        Information("Ending Build");
    });

var tests = Task("Tests")
    .Does(()=>
    {
        Information("Starting Tests");
        var settings = new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoRestore = true,
            NoBuild = true,
            ArgumentCustomization = args=>args.Append(@"/p:CollectCoverage=true")
                                            .Append(@"/p:CoverletOutputFormat=cobertura")
											.Append(@"/p:ExcludeByFile=**/GitVersionInformation.*")
                                            .Append(@"/p:CoverletOutput="+TestResultsDirectory+@"\coverage.xml")
        };
        
        var solutionFiles = GetFiles("../*.sln");
        foreach(var file in solutionFiles)
        {
            Information("Running tests for solution file " + file);
            DotNetCoreTest(file.FullPath, settings);
        }
        Information("Ending Tests");
    });


var packageNuspecFile = Task("Package")
    .Does(()=>
    {
        Information("Starting Package");
        var settings = new DotNetCorePackSettings
        {
            Configuration = configuration,
            NoRestore = true,
            NoBuild = true,
            Verbosity = DotNetCoreVerbosity.Normal,
            OutputDirectory = "../artifacts/"
        };
        DotNetCorePack("../",settings);
        Information("Ending Package");
    });

var fullRun = Task("FullRun")
    .IsDependentOn("Restore-Packages")
    .IsDependentOn("Build")
    .IsDependentOn("Tests")
    .IsDependentOn("Package");

Task("Default")
    .IsDependentOn("FullRun");

RunTarget(target);
