// include Fake libs
#r "packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing.NUnit3

// Directories
let buildDir  = "./build/"
let testDir   = "./test/"
let deployDir = "./deploy/"

// Filesets
let appReferences = 
    !! "**/*.csproj"
        ++ "**/*.fsproj"
        -- "**/*Tests.csproj"
        -- "**/*Tests.fsproj"

let testReferences =
    !! "**/*Tests.csproj"
        ++ "**/*Tests.fsproj"

// Targets
Target "Clean" (fun _ -> 
    CleanDirs [buildDir; testDir; deployDir]
)

Target "BuildApp" (fun _ ->
    appReferences
        |> MSBuildRelease buildDir "Build"
        |> Log "AppBuild-Output: "
)

Target "BuildTests" (fun _ ->
    testReferences
        |> MSBuildDebug testDir "Build"
        |> Log "TestBuild-Output: "
)

Target "UnitTests" (fun _ ->
    !! (testDir + "/*UnitTests.dll")
        |> NUnit3 (fun arg -> { arg with ResultSpecs = [testDir </> "UnitTestResults.xml"] })
)

Target "AcceptanceTests" (fun _ ->
    !! (testDir + "/*AcceptanceTests.dll")
        |> NUnit3 (fun arg -> { arg with ResultSpecs = [testDir </> "AcceptanceTestResults.xml"] })
)

// Build order
"Clean"
    ==> "BuildApp"
    ==> "BuildTests"
    ==> "UnitTests"
    ==> "AcceptanceTests"

// start build
RunTargetOrDefault "AcceptanceTests"
