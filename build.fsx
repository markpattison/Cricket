// include Fake libs
#r "packages/FAKE/tools/FakeLib.dll"

open System

open Fake
open Fake.FileUtils
open Fake.Testing.NUnit3

// Directories
let buildDir  = "./build/"
let testDir   = "./test/"
let clientBuildDir  = "./client/out/"
let bundleDir = "./client/public/"

// NPM helpers
let npm command args workingDir =
  let args = sprintf "%s %s" command (String.concat " " args)
  let cmd, args = if EnvironmentHelper.isUnix then "npm", args else "cmd", ("/C npm " + args)
  let ok =
    execProcess (fun info ->
      info.FileName <- cmd
      info.WorkingDirectory <- workingDir
      info.Arguments <- args) TimeSpan.MaxValue
  if not ok then failwith (sprintf "'%s %s' task failed" cmd args)

let node command args workingDir =
  let args = sprintf "%s %s" command (String.concat " " args)
  let cmd, args = if EnvironmentHelper.isUnix then "node", args else "cmd", ("/C node " + args)
  let ok =
    execProcess (fun info ->
      info.FileName <- cmd
      info.WorkingDirectory <- workingDir
      info.Arguments <- args) TimeSpan.MaxValue
  if not ok then failwith (sprintf "'%s %s' task failed" cmd args)

// Filesets
let appReferences = 
    !! "**/*.csproj"
        ++ "**/*.fsproj"
        -- "**/*Tests.csproj"
        -- "**/*Tests.fsproj"
        -- "**/*Client.fsproj"

let testReferences =
    !! "**/*Tests.csproj"
        ++ "**/*Tests.fsproj"

// Targets
Target "Clean" (fun _ -> 
    CleanDirs [buildDir; testDir; clientBuildDir; bundleDir]
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

Target "CopyFiles" (fun _ ->
    cp "./client/index.html" bundleDir
    cp_r "./client/css" bundleDir
)

Target "Fable" (fun _ ->
   npm "install" [] "./client"
   node "node_modules/fable-compiler" [ ] "./client"
)

// Build order
"Clean"
    ==> "BuildApp"
    ==> "BuildTests"
    ==> "UnitTests"
    ==> "AcceptanceTests"
    ==> "CopyFiles"
    ==> "Fable"

// start build
RunTargetOrDefault "AcceptanceTests"
