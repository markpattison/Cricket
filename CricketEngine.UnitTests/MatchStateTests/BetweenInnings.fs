namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine
open TestHelpers

[<TestFixture>]
type ``Between innings MatchState tests`` ()=

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

    [<Test>]
    member _x.``enforcing the follow-on leaves the state as ABB_Ongoing`` ()=
        stateCompletedPossibleFollowOnAB |> update EnforceFollowOn |> matchStateCase |> should equal ABB_OngoingCase 

    [<Test>]
    member _x.``declining the follow-on leaves the state as ABA_Ongoing`` ()=
        stateCompletedPossibleFollowOnAB |> update DeclineFollowOn |> matchStateCase |> should equal ABA_OngoingCase 

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
