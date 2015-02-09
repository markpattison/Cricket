namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine
open TestHelpers

module ``SummaryStatus tests`` =

    let summary state = SummaryStatus sampleMatchRules { TeamA = "TeamA"; TeamB = "TeamB"; State = state }

    [<Test>]
    let ``match not started`` ()=
        let state = NotStarted
        (summary state) |> should equal "Match not started"

    [<Test>]
    let ``abandoned`` ()=
        let state = Abandoned
        (summary state) |> should equal "Match abandoned without a ball being bowled"

    [<Test>]
    let ``A_Ongoing`` ()=
        let state = A_Ongoing (20 %/ 3)
        (summary state) |> should equal "TeamA are 20 for 3 in their first innings"

    [<Test>]
    let ``A_Completed declared`` ()=
        let state = A_Completed (30 %/% 4)
        (summary state) |> should equal "TeamA scored 30 for 4 declared in their first innings"

    [<Test>]
    let ``A_Completed not declared`` ()=
        let state = A_Completed (40 %/ 5)
        (summary state) |> should equal "TeamA scored 40 all out in their first innings"
    
    [<Test>]
    let ``A_MatchDrawn`` ()=
        let state = A_MatchDrawn sampleEmptyInnings
        (summary state) |> should equal "Match drawn"

    [<Test>]
    let ``AB_Ongoing B behind`` ()=
        let state = AB_Ongoing (100 %/ 10, 50 %/ 9)
        (summary state) |> should equal "TeamB trail by 50 runs with 1 wicket remaining in their first innings"

    [<Test>]
    let ``AB_Ongoing B level status`` ()=
        let state = AB_Ongoing (100 %/ 10, 100 %/ 0)
        (summary state) |> should equal "TeamB are level with 10 wickets remaining in their first innings"

    [<Test>]
    let ``AB_Ongoing B ahead`` ()=
        let state = AB_Ongoing (100 %/ 10, 120 %/ 2)
        (summary state) |> should equal "TeamB lead by 20 runs with 8 wickets remaining in their first innings"

    [<Test>]
    let ``AB_CompletedNoFollowOn B behind`` ()=
        let state = AB_CompletedNoFollowOn (100 %/ 10, 99 %/ 10)
        (summary state) |> should equal "TeamA lead by 1 run after the first innings"

    [<Test>]
    let ``AB_CompletedNoFollowOn B level`` ()=
        let state = AB_CompletedNoFollowOn (100 %/ 10, 100 %/ 10)
        (summary state) |> should equal "TeamB are level after the first innings"

    [<Test>]
    let ``AB_CompletedNoFollowOn B ahead`` ()=
        let state = AB_CompletedNoFollowOn (100 %/ 10, 101 %/ 10)
        (summary state) |> should equal "TeamB lead by 1 run after the first innings"

    [<Test>]
    let ``AB_CompletedPossibleFollowOn B behind`` ()=
        let state = AB_CompletedPossibleFollowOn (100 %/ 10, 99 %/ 10)
        (summary state) |> should equal "TeamA lead by 1 run after the first innings"

    [<Test>]
    let ``AB_CompletedPossibleFollowOn B level`` ()=
        let state = AB_CompletedPossibleFollowOn (100 %/ 10, 100 %/ 10)
        (summary state) |> should equal "TeamB are level after the first innings"

    [<Test>]
    let ``AB_CompletedPossibleFollowOn B ahead`` ()=
        let state = AB_CompletedPossibleFollowOn (100 %/ 10, 101 %/ 10)
        (summary state) |> should equal "TeamB lead by 1 run after the first innings"

    [<Test>]
    let ``AB_MatchDrawn`` ()=
        let state = AB_MatchDrawn (sampleEmptyInnings, sampleEmptyInnings)
        (summary state) |> should equal "Match drawn"

    [<Test>]
    let ``ABA_Ongoing A ahead`` ()=
        let state = ABA_Ongoing (100 %/ 10, 100 %/ 10, 1 %/ 0)
        (summary state) |> should equal "TeamA lead by 1 run with 10 wickets remaining in their second innings"

    [<Test>]
    let ``ABA_Ongoing A level`` ()=
        let state = ABA_Ongoing (100 %/ 10, 100 %/ 10, 0 %/ 9)
        (summary state) |> should equal "TeamA are level with 1 wicket remaining in their second innings"

    [<Test>]
    let ``ABA_Ongoing A behind`` ()=
        let state = ABA_Ongoing (100 %/ 10, 110 %/ 10, 5 %/ 5)
        (summary state) |> should equal "TeamA trail by 5 runs with 5 wickets remaining in their second innings"

    [<Test>]
    let ``ABA_VictoryB`` ()=
        let state = ABA_VictoryB (100 %/ 10, 110 %/ 10, 5 %/ 10)
        (summary state) |> should equal "TeamB won by 5 runs"

    [<Test>]
    let ``ABA_Completed`` ()=
        let state = ABA_Completed (100 %/ 10, 110 %/ 10, 20 %/ 10)
        (summary state) |> should equal "TeamB need 10 runs to win in their second innings"

    [<Test>]
    let ``ABA_MatchDrawn`` ()=
        let state = ABA_MatchDrawn (sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings)
        (summary state) |> should equal "Match drawn"

//        | ABB_Ongoing (a1, b1, b2)
//        | ABB_VictoryA (a1, b1, b2)
//        | ABB_Completed (a1, b1, b2)

    [<Test>]
    let ``ABB_MatchDrawn`` ()=
        let state = ABB_MatchDrawn (sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings)
        (summary state) |> should equal "Match drawn"

//        | ABAB_Ongoing (a1, b1, a2, b2)
//        | ABAB_VictoryA (a1, b1, a2, b2)
//        | ABAB_VictoryB (a1, b1, a2, b2)

    [<Test>]
    let ``ABAB_MatchDrawn`` ()=
        let state = ABAB_MatchDrawn (sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings)
        (summary state) |> should equal "Match drawn"

//        | ABAB_MatchTied (a1, b1, a2, b2)
//        | ABBA_Ongoing (a1, b1, b2, a2)
//        | ABBA_VictoryA (a1, b1, b2, a2)
//        | ABBA_VictoryB (a1, b1, b2, a2)

    [<Test>]
    let ``ABBA_MatchDrawn`` ()=
        let state = ABBA_MatchDrawn (sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings)
        (summary state) |> should equal "Match drawn"

//        | ABBA_MatchTied (a1, b1, b2, a2)