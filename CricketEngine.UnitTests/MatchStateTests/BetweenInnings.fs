namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine
open TestHelpers

[<TestFixture>]
type ``MatchState between innings tests`` ()=

    // TODO tests for starting next innings

    static let a1 = createInnings 10 0
    static let b1 = createInnings 9 0
    static let a2 = createInnings 5 0
    static let b2 = createInnings 4 0
    static let stateCompletedA = A_Completed (a1)
    static let stateCompletedNoFollowOnAB = AB_CompletedNoFollowOn (a1, b1)
    static let stateCompletedPossibleFollowOnAB = AB_CompletedPossibleFollowOn (a1, b1)
    static let stateCompletedABA = ABA_Completed (a1, b1, a2)
    static let stateCompletedABB = ABB_Completed (a1, b1, b2)

    static member testData =
        [
            stateCompletedA;
            stateCompletedNoFollowOnAB;
            stateCompletedPossibleFollowOnAB;
            stateCompletedABA;
            stateCompletedABB          
        ]

    static member testDataNoFollowOn =
        [
            stateCompletedA;
            stateCompletedNoFollowOnAB;
            stateCompletedABA;
            stateCompletedABB          
        ]

    // start next innings cases

    [<Test>]
    member _x.``starting the next innings after one innings leaves the state as AB_Ongoing`` ()=
        stateCompletedA |> update StartNextInnings |> should equal (AB_Ongoing (a1, NewInnings))

    [<Test>]
    member _x.``starting the next innings after two innings leaves the state as ABA_Ongoing`` ()=
        stateCompletedNoFollowOnAB |> update StartNextInnings |> should equal (ABA_Ongoing (a1, b1, NewInnings))

    [<Test>]
    member _x.``starting the next innings after two innings with possible follow on throws an error`` ()=
        (fun () -> stateCompletedPossibleFollowOnAB |> update StartNextInnings |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    member _x.``starting the next innings after three innings leaves the state as ABAB_Ongoing`` ()=
        stateCompletedABA |> update StartNextInnings |> should equal (ABAB_Ongoing (a1, b1, a2, NewInnings))

    [<Test>]
    member _x.``starting the next innings after three innings leaves the state as ABBA_Ongoing`` ()=
        stateCompletedABB |> update StartNextInnings |> should equal (ABBA_Ongoing (a1, b1, b2, NewInnings))

    // draw cases

    [<Test>]
    member _x.``drawing the match after one innings creates a drawn match`` ()=
        stateCompletedA |> update DrawMatch |> should equal (A_MatchDrawn (a1))

    [<Test>]
    member _x.``drawing the match after two innings creates a drawn match`` ()=
        stateCompletedNoFollowOnAB |> update DrawMatch |> should equal (AB_MatchDrawn (a1, b1))

    [<Test>]
    member _x.``drawing the match after two innings with possible follow on creates a drawn match`` ()=
       stateCompletedPossibleFollowOnAB |> update DrawMatch |> should equal (AB_MatchDrawn (a1, b1))

    [<Test>]
    member _x.``drawing the match after three innings creates a drawn match`` ()=
        stateCompletedABA |> update DrawMatch |> should equal (ABA_MatchDrawn (a1, b1, a2))

    [<Test>]
    member _x.``drawing the match after three innings with follow on creates a drawn match`` ()=
        stateCompletedABB |> update DrawMatch |> should equal (ABB_MatchDrawn (a1, b1, b2))

    // follow-on cases

    [<Test>]
    member _x.``enforcing the follow-on leaves the state as ABB_Ongoing`` ()=
        stateCompletedPossibleFollowOnAB |> update EnforceFollowOn |> should equal (ABB_Ongoing (a1, b1, NewInnings))

    [<Test>]
    member _x.``declining the follow-on leaves the state as ABA_Ongoing`` ()=
        stateCompletedPossibleFollowOnAB |> update DeclineFollowOn |>  should equal (ABA_Ongoing (a1, b1, NewInnings))

    // error cases

    [<TestCaseSource("testData")>]
    member _x.``starting the match throws an error`` state =
        (fun () -> state |> update StartMatch |> ignore) |> should throw typeof<System.Exception>

    [<TestCaseSource("testData")>]
    member _x.``abandoning the match throws an error`` state =
        (fun () -> state |> update AbandonMatch |> ignore) |> should throw typeof<System.Exception>
        
    [<TestCaseSource("testData")>]
    member _x.``updating the current innings throws an error`` state =
        (fun () -> state |> update sampleUpdaterOngoing |> ignore) |> should throw typeof<System.Exception>

    [<TestCaseSource("testDataNoFollowOn")>]
    member _x.``enforcing the follow-on throws an error`` state =
        (fun () -> state |> update EnforceFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<TestCaseSource("testDataNoFollowOn")>]
    member _x.``declining the follow-on throws an error`` state =
        (fun () -> state |> update DeclineFollowOn |> ignore) |> should throw typeof<System.Exception>
