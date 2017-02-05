namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine
open TestHelpers

module ``MatchState not started tests`` =

    let state = NotStarted

    [<Test>]
    let ``starting the match creates a new ongoing match`` ()=
        state |> updater StartMatch |> matchStateCase |> should equal A_OngoingCase

    [<Test>]
    let ``abandoning the match creates an abandoned match`` ()=
        state |> updater AbandonMatch |> should equal Abandoned

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> state |> updater DrawMatch |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> state |> updater sampleUpdaterOngoing |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> state |> updater EnforceFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> state |> updater DeclineFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``starting the next innings throws an error`` ()=
        (fun () -> state |> updater StartNextInnings |> ignore) |> should throw typeof<System.Exception>
