namespace Cricket.CricketEngine.UnitTests

open NUnit.Framework
open FsUnit

open Cricket.CricketEngine
open Cricket.CricketEngine.MatchState
open TestHelpers

[<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "MemberNamesMustBePascalCase")>]
[<TestFixture>]
type ``MatchState between innings tests`` ()=

    static let a1 = createInnings 10 0
    static let b1 = createInnings 9 0
    static let a2 = createInnings 5 0
    static let b2 = createInnings 4 0
    static let stateCompletedA = A'Completed (a1)
    static let stateCompletedNoFollowOnAB = AB'CompletedNoFollowOn (a1, b1)
    static let stateCompletedPossibleFollowOnAB = AB'CompletedPossibleFollowOn (a1, b1)
    static let stateCompletedABA = ABA'Completed (a1, b1, a2)
    static let stateCompletedABB = ABB'Completed (a1, b1, b2)

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
        stateCompletedA |> updater StartNextInnings |> should equal (AB'Ongoing (a1, Innings.create))

    [<Test>]
    member _x.``starting the next innings after two innings leaves the state as ABA_Ongoing`` ()=
        stateCompletedNoFollowOnAB |> updater StartNextInnings |> should equal (ABA'Ongoing (a1, b1, Innings.create))

    [<Test>]
    member _x.``starting the next innings after two innings with possible follow on throws an error`` ()=
        (fun () -> stateCompletedPossibleFollowOnAB |> updater StartNextInnings |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    member _x.``starting the next innings after three innings leaves the state as ABAB_Ongoing`` ()=
        stateCompletedABA |> updater StartNextInnings |> should equal (ABAB'Ongoing (a1, b1, a2, Innings.create))

    [<Test>]
    member _x.``starting the next innings after three innings leaves the state as ABBA_Ongoing`` ()=
        stateCompletedABB |> updater StartNextInnings |> should equal (ABBA'Ongoing (a1, b1, b2, Innings.create))

    // draw cases

    [<Test>]
    member _x.``drawing the match after one innings creates a drawn match`` ()=
        stateCompletedA |> updater DrawMatch |> should equal (A'MatchDrawn (a1))

    [<Test>]
    member _x.``drawing the match after two innings creates a drawn match`` ()=
        stateCompletedNoFollowOnAB |> updater DrawMatch |> should equal (AB'MatchDrawn (a1, b1))

    [<Test>]
    member _x.``drawing the match after two innings with possible follow on creates a drawn match`` ()=
       stateCompletedPossibleFollowOnAB |> updater DrawMatch |> should equal (AB'MatchDrawn (a1, b1))

    [<Test>]
    member _x.``drawing the match after three innings creates a drawn match`` ()=
        stateCompletedABA |> updater DrawMatch |> should equal (ABA'MatchDrawn (a1, b1, a2))

    [<Test>]
    member _x.``drawing the match after three innings with follow on creates a drawn match`` ()=
        stateCompletedABB |> updater DrawMatch |> should equal (ABB'MatchDrawn (a1, b1, b2))

    // follow-on cases

    [<Test>]
    member _x.``enforcing the follow-on leaves the state as ABB_Ongoing`` ()=
        stateCompletedPossibleFollowOnAB |> updater EnforceFollowOn |> should equal (ABB'Ongoing (a1, b1, Innings.create))

    [<Test>]
    member _x.``declining the follow-on leaves the state as ABA_Ongoing`` ()=
        stateCompletedPossibleFollowOnAB |> updater DeclineFollowOn |>  should equal (ABA'Ongoing (a1, b1, Innings.create))

    // error cases

    [<TestCaseSource("testData")>]
    member _x.``starting the match throws an error`` state =
        (fun () -> state |> updater StartMatch |> ignore) |> should throw typeof<System.Exception>

    [<TestCaseSource("testData")>]
    member _x.``abandoning the match throws an error`` state =
        (fun () -> state |> updater AbandonMatch |> ignore) |> should throw typeof<System.Exception>
        
    [<TestCaseSource("testData")>]
    member _x.``updating the current innings throws an error`` state =
        (fun () -> state |> updater sampleUpdaterOngoing |> ignore) |> should throw typeof<System.Exception>

    [<TestCaseSource("testDataNoFollowOn")>]
    member _x.``enforcing the follow-on throws an error`` state =
        (fun () -> state |> updater EnforceFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<TestCaseSource("testDataNoFollowOn")>]
    member _x.``declining the follow-on throws an error`` state =
        (fun () -> state |> updater DeclineFollowOn |> ignore) |> should throw typeof<System.Exception>
