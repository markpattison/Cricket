namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine
open TestHelpers

module ``MatchState A_Ongoing tests`` =

    let a1 = createInnings 5 0
    let state = A_Ongoing (a1)
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
        state |> updater DrawMatch |> should equal (A_MatchDrawn (a1))
    
    [<Test>]
    let ``updating the innings as ongoing should leave the state as A_Ongoing`` ()=
        state |> updater sampleUpdaterOngoing |> matchStateCase |> should equal A_OngoingCase

    [<Test>]
    let ``updating the innings as ongoing should use the result of the innings update`` ()=
        state |> updater sampleUpdaterOngoing |> should equal (A_Ongoing (sampleOngoingInnings))

    [<Test>]
    let ``updating the innings as completed should leave the state as A_Completed`` ()=
        state |> updater sampleUpdaterCompleted |> matchStateCase |> should equal A_CompletedCase

    [<Test>]
    let ``updating the innings as completed should use the result of the innings update`` ()=
        state |> updater sampleUpdaterCompleted |> should equal (A_Completed (sampleCompletedInnings))

    [<Test>]
    let ``starting the next innings throws an error`` ()=
        (fun () -> state |> updater StartNextInnings |> ignore) |> should throw typeof<System.Exception>

module ``MatchState AB_Ongoing tests`` =

    let a1 = createInnings 500 0
    let b1 = createInnings 10 0
    let state = AB_Ongoing (a1, b1)
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
        state |> updater DrawMatch |> should equal (AB_MatchDrawn (a1, b1))
        
    [<Test>]
    let ``updating the innings as ongoing should leave the state as AB_Ongoing`` ()=
        state |> updater sampleUpdaterOngoing |> matchStateCase |> should equal AB_OngoingCase

    [<Test>]
    let ``updating the innings as ongoing should use the result of the innings update`` ()=
        state |> updater sampleUpdaterOngoing |> should equal (AB_Ongoing (a1, sampleOngoingInnings))

    let sampleCompletedInningsNoFollowOn = createInnings 301 10
    let sampleUpdaterCompletedNoFollowOn = UpdateInnings (fun _ -> sampleCompletedInningsNoFollowOn)

    [<Test>]
    let ``updating the innings as completed with B on follow-on target should leave the state as AB_CompletedNoFollowOn`` ()=
        state |> updater sampleUpdaterCompletedNoFollowOn  |> matchStateCase |> should equal AB_CompletedNoFollowOnCase

    [<Test>]
    let ``updating the innings as completed with B on follow-on target should use the result of the innings update`` ()=
        state |> updater sampleUpdaterCompletedNoFollowOn  |> should equal (AB_CompletedNoFollowOn (a1, sampleCompletedInningsNoFollowOn))

    let sampleCompletedInningsPossibleFollowOn = createInnings 300 10
    let sampleUpdaterCompletedPossibleFollowOn = UpdateInnings (fun _ -> sampleCompletedInningsPossibleFollowOn)

    [<Test>]
    let ``updating the innings as completed with B behind follow-on target should leave the state as AB_CompletedPossibleFollowOn`` ()=
        state |> updater sampleUpdaterCompletedPossibleFollowOn  |> matchStateCase |> should equal AB_CompletedPossibleFollowOnCase

    [<Test>]
    let ``updating the innings as completed with B behind follow-on target should use the result of the innings update`` ()=
        state |> updater sampleUpdaterCompletedPossibleFollowOn  |> should equal (AB_CompletedPossibleFollowOn (a1, sampleCompletedInningsPossibleFollowOn))

    [<Test>]
    let ``starting the next innings throws an error`` ()=
        (fun () -> state |> updater StartNextInnings |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABA_Ongoing tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let a2 = createInnings 3 0
    let state = ABA_Ongoing (a1, b1, a2)
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
        state |> updater DrawMatch |> should equal (ABA_MatchDrawn (a1, b1, a2))
        
    [<Test>]
    let ``updating the innings as ongoing should leave the state as ABA_Ongoing`` ()=
        state |> updater sampleUpdaterOngoing |> matchStateCase |> should equal ABA_OngoingCase

    [<Test>]
    let ``updating the innings as ongoing should use the result of the innings update`` ()=
        state |> updater sampleUpdaterOngoing |> should equal (ABA_Ongoing (a1, b1, sampleOngoingInnings))

    let sampleCompletedInningsNoVictory = createInnings 5 10
    let sampleUpdaterCompletedNoVictory = UpdateInnings (fun _ -> sampleCompletedInningsNoVictory)

    [<Test>]
    let ``updating the innings as completed with A not behind should leave the state as ABA_Completed`` ()=
        state |> updater sampleUpdaterCompletedNoVictory  |> matchStateCase |> should equal ABA_CompletedCase

    [<Test>]
    let ``updating the innings as completed with A not behindshould use the result of the innings update`` ()=
        state |> updater sampleUpdaterCompletedNoVictory  |> should equal (ABA_Completed (a1, b1, sampleCompletedInningsNoVictory))

    let sampleCompletedInningsVictory = createInnings 4 10
    let sampleUpdaterCompletedVictory = UpdateInnings (fun _ -> sampleCompletedInningsVictory)

    [<Test>]
    let ``updating the innings as completed with A behind should leave the state as ABA_VictoryB`` ()=
        state |> updater sampleUpdaterCompletedVictory  |> matchStateCase |> should equal ABA_VictoryBCase

    [<Test>]
    let ``updating the innings as completed with A behind should use the result of the innings update`` ()=
        state |> updater sampleUpdaterCompletedVictory  |> should equal (ABA_VictoryB (a1, b1, sampleCompletedInningsVictory))

    [<Test>]
    let ``starting the next innings throws an error`` ()=
        (fun () -> state |> updater StartNextInnings |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABB_Ongoing tests`` =

    let a1 = createInnings 10 0
    let b1 = createInnings 5 0
    let b2 = createInnings 3 0
    let state = ABB_Ongoing (a1, b1, b2)
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
        state |> updater DrawMatch |> should equal (ABB_MatchDrawn (a1, b1, b2))
        
    [<Test>]
    let ``updating the innings as ongoing should leave the state as ABB_Ongoing`` ()=
        state |> updater sampleUpdaterOngoing |> matchStateCase |> should equal ABB_OngoingCase

    [<Test>]
    let ``updating the innings as ongoing should use the result of the innings update`` ()=
        state |> updater sampleUpdaterOngoing |> should equal (ABB_Ongoing (a1, b1, sampleOngoingInnings))

    let sampleCompletedInningsNoVictory = createInnings 5 10
    let sampleUpdaterCompletedNoVictory = UpdateInnings (fun _ -> sampleCompletedInningsNoVictory)

    [<Test>]
    let ``updating the innings as completed with B not behind should leave the state as ABB_Completed`` ()=
        state |> updater sampleUpdaterCompletedNoVictory |> matchStateCase |> should equal ABB_CompletedCase

    [<Test>]
    let ``updating the innings as completed with B not behind should use the result of the innings update`` ()=
        state |> updater sampleUpdaterCompletedNoVictory |> should equal (ABB_Completed (a1, b1, sampleCompletedInningsNoVictory))

    let sampleCompletedInningsVictory = createInnings 4 10
    let sampleUpdaterCompletedVictory = UpdateInnings (fun _ -> sampleCompletedInningsVictory)

    [<Test>]
    let ``updating the innings as completed with B behind should leave the state as ABB_VictoryA`` ()=
        state |> updater sampleUpdaterCompletedVictory |> matchStateCase |> should equal ABB_VictoryACase

    [<Test>]
    let ``updating the innings as completed with B behind should use the result of the innings update`` ()=
        state |> updater sampleUpdaterCompletedVictory |> should equal (ABB_VictoryA (a1, b1, sampleCompletedInningsVictory))

    [<Test>]
    let ``starting the next innings throws an error`` ()=
        (fun () -> state |> updater StartNextInnings |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABAB_Ongoing tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let a2 = createInnings 8 0
    let b2 = createInnings 2 0
    let state = ABAB_Ongoing (a1, b1, a2, b2)
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
        state |> updater DrawMatch |> should equal (ABAB_MatchDrawn (a1, b1, a2, b2))
    
    let sampleMatchOngoingInnings = createInnings 3 5
    let sampleUpdaterMatchOngoing = UpdateInnings (fun _ -> sampleMatchOngoingInnings)
        
    [<Test>]
    let ``updating the innings as ongoing with B not ahead should leave the state as ABAB_Ongoing`` ()=
        state |> updater sampleUpdaterMatchOngoing |> matchStateCase |> should equal ABAB_OngoingCase

    [<Test>]
    let ``updating the innings as ongoing with B not ahead should use the result of the innings update`` ()=
        state |> updater sampleUpdaterMatchOngoing |> should equal (ABAB_Ongoing (a1, b1, a2, sampleMatchOngoingInnings))

    let sampleOngoingInningsWicketsVictory = createInnings 5 7
    let sampleUpdaterOngoingWicketsVictory = UpdateInnings (fun _ -> sampleOngoingInningsWicketsVictory)

    [<Test>]
    let ``updating the innings as ongoing with B ahead should leave the state as ABAB_VictoryB`` ()=
        state |> updater sampleUpdaterOngoingWicketsVictory |> matchStateCase |> should equal ABAB_VictoryBCase

    [<Test>]
    let ``updating the innings as ongoing with B ahead should use the result of the innings update`` ()=
        state |> updater sampleUpdaterOngoingWicketsVictory |> should equal (ABAB_VictoryB (a1, b1, a2, sampleOngoingInningsWicketsVictory))

    let sampleCompletedInningsRunsVictory = createInnings 2 10
    let sampleUpdaterCompletedRunsVictory = UpdateInnings (fun _ -> sampleCompletedInningsRunsVictory)

    [<Test>]
    let ``updating the innings as completed with B behind should leave the state as ABAB_VictoryA`` ()=
        state |> updater sampleUpdaterCompletedRunsVictory |> matchStateCase |> should equal ABAB_VictoryACase

    [<Test>]
    let ``updating the innings as completed with B behind should use the result of the innings update`` ()=
        state |> updater sampleUpdaterCompletedRunsVictory |> should equal (ABAB_VictoryA (a1, b1, a2, sampleCompletedInningsRunsVictory))
    
    let sampleCompletedInningsTie = createInnings 3 10
    let sampleUpdaterCompletedTie = UpdateInnings (fun _ -> sampleCompletedInningsTie)

    [<Test>]
    let ``updating the innings as completed with B level should leave the state as ABAB_MatchTied`` ()=
        state |> updater sampleUpdaterCompletedTie |> matchStateCase |> should equal ABAB_MatchTiedCase

    [<Test>]
    let ``updating the innings as completed with B level should use the result of the innings update`` ()=
        state |> updater sampleUpdaterCompletedTie |> should equal (ABAB_MatchTied (a1, b1, a2, sampleCompletedInningsTie))

    let sampleCompletedInningsInconsistent = createInnings 4 10
    let sampleUpdaterCompletedInconsistent = UpdateInnings (fun _ -> sampleCompletedInningsInconsistent)

    [<Test>]
    let ``updating the innings as completed with B ahead throws an error`` ()=
        (fun () -> state |> updater sampleUpdaterCompletedInconsistent |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``starting the next innings throws an error`` ()=
        (fun () -> state |> updater StartNextInnings |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABBA_Ongoing tests`` =

    let a1 = createInnings 15 0
    let b1 = createInnings 10 0
    let b2 = createInnings 8 0
    let a2 = createInnings 1 0
    let state = ABBA_Ongoing (a1, b1, b2, a2)
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
        state |> updater DrawMatch |> should equal (ABBA_MatchDrawn (a1, b1, b2, a2))
        
    let sampleMatchOngoingInnings = createInnings 3 5
    let sampleUpdaterMatchOngoing = UpdateInnings (fun _ -> sampleMatchOngoingInnings)
        
    [<Test>]
    let ``updating the innings as ongoing with A not ahead should leave the state as ABBA_Ongoing`` ()=
        state |> updater sampleUpdaterMatchOngoing |> matchStateCase |> should equal ABBA_OngoingCase

    [<Test>]
    let ``updating the innings as ongoing with A not ahead should use the result of the innings update`` ()=
        state |> updater sampleUpdaterMatchOngoing |> should equal (ABBA_Ongoing (a1, b1, b2, sampleMatchOngoingInnings))

    let sampleOngoingInningsWicketsVictory = createInnings 5 7
    let sampleUpdaterOngoingWicketsVictory = UpdateInnings (fun _ -> sampleOngoingInningsWicketsVictory)

    [<Test>]
    let ``updating the innings as ongoing with A ahead should leave the state as ABBA_VictoryA`` ()=
        state |> updater sampleUpdaterOngoingWicketsVictory |> matchStateCase |> should equal ABBA_VictoryACase

    [<Test>]
    let ``updating the innings as ongoing with A ahead should use the result of the innings update`` ()=
        state |> updater sampleUpdaterOngoingWicketsVictory |> should equal (ABBA_VictoryA (a1, b1, b2, sampleOngoingInningsWicketsVictory))

    let sampleCompletedInningsRunsVictory = createInnings 2 10
    let sampleUpdaterCompletedRunsVictory = UpdateInnings (fun _ -> sampleCompletedInningsRunsVictory)

    [<Test>]
    let ``updating the innings as completed with A behind should leave the state as ABBA_VictoryB`` ()=
        state |> updater sampleUpdaterCompletedRunsVictory |> matchStateCase |> should equal ABBA_VictoryBCase

    [<Test>]
    let ``updating the innings as completed with A behind should use the result of the innings update`` ()=
        state |> updater sampleUpdaterCompletedRunsVictory |> should equal (ABBA_VictoryB (a1, b1, b2, sampleCompletedInningsRunsVictory))
    
    let sampleCompletedInningsTie = createInnings 3 10
    let sampleUpdaterCompletedTie = UpdateInnings (fun _ -> sampleCompletedInningsTie)

    [<Test>]
    let ``updating the innings as completed with A level should leave the state as ABBA_MatchTied`` ()=
        state |> updater sampleUpdaterCompletedTie |> matchStateCase |> should equal ABBA_MatchTiedCase

    [<Test>]
    let ``updating the innings as completed with A level should use the result of the innings update`` ()=
        state |> updater sampleUpdaterCompletedTie |> should equal (ABBA_MatchTied (a1, b1, b2, sampleCompletedInningsTie))

    let sampleCompletedInningsInconsistent = createInnings 4 10
    let sampleUpdaterCompletedInconsistent = UpdateInnings (fun _ -> sampleCompletedInningsInconsistent)

    [<Test>]
    let ``updating the innings as completed with A ahead throws an error`` ()=
        (fun () -> state |> updater sampleUpdaterCompletedInconsistent |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``starting the next innings throws an error`` ()=
        (fun () -> state |> updater StartNextInnings |> ignore) |> should throw typeof<System.Exception>
