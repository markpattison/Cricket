namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine
open TestHelpers

module ``MatchState A_Ongoing tests`` =

    let a1 = createInnings 5 0
    let state = A'Ongoing (a1)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> state |> updater StartMatch |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> state |> updater AbandonMatch |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> state |> updater EnforceFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> state |> updater DeclineFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        state |> updater DrawMatch |> should equal (A'MatchDrawn (a1))
    
    [<Test>]
    let ``updating the innings as ongoing should leave the state as A'Ongoing`` ()=
        state |> updater sampleUpdaterOngoing |> matchStateCase |> should equal A'OngoingCase

    [<Test>]
    let ``updating the innings as completed should leave the state as A'Completed`` ()=
        state |> updater sampleUpdaterCompleted |> matchStateCase |> should equal A'CompletedCase

    [<Test>]
    let ``starting the next innings throws an error`` ()=
        (fun () -> state |> updater StartNextInnings |> ignore) |> should throw typeof<System.Exception>

module ``MatchState AB_Ongoing tests`` =

    let a1 = createInnings 500 0
    let b1 = createInnings 300 9
    let state = AB'Ongoing (a1, b1)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> state |> updater StartMatch |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> state |> updater AbandonMatch |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> state |> updater EnforceFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> state |> updater DeclineFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        state |> updater DrawMatch |> should equal (AB'MatchDrawn (a1, b1))
        
    [<Test>]
    let ``updating the innings as ongoing should leave the state as AB'Ongoing`` ()=
        state |> updater (UpdateInnings (UpdateForBall DotBall)) |> matchStateCase |> should equal AB'OngoingCase

    [<Test>]
    let ``updating the innings as completed with B ahead of follow-on target should leave the state as AB'CompletedNoFollowOn`` ()=
        state |> updater (UpdateInnings (UpdateForBall (RunOutStriker (1, false))))  |> matchStateCase |> should equal AB'CompletedNoFollowOnCase

    let sampleUpdaterCompletedPossibleFollowOn = UpdateInnings (UpdateForBall LBW)

    [<Test>]
    let ``updating the innings as completed with B behind follow-on target should leave the state as AB'CompletedPossibleFollowOn`` ()=
        state |> updater (UpdateInnings (UpdateForBall LBW))  |> matchStateCase |> should equal AB'CompletedPossibleFollowOnCase

    [<Test>]
    let ``starting the next innings throws an error`` ()=
        (fun () -> state |> updater StartNextInnings |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABA_Ongoing tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let a2 = createInnings 3 9
    let state = ABA'Ongoing (a1, b1, a2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> state |> updater StartMatch |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> state |> updater AbandonMatch |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> state |> updater EnforceFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> state |> updater DeclineFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        state |> updater DrawMatch |> should equal (ABA'MatchDrawn (a1, b1, a2))
        
    [<Test>]
    let ``updating the innings as ongoing should leave the state as ABA'Ongoing`` ()=
        state |> updater sampleUpdaterOngoing |> matchStateCase |> should equal ABA'OngoingCase

    [<Test>]
    let ``updating the innings as completed with A not behind should leave the state as ABA'Completed`` ()=
        state |> updater (UpdateInnings (UpdateForBall (RunOutStriker (3, false))))  |> matchStateCase |> should equal ABA'CompletedCase

    [<Test>]
    let ``updating the innings as completed with A behind should leave the state as ABA'VictoryB`` ()=
        state |> updater (UpdateInnings (UpdateForBall (RunOutStriker (0, false))))  |> matchStateCase |> should equal ABA'VictoryBCase

    [<Test>]
    let ``starting the next innings throws an error`` ()=
        (fun () -> state |> updater StartNextInnings |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABB_Ongoing tests`` =

    let a1 = createInnings 10 0
    let b1 = createInnings 5 0
    let b2 = createInnings 3 9
    let state = ABB'Ongoing (a1, b1, b2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> state |> updater StartMatch |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> state |> updater AbandonMatch |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> state |> updater EnforceFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> state |> updater DeclineFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        state |> updater DrawMatch |> should equal (ABB'MatchDrawn (a1, b1, b2))
        
    [<Test>]
    let ``updating the innings as ongoing should leave the state as ABB'Ongoing`` ()=
        state |> updater sampleUpdaterOngoing |> matchStateCase |> should equal ABB'OngoingCase

    [<Test>]
    let ``updating the innings as completed with B not behind should leave the state as ABB'Completed`` ()=
        state |> updater (UpdateInnings (UpdateForBall (RunOutStriker (2, false)))) |> matchStateCase |> should equal ABB'CompletedCase

    [<Test>]
    let ``updating the innings as completed with B behind should leave the state as ABB'VictoryA`` ()=
        state |> updater (UpdateInnings (UpdateForBall (RunOutStriker (0, false)))) |> matchStateCase |> should equal ABB'VictoryACase

    [<Test>]
    let ``starting the next innings throws an error`` ()=
        (fun () -> state |> updater StartNextInnings |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABAB_Ongoing tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let a2 = createInnings 8 0
    let b2 = createInnings 2 9
    let state = ABAB'Ongoing (a1, b1, a2, b2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> state |> updater StartMatch |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> state |> updater AbandonMatch |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> state |> updater EnforceFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> state |> updater DeclineFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        state |> updater DrawMatch |> should equal (ABAB'MatchDrawn (a1, b1, a2, b2))
        
    [<Test>]
    let ``updating the innings as ongoing with B not ahead should leave the state as ABAB'Ongoing`` ()=
        state |> updater (UpdateInnings (UpdateForBall DotBall)) |> matchStateCase |> should equal ABAB'OngoingCase

    [<Test>]
    let ``updating the innings as ongoing with B ahead should leave the state as ABAB'VictoryB`` ()=
        state |> updater (UpdateInnings (UpdateForBall Four)) |> matchStateCase |> should equal ABAB'VictoryBCase

    [<Test>]
    let ``updating the innings as ongoing (run out after victory) with B ahead should leave the state as ABAB'VictoryB`` ()=
        state |> updater (UpdateInnings (UpdateForBall (RunOutStriker (4, false)))) |> matchStateCase |> should equal ABAB'VictoryBCase

    [<Test>]
    let ``updating the innings as completed with B behind should leave the state as ABAB'VictoryA`` ()=
        state |> updater (UpdateInnings (UpdateForBall (RunOutStriker (0, false)))) |> matchStateCase |> should equal ABAB'VictoryACase

    [<Test>]
    let ``updating the innings as completed with B level should leave the state as ABAB'MatchTied`` ()=
        state |> updater (UpdateInnings (UpdateForBall (RunOutStriker (1, false)))) |> matchStateCase |> should equal ABAB'MatchTiedCase

    [<Test>]
    let ``updating the innings as completed with B ahead throws an error`` ()=
        (fun () -> state |> updater (UpdateInnings (UpdateForBall Four)) |> updater (UpdateInnings (UpdateForBall (RunOutStriker (0, false)))) |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``starting the next innings throws an error`` ()=
        (fun () -> state |> updater StartNextInnings |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABBA_Ongoing tests`` =

    let a1 = createInnings 15 0
    let b1 = createInnings 10 0
    let b2 = createInnings 8 0
    let a2 = createInnings 1 9
    let state = ABBA'Ongoing (a1, b1, b2, a2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> state |> updater StartMatch |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> state |> updater AbandonMatch |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> state |> updater EnforceFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> state |> updater DeclineFollowOn |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        state |> updater DrawMatch |> should equal (ABBA'MatchDrawn (a1, b1, b2, a2))
        
    [<Test>]
    let ``updating the innings as ongoing with A not ahead should leave the state as ABBA'Ongoing`` ()=
        state |> updater (UpdateInnings (UpdateForBall DotBall)) |> matchStateCase |> should equal ABBA'OngoingCase

    [<Test>]
    let ``updating the innings as ongoing with A ahead should leave the state as ABBA'VictoryA`` ()=
        state |> updater (UpdateInnings (UpdateForBall Four)) |> matchStateCase |> should equal ABBA'VictoryACase

    [<Test>]
    let ``updating the innings as ongoing (run out after victory) with A ahead should leave the state as ABBA'VictoryA`` ()=
        state |> updater (UpdateInnings (UpdateForBall (RunOutStriker (4, false)))) |> matchStateCase |> should equal ABBA'VictoryACase

    [<Test>]
    let ``updating the innings as completed with A behind should leave the state as ABBA'VictoryB`` ()=
        state |> updater (UpdateInnings (UpdateForBall (RunOutStriker (0, false)))) |> matchStateCase |> should equal ABBA'VictoryBCase

    [<Test>]
    let ``updating the innings as completed with A level should leave the state as ABBA'MatchTied`` ()=
        state |> updater (UpdateInnings (UpdateForBall (RunOutStriker (2, false)))) |> matchStateCase |> should equal ABBA'MatchTiedCase

    [<Test>]
    let ``updating the innings as completed with A ahead throws an error`` ()=
        (fun () -> state |> updater (UpdateInnings (UpdateForBall Four)) |> updater (UpdateInnings (UpdateForBall (RunOutStriker (0, false)))) |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``starting the next innings throws an error`` ()=
        (fun () -> state |> updater StartNextInnings |> ignore) |> should throw typeof<System.Exception>
