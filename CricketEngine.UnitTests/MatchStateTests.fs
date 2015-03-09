namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine
open TestHelpers

// non-ongoing cases

module ``MatchState NotStarted tests`` =

    let state = NotStarted
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match creates a new ongoing match`` ()=
        (StartMatch sampleInnings sampleMatchRules state) |> matchStateCase |> should equal A_OngoingCase

    [<Test>]
    let ``abandoning the match creates an abandoned match`` ()=
        (AbandonMatch sampleMatchRules state) |> should equal Abandoned

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> DrawMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState Abandoned tests`` =

    let state = Abandoned
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> DrawMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState A_Completed tests`` =

    let a1 = createInnings 5 0
    let state = A_Completed (a1)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        (DrawMatch sampleMatchRules state) |> should equal (A_MatchDrawn (a1))
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState A_MatchDrawn tests`` =

    let a1 = createInnings 5 0
    let state = A_MatchDrawn (a1)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState AB_CompletedNoFollowOn tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let state = AB_CompletedNoFollowOn (a1, b1)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        (DrawMatch sampleMatchRules state) |> should equal (AB_MatchDrawn (a1, b1))
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState AB_CompletedPossibleFollowOn tests`` =

    let a1 = createInnings 15 0
    let b1 = createInnings 10 0
    let state = AB_CompletedPossibleFollowOn (a1, b1)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        (DrawMatch sampleMatchRules state) |> should equal (AB_MatchDrawn (a1, b1))
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on leaves the state as ABB_Ongoing`` ()=
        (EnforceFollowOn sampleInnings sampleMatchRules state) |> matchStateCase |> should equal ABB_OngoingCase 

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (DeclineFollowOn sampleInnings sampleMatchRules state) |> matchStateCase |> should equal ABA_OngoingCase 

