#r "paket: groupref Build //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO.Globbing.Operators
open Fake.DotNet

// Filesets

let appReferences = 
    !! "src/CricketEngine/CricketEngine.fsproj"
    ++ "src/MatchRunner/MatchRunner.fsproj"

let fableDirectory = "src/FableCricket"
let fableReferences =
    !! "src/FableCricket/FableCricket.fsproj"

let unitTestReferences =
    !! "tests/**/*UnitTests.fsproj"

let acceptanceTestReferences =
    !! "tests/**/*AcceptanceTests.fsproj"

let dotnetcliVersion = "2.1.302"

let install = lazy DotNet.install (fun p ->
    { p with Version = DotNet.CliVersion.Version dotnetcliVersion })

let inline dotnet arg = DotNet.Options.lift install.Value arg

let inline withWorkDir wd =
    DotNet.Options.lift install.Value
    >> DotNet.Options.withWorkingDirectory wd

let inline withCustomParams cp =
    DotNet.Options.lift install.Value
    >> DotNet.Options.withCustomParams (Some cp)

let inDebug =
    (fun (buildOptions: DotNet.BuildOptions) -> { buildOptions with Configuration = DotNet.BuildConfiguration.Debug })

// Targets

Target.create "Clean" (fun _ ->
    [ appReferences; unitTestReferences; acceptanceTestReferences ]
    |> Seq.concat
    |> Seq.iter (fun proj ->
        let result = DotNet.exec dotnet "clean" proj
        if not result.OK then failwithf "dotnet clean failed with code %i" result.ExitCode))

Target.create "UpdateVersionNumber" (fun _ ->
    let release =
        Fake.IO.File.read "RELEASE_NOTES.md"
        |> ReleaseNotes.parse
    let revisionFromCI = Environment.environVarOrNone "BUILD_BUILDID"
    let version =
        match revisionFromCI with
        | None -> release.AssemblyVersion
        | Some s -> sprintf "%s build %s" release.AssemblyVersion s
    let versionFiles = !! "**/Version.fs"
    Fake.IO.Shell.regexReplaceInFilesWithEncoding @"VersionNumber = "".*""" (sprintf @"VersionNumber = ""%s""" version) System.Text.Encoding.UTF8 versionFiles
    Trace.trace (sprintf @"Version = %s" version))

Target.create "Restore" (fun _ ->
    [ appReferences; unitTestReferences; acceptanceTestReferences; fableReferences ]
    |> Seq.concat
    |> Seq.iter (fun proj -> DotNet.restore (withCustomParams "--no-dependencies") proj))

Target.create "BuildApp" (fun _ ->
    appReferences
    |> Seq.iter (fun proj -> DotNet.build (withCustomParams "--no-dependencies --no-restore") proj))

Target.create "RunUnitTests" (fun _ ->
    unitTestReferences
    |> Seq.iter (fun proj -> DotNet.test (withCustomParams "--logger:trx --no-restore") proj))

Target.create "RunAcceptanceTests" (fun _ ->
    acceptanceTestReferences
    |> Seq.iter (fun proj -> DotNet.test (withCustomParams "--logger:trx --no-restore") proj))

Target.create "NpmInstall" (fun _ ->
    Fake.JavaScript.Npm.install (fun p -> { p with WorkingDirectory = fableDirectory }))

Target.create "BuildFable" (fun _ ->
    fableReferences
    |> Seq.iter (fun proj ->
        let result =
            DotNet.exec (withWorkDir fableDirectory) "fable npm-build" proj
        if result.ExitCode <> 0 then failwithf "'dotnet fable' failed in %s" fableDirectory))

Target.create "RunFable" (fun _ ->
    fableReferences
    |> Seq.iter (fun proj ->
        let result =
            DotNet.exec (withWorkDir fableDirectory) "fable npm-start" proj
        if result.ExitCode <> 0 then failwithf "'dotnet fable' failed in %s" fableDirectory))

// Build order

open Fake.Core.TargetOperators

"Clean"
    ==> "UpdateVersionNumber"
    ==> "Restore"
    ==> "BuildApp"
    ==> "RunUnitTests"
    ==> "RunAcceptanceTests"
    ==> "NpmInstall"

"NpmInstall" ==> "BuildFable"
"NpmInstall" ==> "RunFable"

// start build
Target.runOrDefault "BuildFable"