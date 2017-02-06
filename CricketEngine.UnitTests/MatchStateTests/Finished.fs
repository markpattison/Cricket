namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine
open TestHelpers

[<TestFixture>]
type ``MatchState finished tests`` ()=
    
    static member testData =
        [
            Abandoned;
            A'MatchDrawn (createInnings 5 0);
            AB'MatchDrawn (createInnings 5 0, createInnings 10 0);
            ABA'VictoryB (createInnings 5 0, createInnings 10 0, createInnings 3 10);
            ABA'MatchDrawn (createInnings 5 0, createInnings 10 0, createInnings 3 0);
            ABB'VictoryA (createInnings 5 0, createInnings 10 0, createInnings 3 10);
            ABB'MatchDrawn (createInnings 5 0, createInnings 10 0, createInnings 3 0);
            ABAB'VictoryA (createInnings 5 0, createInnings 10 0, createInnings 8 0, createInnings 2 10);
            ABAB'VictoryB (createInnings 5 0, createInnings 10 0, createInnings 8 0, createInnings 4 0);
            ABAB'MatchDrawn (createInnings 5 0, createInnings 10 0, createInnings 8 0, createInnings 2 0);
            ABAB'MatchTied (createInnings 5 0, createInnings 10 0, createInnings 8 0, createInnings 3 10);
            ABBA'VictoryA (createInnings 15 0, createInnings 10 0, createInnings 7 0, createInnings 3 0);
            ABBA'VictoryB (createInnings 15 0, createInnings 10 0, createInnings 7 0, createInnings 1 10);
            ABBA'MatchDrawn (createInnings 15 0, createInnings 10 0, createInnings 7 0, createInnings 1 0);
            ABBA'MatchTied (createInnings 15 0, createInnings 10 0, createInnings 7 0, createInnings 2 10);
        ]

    [<TestCaseSource("testData")>]
    member _x.``starting the match throws an error`` state =
        (fun () -> state |> updater StartMatch |> ignore) |> should throw typeof<System.Exception>

    [<TestCaseSource("testData")>]
    member _x.``abandoning the match throws an error`` state =
        (fun () -> state |> updater AbandonMatch |> ignore) |> should throw typeof<System.Exception>

    [<TestCaseSource("testData")>]
    member _x.``drawing the match throws an error`` state =
        (fun () -> state |> updater DrawMatch |> ignore) |> should throw typeof<System.Exception>
        
    [<TestCaseSource("testData")>]
    member _x.``updating the current innings throws an error`` state =
        (fun () -> state |> updater sampleUpdaterOngoing |> ignore) |> should throw typeof<System.Exception>

    [<TestCaseSource("testData")>]
    member _x.``enforcing the follow-on throws an error`` state =
        (fun () -> state |> updater EnforceFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<TestCaseSource("testData")>]
    member _x.``declining the follow-on throws an error`` state =
        (fun () -> state |> updater DeclineFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<TestCaseSource("testData")>]
    member _x.``starting the next innings throws an error`` state =
        (fun () -> state |> updater StartNextInnings |> ignore) |> should throw typeof<System.Exception>
