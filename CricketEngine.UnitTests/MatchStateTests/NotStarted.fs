namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine
open TestHelpers

module ``MatchState NotStarted tests`` =

    let state = NotStarted
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match creates a new ongoing match`` ()=
        state |> update StartMatch |> matchStateCase |> should equal A_OngoingCase

    [<Test>]
    let ``abandoning the match creates an abandoned match`` ()=
        state |> update AbandonMatch |> should equal Abandoned

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> state |> update DrawMatch |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> state |> update sampleUpdaterOngoing |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> state |> update EnforceFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> state |> update DeclineFollowOn |> ignore) |> should throw typeof<System.Exception>
