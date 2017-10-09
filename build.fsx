// include Fake libs
#r "packages/FAKE/tools/FakeLib.dll"

open System

open Fake
open Fake.FileUtils
open Fake.NpmHelper

// Filesets
let appReferences = 
    !! "**/*.csproj"
        ++ "**/*.fsproj"
        -- "**/*Tests.csproj"
        -- "**/*Tests.fsproj"
        -- "**/*Fable*.fsproj"

let fableDirectory = "FableCricket"
let fableReferences =   !! (fableDirectory + "/*.fsproj")
let fableReference = fableReferences |> Seq.exactlyOne

let unitTestReferences =
    !! "**/*UnitTests.csproj"
        ++ "**/*UnitTests.fsproj"

let acceptanceTestReferences =
    !! "**/*AcceptanceTests.csproj"
        ++ "**/*AcceptanceTests.fsproj"

let dotnetcliVersion = "2.0.0"
let mutable dotnetExePath = "dotnet"

// Targets
Target "InstallDotNetCore" (fun _ ->
    dotnetExePath <- DotNetCli.InstallDotNetSDK dotnetcliVersion
)

Target "Clean" (fun _ ->
    [ appReferences; unitTestReferences; acceptanceTestReferences ]
    |> Seq.concat
    |> Seq.iter (fun proj -> DotNetCli.RunCommand id ("clean " + proj))
)

Target "Restore" (fun _ ->
    [ appReferences; unitTestReferences; acceptanceTestReferences; fableReferences ]
    |> Seq.concat
    |> Seq.iter (fun proj -> DotNetCli.Restore (fun p -> { p with Project = proj; ToolPath = dotnetExePath }))
)

Target "BuildApp" (fun _ ->
    appReferences
    |> Seq.iter (fun proj -> DotNetCli.Build (fun p -> { p with Project = proj; ToolPath = dotnetExePath }))
)

Target "BuildTests" (fun _ ->
    [ unitTestReferences; acceptanceTestReferences ]
    |> Seq.concat
    |> Seq.iter (fun proj -> DotNetCli.Build (fun p -> { p with Project = proj; ToolPath = dotnetExePath }))
)

Target "RunUnitTests" (fun _ ->
    unitTestReferences
    |> Seq.iter (fun proj -> DotNetCli.Test (fun p -> { p with Project = proj; ToolPath = dotnetExePath }))
)

Target "RunAcceptanceTests" (fun _ ->
    acceptanceTestReferences
    |> Seq.iter (fun proj -> DotNetCli.Test (fun p -> { p with Project = proj; ToolPath = dotnetExePath }))
)

Target "NpmInstall" (fun _ ->
    Npm (fun p ->
        { p with Command = Install Standard; WorkingDirectory = fableDirectory })
)

Target "BuildFable" (fun _ ->
    fableReference
    |> (fun proj -> DotNetCli.RunCommand (fun p -> { p with WorkingDir = fableDirectory; ToolPath = dotnetExePath }) ("fable npm-build " + proj))
)

Target "RunFable" (fun _ ->
    fableReference
    |> (fun proj -> DotNetCli.RunCommand (fun p -> { p with WorkingDir = fableDirectory; ToolPath = dotnetExePath }) ("fable npm-start " + proj))
)

// Build order
"InstallDotNetCore"
    ==> "Clean"
    ==> "Restore"
    ==> "BuildApp"
    ==> "BuildTests"
    ==> "RunUnitTests"
    ==> "RunAcceptanceTests"
    ==> "NpmInstall"

"NpmInstall" ==> "BuildFable"
"NpmInstall" ==> "RunFable"

// start build
RunTargetOrDefault "BuildFable"