module ``MatchState AB_MatchDrawn tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let state = AB_MatchDrawn (a1, b1)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABA_VictoryB tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let a2 = createInnings 3 10
    let state = ABA_VictoryB (a1, b1, a2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABA_Completed tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let a2 = createInnings 3 0
    let state = ABA_Completed (a1, b1, a2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        (DrawMatch sampleMatchRules state) |> should equal (ABA_MatchDrawn (a1, b1, a2))
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABA_MatchDrawn tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let a2 = createInnings 3 0
    let state = ABA_MatchDrawn (a1, b1, a2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABB_VictoryA tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let b2 = createInnings 3 10
    let state = ABB_VictoryA (a1, b1, b2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABB_Completed tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let b2 = createInnings 3 0
    let state = ABB_Completed (a1, b1, b2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        (DrawMatch sampleMatchRules state) |> should equal (ABB_MatchDrawn (a1, b1, b2))
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABB_MatchDrawn tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let a2 = createInnings 3 0
    let state = ABB_MatchDrawn (a1, b1, a2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABAB_VictoryA tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let a2 = createInnings 8 0
    let b2 = createInnings 2 10
    let state = ABAB_VictoryA (a1, b1, a2, b2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABAB_VictoryB tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let a2 = createInnings 8 0
    let b2 = createInnings 4 0
    let state = ABAB_VictoryB (a1, b1, a2, b2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABAB_MatchDrawn tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let a2 = createInnings 8 0
    let b2 = createInnings 2 0
    let state = ABAB_MatchDrawn (a1, b1, a2, b2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABAB_MatchTied tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let a2 = createInnings 8 0
    let b2 = createInnings 3 10
    let state = ABAB_MatchTied (a1, b1, a2, b2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABBA_VictoryA tests`` =

    let a1 = createInnings 15 0
    let b1 = createInnings 10 0
    let b2 = createInnings 7 0
    let a2 = createInnings 3 0
    let state = ABBA_VictoryA (a1, b1, b2, a2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABBA_VictoryB tests`` =

    let a1 = createInnings 15 0
    let b1 = createInnings 10 0
    let b2 = createInnings 7 0
    let a2 = createInnings 1 10
    let state = ABAB_VictoryB (a1, b1, b2, a2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABBA_MatchDrawn tests`` =

    let a1 = createInnings 15 0
    let b1 = createInnings 10 0
    let b2 = createInnings 7 0
    let a2 = createInnings 1 0
    let state = ABBA_MatchDrawn (a1, b1, b2, a2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABBA_MatchTied tests`` =

    let a1 = createInnings 15 0
    let b1 = createInnings 10 0
    let b2 = createInnings 7 0
    let a2 = createInnings 2 10
    let state = ABBA_MatchTied (a1, b1, b2, a2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>
        
    [<Test>]
    let ``updating the current innings throws an error`` ()=
        (fun () -> sampleUpdaterOngoing state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

// ongoing cases

module ``MatchState A_Ongoing tests`` =

    let a1 = createInnings 5 0
    let state = A_Ongoing (a1)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        (DrawMatch sampleMatchRules state) |> should equal (A_MatchDrawn (a1))
    
    [<Test>]
    let ``updating the innings as ongoing should leave the state as A_Ongoing`` ()=
        (sampleUpdaterOngoing state) |> matchStateCase |> should equal A_OngoingCase

    [<Test>]
    let ``updating the innings as ongoing should use the result of the innings update`` ()=
        (sampleUpdaterOngoing state) |> should equal (A_Ongoing (sampleOngoingInnings))

    [<Test>]
    let ``updating the innings as completed should leave the state as A_Completed`` ()=
        (sampleUpdaterCompleted state) |> matchStateCase |> should equal A_CompletedCase

    [<Test>]
    let ``updating the innings as completed should use the result of the innings update`` ()=
        (sampleUpdaterCompleted state) |> should equal (A_Completed (sampleCompletedInnings))

module ``MatchState AB_Ongoing tests`` =

    let a1 = createInnings 500 0
    let b1 = createInnings 10 0
    let state = AB_Ongoing (a1, b1)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        (DrawMatch sampleMatchRules state) |> should equal (AB_MatchDrawn (a1, b1))
        
    [<Test>]
    let ``updating the innings as ongoing should leave the state as AB_Ongoing`` ()=
        (sampleUpdaterOngoing state) |> matchStateCase |> should equal AB_OngoingCase

    [<Test>]
    let ``updating the innings as ongoing should use the result of the innings update`` ()=
        (sampleUpdaterOngoing state) |> should equal (AB_Ongoing (a1, sampleOngoingInnings))

    let sampleCompletedInningsNoFollowOn = createInnings 301 10
    let sampleUpdaterCompletedNoFollowOn = UpdateInnings sampleMatchRules (fun _ -> InningsCompleted sampleCompletedInningsNoFollowOn)

    [<Test>]
    let ``updating the innings as completed with B on follow-on target should leave the state as AB_CompletedNoFollowOn`` ()=
        (sampleUpdaterCompletedNoFollowOn state) |> matchStateCase |> should equal AB_CompletedNoFollowOnCase

    [<Test>]
    let ``updating the innings as completed with B on follow-on target should use the result of the innings update`` ()=
        (sampleUpdaterCompletedNoFollowOn state) |> should equal (AB_CompletedNoFollowOn (a1, sampleCompletedInningsNoFollowOn))

    let sampleCompletedInningsPossibleFollowOn = createInnings 300 10
    let sampleUpdaterCompletedPossibleFollowOn = UpdateInnings sampleMatchRules (fun _ -> InningsCompleted sampleCompletedInningsPossibleFollowOn)

    [<Test>]
    let ``updating the innings as completed with B behind follow-on target should leave the state as AB_CompletedPossibleFollowOn`` ()=
        (sampleUpdaterCompletedPossibleFollowOn state) |> matchStateCase |> should equal AB_CompletedPossibleFollowOnCase

    [<Test>]
    let ``updating the innings as completed with B behind follow-on target should use the result of the innings update`` ()=
        (sampleUpdaterCompletedPossibleFollowOn state) |> should equal (AB_CompletedPossibleFollowOn (a1, sampleCompletedInningsPossibleFollowOn))

module ``MatchState ABA_Ongoing tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let a2 = createInnings 3 0
    let state = ABA_Ongoing (a1, b1, a2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        (DrawMatch sampleMatchRules state) |> should equal (ABA_MatchDrawn (a1, b1, a2))
        
    [<Test>]
    let ``updating the innings as ongoing should leave the state as ABA_Ongoing`` ()=
        (sampleUpdaterOngoing state) |> matchStateCase |> should equal ABA_OngoingCase

    [<Test>]
    let ``updating the innings as ongoing should use the result of the innings update`` ()=
        (sampleUpdaterOngoing state) |> should equal (ABA_Ongoing (a1, b1, sampleOngoingInnings))

    let sampleCompletedInningsNoVictory = createInnings 5 10
    let sampleUpdaterCompletedNoVictory = UpdateInnings sampleMatchRules (fun _ -> InningsCompleted sampleCompletedInningsNoVictory)

    [<Test>]
    let ``updating the innings as completed with A not behind should leave the state as ABA_Completed`` ()=
        (sampleUpdaterCompletedNoVictory state) |> matchStateCase |> should equal ABA_CompletedCase

    [<Test>]
    let ``updating the innings as completed with A not behindshould use the result of the innings update`` ()=
        (sampleUpdaterCompletedNoVictory state) |> should equal (ABA_Completed (a1, b1, sampleCompletedInningsNoVictory))

    let sampleCompletedInningsVictory = createInnings 4 10
    let sampleUpdaterCompletedVictory = UpdateInnings sampleMatchRules (fun _ -> InningsCompleted sampleCompletedInningsVictory)

    [<Test>]
    let ``updating the innings as completed with A behind should leave the state as ABA_VictoryB`` ()=
        (sampleUpdaterCompletedVictory state) |> matchStateCase |> should equal ABA_VictoryBCase

    [<Test>]
    let ``updating the innings as completed with A behind should use the result of the innings update`` ()=
        (sampleUpdaterCompletedVictory state) |> should equal (ABA_VictoryB (a1, b1, sampleCompletedInningsVictory))

module ``MatchState ABB_Ongoing tests`` =

    let a1 = createInnings 10 0
    let b1 = createInnings 5 0
    let b2 = createInnings 3 0
    let state = ABB_Ongoing (a1, b1, b2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        (DrawMatch sampleMatchRules state) |> should equal (ABB_MatchDrawn (a1, b1, b2))
        
    [<Test>]
    let ``updating the innings as ongoing should leave the state as ABB_Ongoing`` ()=
        (sampleUpdaterOngoing state) |> matchStateCase |> should equal ABB_OngoingCase

    [<Test>]
    let ``updating the innings as ongoing should use the result of the innings update`` ()=
        (sampleUpdaterOngoing state) |> should equal (ABB_Ongoing (a1, b1, sampleOngoingInnings))

    let sampleCompletedInningsNoVictory = createInnings 5 10
    let sampleUpdaterCompletedNoVictory = UpdateInnings sampleMatchRules (fun _ -> InningsCompleted sampleCompletedInningsNoVictory)

    [<Test>]
    let ``updating the innings as completed with B not behind should leave the state as ABB_Completed`` ()=
        (sampleUpdaterCompletedNoVictory state) |> matchStateCase |> should equal ABB_CompletedCase

    [<Test>]
    let ``updating the innings as completed with B not behind should use the result of the innings update`` ()=
        (sampleUpdaterCompletedNoVictory state) |> should equal (ABB_Completed (a1, b1, sampleCompletedInningsNoVictory))

    let sampleCompletedInningsVictory = createInnings 4 10
    let sampleUpdaterCompletedVictory = UpdateInnings sampleMatchRules (fun _ -> InningsCompleted sampleCompletedInningsVictory)

    [<Test>]
    let ``updating the innings as completed with B behind should leave the state as ABB_VictoryA`` ()=
        (sampleUpdaterCompletedVictory state) |> matchStateCase |> should equal ABB_VictoryACase

    [<Test>]
    let ``updating the innings as completed with B behind should use the result of the innings update`` ()=
        (sampleUpdaterCompletedVictory state) |> should equal (ABB_VictoryA (a1, b1, sampleCompletedInningsVictory))

module ``MatchState ABAB_Ongoing tests`` =

    let a1 = createInnings 5 0
    let b1 = createInnings 10 0
    let a2 = createInnings 8 0
    let b2 = createInnings 2 0
    let state = ABAB_Ongoing (a1, b1, a2, b2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        (DrawMatch sampleMatchRules state) |> should equal (ABAB_MatchDrawn (a1, b1, a2, b2))
    
    let sampleMatchOngoingInnings = createInnings 3 5
    let sampleUpdaterMatchOngoing = UpdateInnings sampleMatchRules (fun _ -> InningsOngoing sampleMatchOngoingInnings)
        
    [<Test>]
    let ``updating the innings as ongoing with B not ahead should leave the state as ABAB_Ongoing`` ()=
        (sampleUpdaterMatchOngoing state) |> matchStateCase |> should equal ABAB_OngoingCase

    [<Test>]
    let ``updating the innings as ongoing with B not ahead should use the result of the innings update`` ()=
        (sampleUpdaterMatchOngoing state) |> should equal (ABAB_Ongoing (a1, b1, a2, sampleMatchOngoingInnings))

    let sampleOngoingInningsWicketsVictory = createInnings 5 7
    let sampleUpdaterOngoingWicketsVictory = UpdateInnings sampleMatchRules (fun _ -> InningsOngoing sampleOngoingInningsWicketsVictory)

    [<Test>]
    let ``updating the innings as ongoing with B ahead should leave the state as ABAB_VictoryB`` ()=
        (sampleUpdaterOngoingWicketsVictory state) |> matchStateCase |> should equal ABAB_VictoryBCase

    [<Test>]
    let ``updating the innings as ongoing with B ahead should use the result of the innings update`` ()=
        (sampleUpdaterOngoingWicketsVictory state) |> should equal (ABAB_VictoryB (a1, b1, a2, sampleOngoingInningsWicketsVictory))

    let sampleCompletedInningsRunsVictory = createInnings 2 10
    let sampleUpdaterCompletedRunsVictory = UpdateInnings sampleMatchRules (fun _ -> InningsCompleted sampleCompletedInningsRunsVictory)

    [<Test>]
    let ``updating the innings as completed with B behind should leave the state as ABAB_VictoryA`` ()=
        (sampleUpdaterCompletedRunsVictory state) |> matchStateCase |> should equal ABAB_VictoryACase

    [<Test>]
    let ``updating the innings as completed with B behind should use the result of the innings update`` ()=
        (sampleUpdaterCompletedRunsVictory state) |> should equal (ABAB_VictoryA (a1, b1, a2, sampleCompletedInningsRunsVictory))
    
    let sampleCompletedInningsTie = createInnings 3 10
    let sampleUpdaterCompletedTie = UpdateInnings sampleMatchRules (fun _ -> InningsCompleted sampleCompletedInningsTie)

    [<Test>]
    let ``updating the innings as completed with B level should leave the state as ABAB_MatchTied`` ()=
        (sampleUpdaterCompletedTie state) |> matchStateCase |> should equal ABAB_MatchTiedCase

    [<Test>]
    let ``updating the innings as completed with B level should use the result of the innings update`` ()=
        (sampleUpdaterCompletedTie state) |> should equal (ABAB_MatchTied (a1, b1, a2, sampleCompletedInningsTie))

    let sampleCompletedInningsInconsistent = createInnings 4 10
    let sampleUpdaterCompletedInconsistent = UpdateInnings sampleMatchRules (fun _ -> InningsCompleted sampleCompletedInningsInconsistent)

    [<Test>]
    let ``updating the innings as completed with B ahead throws an error`` ()=
        (fun () -> sampleUpdaterCompletedInconsistent state |> ignore) |> should throw typeof<System.Exception>

module ``MatchState ABBA_Ongoing tests`` =

    let a1 = createInnings 15 0
    let b1 = createInnings 10 0
    let b2 = createInnings 8 0
    let a2 = createInnings 1 0
    let state = ABBA_Ongoing (a1, b1, b2, a2)
    let sampleInnings = TestHelpers.sampleEmptyInnings

    [<Test>]
    let ``starting the match throws an error`` ()=
        (fun () -> StartMatch sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``abandoning the match throws an error`` ()=
        (fun () -> AbandonMatch sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``enforcing the follow-on throws an error`` ()=
        (fun () -> EnforceFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``declining the follow-on throws an error`` ()=
        (fun () -> DeclineFollowOn sampleInnings sampleMatchRules state |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    let ``drawing the match creates a drawn match`` ()=
        (DrawMatch sampleMatchRules state) |> should equal (ABBA_MatchDrawn (a1, b1, b2, a2))
        
    let sampleMatchOngoingInnings = createInnings 3 5
    let sampleUpdaterMatchOngoing = UpdateInnings sampleMatchRules (fun _ -> InningsOngoing sampleMatchOngoingInnings)
        
    [<Test>]
    let ``updating the innings as ongoing with A not ahead should leave the state as ABBA_Ongoing`` ()=
        (sampleUpdaterMatchOngoing state) |> matchStateCase |> should equal ABBA_OngoingCase

    [<Test>]
    let ``updating the innings as ongoing with A not ahead should use the result of the innings update`` ()=
        (sampleUpdaterMatchOngoing state) |> should equal (ABBA_Ongoing (a1, b1, b2, sampleMatchOngoingInnings))

    let sampleOngoingInningsWicketsVictory = createInnings 5 7
    let sampleUpdaterOngoingWicketsVictory = UpdateInnings sampleMatchRules (fun _ -> InningsOngoing sampleOngoingInningsWicketsVictory)

    [<Test>]
    let ``updating the innings as ongoing with A ahead should leave the state as ABBA_VictoryA`` ()=
        (sampleUpdaterOngoingWicketsVictory state) |> matchStateCase |> should equal ABBA_VictoryACase

    [<Test>]
    let ``updating the innings as ongoing with A ahead should use the result of the innings update`` ()=
        (sampleUpdaterOngoingWicketsVictory state) |> should equal (ABBA_VictoryA (a1, b1, b2, sampleOngoingInningsWicketsVictory))

    let sampleCompletedInningsRunsVictory = createInnings 2 10
    let sampleUpdaterCompletedRunsVictory = UpdateInnings sampleMatchRules (fun _ -> InningsCompleted sampleCompletedInningsRunsVictory)

    [<Test>]
    let ``updating the innings as completed with A behind should leave the state as ABBA_VictoryB`` ()=
        (sampleUpdaterCompletedRunsVictory state) |> matchStateCase |> should equal ABBA_VictoryBCase

    [<Test>]
    let ``updating the innings as completed with A behind should use the result of the innings update`` ()=
        (sampleUpdaterCompletedRunsVictory state) |> should equal (ABBA_VictoryB (a1, b1, b2, sampleCompletedInningsRunsVictory))
    
    let sampleCompletedInningsTie = createInnings 3 10
    let sampleUpdaterCompletedTie = UpdateInnings sampleMatchRules (fun _ -> InningsCompleted sampleCompletedInningsTie)

    [<Test>]
    let ``updating the innings as completed with A level should leave the state as ABBA_MatchTied`` ()=
        (sampleUpdaterCompletedTie state) |> matchStateCase |> should equal ABBA_MatchTiedCase

    [<Test>]
    let ``updating the innings as completed with A level should use the result of the innings update`` ()=
        (sampleUpdaterCompletedTie state) |> should equal (ABBA_MatchTied (a1, b1, b2, sampleCompletedInningsTie))

    let sampleCompletedInningsInconsistent = createInnings 4 10
    let sampleUpdaterCompletedInconsistent = UpdateInnings sampleMatchRules (fun _ -> InningsCompleted sampleCompletedInningsInconsistent)

    [<Test>]
    let ``updating the innings as completed with A ahead throws an error`` ()=
        (fun () -> sampleUpdaterCompletedInconsistent state |> ignore) |> should throw typeof<System.Exception>
