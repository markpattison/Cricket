#r "paket: groupref Build //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO.Globbing.Operators
open Fake.DotNet

// Filesets

let serverReferences = !! "src/Server/Server.fsproj"

let appReferences = 
    !! "src/Shared/CricketEngine/CricketEngine.fsproj"
    ++ "src/Shared/MatchRunner/MatchRunner.fsproj"

let fableDirectory = "src/Client"

let fableReferences =
    !! "src/Client/Client.fsproj"

let unitTestReferences =
    !! "tests/**/*UnitTests.fsproj"

let acceptanceTestReferences =
    !! "tests/**/*AcceptanceTests.fsproj"

let dotnetcliVersion = "3.1.201"

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

let npxTool =
    match ProcessUtils.tryFindFileOnPath "npx.cmd" with
    | Some t -> t
    | None -> failwith "npx not found"

let runTool cmd args workingDir =
    let arguments = args |> String.split ' ' |> Arguments.OfArgs
    Command.RawCommand (cmd, arguments)
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory workingDir
    |> CreateProcess.ensureExitCode
    |> Proc.run
    |> ignore

// Targets

Target.create "Clean" (fun _ ->
    [ appReferences; serverReferences; unitTestReferences; acceptanceTestReferences ]
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
    [ appReferences; serverReferences; unitTestReferences; acceptanceTestReferences; fableReferences ]
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

Target.create "Build" (fun _ ->
    serverReferences
    |> Seq.iter (fun proj -> DotNet.build (withCustomParams "--no-dependencies --no-restore") proj)

    runTool npxTool "webpack-cli --config webpack.config.js -p" fableDirectory)

Target.create "Run" (fun _ ->
    let server = async {
        serverReferences
        |> Seq.iter (fun proj -> DotNet.exec dotnet (sprintf "watch --project %s run" proj) |> ignore)
    }
    let client = async {
        runTool npxTool "webpack-dev-server --config webpack.config.js" fableDirectory
    }

    [ server; client ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore)

// Build order

open Fake.Core.TargetOperators

"Clean"
    ==> "UpdateVersionNumber"
    ==> "Restore"
    ==> "BuildApp"
    ==> "RunUnitTests"
    ==> "RunAcceptanceTests"
    ==> "NpmInstall"

"NpmInstall" ==> "Build"
"NpmInstall" ==> "Run"

// start build
Target.runOrDefault "Build"