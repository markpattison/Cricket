// include Fake libs
#r "packages/FAKE/tools/FakeLib.dll"

open System

open Fake
open Fake.FileUtils
open Fake.YarnHelper

// Directories

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

// Targets
Target "Clean" (fun _ ->
    [ appReferences; unitTestReferences; acceptanceTestReferences ]
    |> Seq.concat
    |> Seq.iter (fun proj -> DotNetCli.RunCommand id ("clean " + proj))
)

Target "Restore" (fun _ ->
    [ appReferences; unitTestReferences; acceptanceTestReferences; fableReferences ]
    |> Seq.concat
    |> Seq.iter (fun proj -> DotNetCli.Restore (fun p -> { p with Project = proj }))
)

Target "BuildApp" (fun _ ->
    appReferences
    |> Seq.iter (fun proj -> DotNetCli.Build (fun p -> { p with Project = proj }))
)

Target "BuildTests" (fun _ ->
    [ unitTestReferences; acceptanceTestReferences ]
    |> Seq.concat
    |> Seq.iter (fun proj -> DotNetCli.Build (fun p -> { p with Project = proj }))
)

Target "RunUnitTests" (fun _ ->
    unitTestReferences
    |> Seq.iter (fun proj -> DotNetCli.Test (fun p -> { p with Project = proj }))
)

Target "RunAcceptanceTests" (fun _ ->
    acceptanceTestReferences
    |> Seq.iter (fun proj -> DotNetCli.Test (fun p -> { p with Project = proj }))
)

Target "YarnInstall" (fun _ ->
    Yarn (fun p ->
        { p with Command = Install Standard; WorkingDirectory = fableDirectory })
)

Target "BuildFable" (fun _ ->
    fableReference
    |> (fun proj -> DotNetCli.RunCommand (fun p -> { p with WorkingDir = fableDirectory }) ("fable yarn-build " + proj))
)

Target "RunFable" (fun _ ->
    fableReference
    |> (fun proj -> DotNetCli.RunCommand (fun p -> { p with WorkingDir = fableDirectory }) ("fable yarn-start " + proj))
)

// Build order
"Clean"
    ==> "Restore"
    ==> "BuildApp"
    ==> "BuildTests"
    ==> "RunUnitTests"
    ==> "RunAcceptanceTests"
    ==> "YarnInstall"

"YarnInstall" ==> "BuildFable"
"YarnInstall" ==> "RunFable"

// start build
RunTargetOrDefault "BuildFable"