#load "tools/includes.fsx"

open IntelliFactory.Build

let bt =
    BuildTool().PackageId("WebSharper.Ace")
        .VersionFrom("WebSharper")
        .WithFramework(fun x -> x.Net40)

let extension =
    bt.WebSharper.Extension("WebSharper.Ace")
        .SourcesFromProject()

bt.Solution [
    extension
    
    bt.NuGet.CreatePackage()
        .Configure(fun configuration ->
            { configuration with
                Title = Some "WebSharper.Ace"
                LicenseUrl = Some "http://websharper.com/licensing"
                ProjectUrl = Some "https://bitbucket.org/intellifactory/websharper.ace"
                Description = "WebSharper bindings for Ace."
                Authors = [ "IntelliFactory" ]
                RequiresLicenseAcceptance = true })
        .Add(extension)
]
|> bt.Dispatch
