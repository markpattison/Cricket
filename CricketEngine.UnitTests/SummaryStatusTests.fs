namespace CricketEnging.UnitTests

open FsUnit
open NUnit.Framework

open MatchRules
open Innings
open MatchState
open Match

module ``SummaryStatus tests`` =

    let sampleMatchRules = { FollowOnMargin = 200; }
    let summary state = SummaryStatus sampleMatchRules { TeamA = "TeamA"; TeamB = "TeamB"; State = state }

    let sampleInnings runs wickets = Innings (runs, wickets, false)
    let sampleDeclaredInnings runs wickets = Innings (runs, wickets, true)

    let emptyInnings = sampleInnings 0 0

    [<Test>]
    let ``match not started status`` ()=
        let state = NotStarted
        (summary state) |> should equal "Match not started"

    [<Test>]
    let ``abandoned status`` ()=
        let state = Abandoned
        (summary state) |> should equal "Match abandoned without a ball being bowled"

    [<Test>]
    let ``A_Ongoing status`` ()=
        let state = A_Ongoing (sampleInnings 20 3)
        (summary state) |> should equal "TeamA are 20 for 3 in their first innings"

    [<Test>]
    let ``A_Completed declared status`` ()=
        let state = A_Completed (sampleDeclaredInnings 30 4)
        (summary state) |> should equal "TeamA scored 30 for 4 declared in their first innings"

    [<Test>]
    let ``A_Completed not declared status`` ()=
        let state = A_Completed (sampleInnings 40 5)
        (summary state) |> should equal "TeamA scored 40 all out in their first innings"
    
    [<Test>]
    let ``A_MatchDrawn status`` ()=
        let state = A_MatchDrawn emptyInnings
        (summary state) |> should equal "Match drawn"

//        | AB_Ongoing (a1, b1)
//        | AB_CompletedNoFollowOn (a1, b1)
//        | AB_CompletedPossibleFollowOn (a1, b1)

    [<Test>]
    let ``AB_MatchDrawn status`` ()=
        let state = AB_MatchDrawn (emptyInnings, emptyInnings)
        (summary state) |> should equal "Match drawn"

//        | AB_MatchDrawn (a1, b1)
//        | ABA_Ongoing (a1, b1, a2)
//        | ABA_VictoryB (a1, b1, a2)
//        | ABA_Completed (a1, b1, a2)

    [<Test>]
    let ``ABA_MatchDrawn status`` ()=
        let state = ABA_MatchDrawn (emptyInnings, emptyInnings, emptyInnings)
        (summary state) |> should equal "Match drawn"

//        | ABB_Ongoing (a1, b1, b2)
//        | ABB_VictoryA (a1, b1, b2)
//        | ABB_Completed (a1, b1, b2)

    [<Test>]
    let ``ABB_MatchDrawn status`` ()=
        let state = ABB_MatchDrawn (emptyInnings, emptyInnings, emptyInnings)
        (summary state) |> should equal "Match drawn"

//        | ABAB_Ongoing (a1, b1, a2, b2)
//        | ABAB_VictoryA (a1, b1, a2, b2)
//        | ABAB_VictoryB (a1, b1, a2, b2)

    [<Test>]
    let ``ABAB_MatchDrawn status`` ()=
        let state = ABAB_MatchDrawn (emptyInnings, emptyInnings, emptyInnings, emptyInnings)
        (summary state) |> should equal "Match drawn"

//        | ABAB_MatchTied (a1, b1, a2, b2)
//        | ABBA_Ongoing (a1, b1, b2, a2)
//        | ABBA_VictoryA (a1, b1, b2, a2)
//        | ABBA_VictoryB (a1, b1, b2, a2)

    [<Test>]
    let ``ABBA_MatchDrawn status`` ()=
        let state = ABBA_MatchDrawn (emptyInnings, emptyInnings, emptyInnings, emptyInnings)
        (summary state) |> should equal "Match drawn"

//        | ABBA_MatchTied (a1, b1, b2, a2)