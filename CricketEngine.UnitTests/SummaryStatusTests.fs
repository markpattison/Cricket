namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine
open TestHelpers

module ``SummaryStatus tests`` =

    let summary state = Match.summaryStatus { TeamA = "TeamA"; TeamB = "TeamB"; State = state; Rules = sampleMatchRules }

    [<Test>]
    let ``match not started`` ()=
        let state = NotStarted
        (summary state) |> should equal "Match not started"

    [<Test>]
    let ``abandoned`` ()=
        let state = Abandoned
        (summary state) |> should equal "Match abandoned without a ball being bowled"

    [<Test>]
    let ``A ongoing`` ()=
        let state = A'Ongoing (20 %/ 3)
        (summary state) |> should equal "TeamA are 20 for 3 in their first innings"

    [<Test>]
    let ``A completed declared`` ()=
        let state = A'Completed (30 %/% 4)
        (summary state) |> should equal "TeamA scored 30 for 4 declared in their first innings"

    [<Test>]
    let ``A completed not declared`` ()=
        let state = A'Completed (40 %/ 5)
        (summary state) |> should equal "TeamA scored 40 all out in their first innings"
    
    [<Test>]
    let ``A match drawn`` ()=
        let state = A'MatchDrawn sampleEmptyInnings
        (summary state) |> should equal "Match drawn"

    [<Test>]
    let ``AB ongoing B, behind`` ()=
        let state = AB'Ongoing (100 %/ 10, 50 %/ 9)
        (summary state) |> should equal "TeamB trail by 50 runs with 1 wicket remaining in their first innings"

    [<Test>]
    let ``AB ongoing B, level`` ()=
        let state = AB'Ongoing (100 %/ 10, 100 %/ 0)
        (summary state) |> should equal "TeamB are level with 10 wickets remaining in their first innings"

    [<Test>]
    let ``AB ongoing, B ahead`` ()=
        let state = AB'Ongoing (100 %/ 10, 120 %/ 2)
        (summary state) |> should equal "TeamB lead by 20 runs with 8 wickets remaining in their first innings"

    [<Test>]
    let ``AB completed, no follow on, B behind`` ()=
        let state = AB'CompletedNoFollowOn (100 %/ 10, 99 %/ 10)
        (summary state) |> should equal "TeamA lead by 1 run after the first innings"

    [<Test>]
    let ``AB completed, no follow on, B level`` ()=
        let state = AB'CompletedNoFollowOn (100 %/ 10, 100 %/ 10)
        (summary state) |> should equal "TeamB are level after the first innings"

    [<Test>]
    let ``AB completed, no follow on, B ahead`` ()=
        let state = AB'CompletedNoFollowOn (100 %/ 10, 101 %/ 10)
        (summary state) |> should equal "TeamB lead by 1 run after the first innings"

    [<Test>]
    let ``AB completed, possible follow on, B behind`` ()=
        let state = AB'CompletedPossibleFollowOn (100 %/ 10, 99 %/ 10)
        (summary state) |> should equal "TeamA lead by 1 run after the first innings"

    [<Test>]
    let ``AB completed, possible follow on, B level`` ()=
        let state = AB'CompletedPossibleFollowOn (100 %/ 10, 100 %/ 10)
        (summary state) |> should equal "TeamB are level after the first innings"

    [<Test>]
    let ``AB completed, possible follow on, B ahead`` ()=
        let state = AB'CompletedPossibleFollowOn (100 %/ 10, 101 %/ 10)
        (summary state) |> should equal "TeamB lead by 1 run after the first innings"

    [<Test>]
    let ``AB match drawn`` ()=
        let state = AB'MatchDrawn (sampleEmptyInnings, sampleEmptyInnings)
        (summary state) |> should equal "Match drawn"

    [<Test>]
    let ``ABA ongoing, A ahead`` ()=
        let state = ABA'Ongoing (100 %/ 10, 100 %/ 10, 1 %/ 0)
        (summary state) |> should equal "TeamA lead by 1 run with 10 wickets remaining in their second innings"

    [<Test>]
    let ``ABA ongoing, A level`` ()=
        let state = ABA'Ongoing (100 %/ 10, 100 %/ 10, 0 %/ 9)
        (summary state) |> should equal "TeamA are level with 1 wicket remaining in their second innings"

    [<Test>]
    let ``ABA ongoing, A behind`` ()=
        let state = ABA'Ongoing (100 %/ 10, 110 %/ 10, 5 %/ 5)
        (summary state) |> should equal "TeamA trail by 5 runs with 5 wickets remaining in their second innings"

    [<Test>]
    let ``ABA victory B`` ()=
        let state = ABA'VictoryB (100 %/ 10, 110 %/ 10, 5 %/ 10)
        (summary state) |> should equal "TeamB won by 5 runs"

    [<Test>]
    let ``ABA completed`` ()=
        let state = ABA'Completed (100 %/ 10, 110 %/ 10, 20 %/ 10)
        (summary state) |> should equal "TeamB need 11 runs to win in their second innings"

    [<Test>]
    let ``ABA match drawn`` ()=
        let state = ABA'MatchDrawn (sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings)
        (summary state) |> should equal "Match drawn"

    [<Test>]
    let ``ABB ongoing, B ahead`` ()=
        let state = ABB'Ongoing (100 %/ 10, 30 %/ 10, 110 %/ 5)
        (summary state) |> should equal "TeamB lead by 40 runs with 5 wickets remaining in their second innings"

    [<Test>]
    let ``ABB ongoing, B level`` ()=
        let state = ABB'Ongoing (100 %/ 10, 30 %/ 10, 70 %/ 5)
        (summary state) |> should equal "TeamB are level with 5 wickets remaining in their second innings"

    [<Test>]
    let ``ABB ongoing, B behind`` ()=
        let state = ABB'Ongoing (100 %/ 10, 30 %/ 10, 30 %/ 5)
        (summary state) |> should equal "TeamB trail by 40 runs with 5 wickets remaining in their second innings"

    [<Test>]
    let ``ABB victory A`` ()=
        let state = ABB'VictoryA (100 %/ 10, 30 %/ 10, 30 %/ 10)
        (summary state) |> should equal "TeamA won by an innings and 40 runs"

    [<Test>]
    let ``ABB completed, B ahead`` ()=
        let state = ABB'Completed (100 %/ 10, 30 %/ 10, 110 %/ 10)
        (summary state) |> should equal "TeamA need 41 runs to win in their second innings"

    [<Test>]
    let ``ABB completed, B level`` ()=
        let state = ABB'Completed (100 %/ 10, 30 %/ 10, 70 %/ 10)
        (summary state) |> should equal "TeamA need 1 run to win in their second innings"

    [<Test>]
    let ``ABB match drawn`` ()=
        let state = ABB'MatchDrawn (sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings)
        (summary state) |> should equal "Match drawn"

    [<Test>]
    let ``ABAB ongoing, B level`` ()=
        let state = ABAB'Ongoing (100 %/ 10, 100 %/ 10, 70 %/ 10, 70 %/ 0)
        (summary state) |> should equal "TeamB need 1 run to win with 10 wickets remaining"

    [<Test>]
    let ``ABAB ongoing, B behind`` ()=
        let state = ABAB'Ongoing (100 %/ 10, 100 %/ 10, 70 %/ 10, 50 %/ 9)
        (summary state) |> should equal "TeamB need 21 runs to win with 1 wicket remaining"

    [<Test>]
    let ``ABAB victory A`` ()=
        let state = ABAB'VictoryA (100 %/ 10, 100 %/ 10, 70 %/ 10, 68 %/ 10)
        (summary state) |> should equal "TeamA won by 2 runs"

    [<Test>]
    let ``ABAB victory B`` ()=
        let state = ABAB'VictoryB (100 %/ 10, 100 %/ 10, 70 %/ 10, 71 %/ 9)
        (summary state) |> should equal "TeamB won by 1 wicket"

    [<Test>]
    let ``ABAB match drawn`` ()=
        let state = ABAB'MatchDrawn (sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings)
        (summary state) |> should equal "Match drawn"

    [<Test>]
    let ``ABAB match tied`` ()=
        let state = ABAB'MatchTied (100 %/ 10, 100 %/ 10, 70 %/ 10, 70 %/ 10)
        (summary state) |> should equal "Match tied"

    [<Test>]
    let ``ABBA ongoing, A level`` ()=
        let state = ABBA'Ongoing (100 %/ 10, 70 %/ 10, 100 %/ 10, 70 %/ 8)
        (summary state) |> should equal "TeamA need 1 run to win with 2 wickets remaining"

    [<Test>]
    let ``ABBA ongoing, A behind`` ()=
        let state = ABBA'Ongoing (100 %/ 10, 70 %/ 10, 100 %/ 10, 60 %/ 8)
        (summary state) |> should equal "TeamA need 11 runs to win with 2 wickets remaining"

    [<Test>]
    let ``ABBA victory A`` ()=
        let state = ABBA'VictoryA (100 %/ 10, 70 %/ 10, 70 %/ 10, 101 %/ 8)
        (summary state) |> should equal "TeamA won by 2 wickets"

    [<Test>]
    let ``ABBA victory B`` ()=
        let state = ABBA'VictoryB (100 %/ 10, 70 %/ 10, 70 %/ 10, 30 %/ 7)
        (summary state) |> should equal "TeamB won by 10 runs"

    [<Test>]
    let ``ABBA match drawn`` ()=
        let state = ABBA'MatchDrawn (sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings)
        (summary state) |> should equal "Match drawn"

    [<Test>]
    let ``ABBA match tied`` ()=
        let state = ABAB'MatchTied (100 %/ 10, 70 %/ 10, 70 %/ 10, 100 %/ 10)
        (summary state) |> should equal "Match tied"
