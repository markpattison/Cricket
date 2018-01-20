namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine
open TestHelpers

module ``SummaryMatchState tests`` =

    let summary state = MatchState.summaryState state

    let inProgress state =
        match state with
        | InningsInProgress _ -> true
        | _ -> false

    [<Test>]
    let ``match not started`` ()=
        let state = NotStarted
        (summary state) |> should equal NotYetStarted

    [<Test>]
    let ``abandoned`` ()=
        let state = Abandoned
        (summary state) |> should equal (MatchCompleted NoResult)

    [<Test>]
    let ``A ongoing`` ()=
        let state = A'Ongoing (20 %/ 3)
        (summary state) |> inProgress |> should be True

    [<Test>]
    let ``A completed declared`` ()=
        let state = A'Completed (30 %/% 4)
        (summary state) |> should equal BetweenInnings

    [<Test>]
    let ``A completed not declared`` ()=
        let state = A'Completed (40 %/ 5)
        (summary state) |> should equal BetweenInnings
    
    [<Test>]
    let ``A match drawn`` ()=
        let state = A'MatchDrawn sampleEmptyInnings
        (summary state) |> should equal (MatchCompleted Draw)

    [<Test>]
    let ``AB ongoing B, behind`` ()=
        let state = AB'Ongoing (100 %/ 10, 50 %/ 9)
        (summary state) |> inProgress |> should be True

    [<Test>]
    let ``AB ongoing B, level`` ()=
        let state = AB'Ongoing (100 %/ 10, 100 %/ 0)
        (summary state) |> inProgress |> should be True

    [<Test>]
    let ``AB ongoing, B ahead`` ()=
        let state = AB'Ongoing (100 %/ 10, 120 %/ 2)
        (summary state) |> inProgress |> should be True

    [<Test>]
    let ``AB completed, no follow on, B behind`` ()=
        let state = AB'CompletedNoFollowOn (100 %/ 10, 99 %/ 10)
        (summary state) |> should equal BetweenInnings

    [<Test>]
    let ``AB completed, no follow on, B level`` ()=
        let state = AB'CompletedNoFollowOn (100 %/ 10, 100 %/ 10)
        (summary state) |> should equal BetweenInnings

    [<Test>]
    let ``AB completed, no follow on, B ahead`` ()=
        let state = AB'CompletedNoFollowOn (100 %/ 10, 101 %/ 10)
        (summary state) |> should equal BetweenInnings

    [<Test>]
    let ``AB completed, possible follow on, B behind`` ()=
        let state = AB'CompletedPossibleFollowOn (100 %/ 10, 99 %/ 10)
        (summary state) |> should equal AwaitingFollowOnDecision

    [<Test>]
    let ``AB completed, possible follow on, B level`` ()=
        let state = AB'CompletedPossibleFollowOn (100 %/ 10, 100 %/ 10)
        (summary state) |> should equal AwaitingFollowOnDecision

    [<Test>]
    let ``AB completed, possible follow on, B ahead`` ()=
        let state = AB'CompletedPossibleFollowOn (100 %/ 10, 101 %/ 10)
        (summary state) |> should equal AwaitingFollowOnDecision

    [<Test>]
    let ``AB match drawn`` ()=
        let state = AB'MatchDrawn (sampleEmptyInnings, sampleEmptyInnings)
        (summary state) |> should equal (MatchCompleted Draw)

    [<Test>]
    let ``ABA ongoing, A ahead`` ()=
        let state = ABA'Ongoing (100 %/ 10, 100 %/ 10, 1 %/ 0)
        (summary state) |> inProgress |> should be True

    [<Test>]
    let ``ABA ongoing, A level`` ()=
        let state = ABA'Ongoing (100 %/ 10, 100 %/ 10, 0 %/ 9)
        (summary state) |> inProgress |> should be True

    [<Test>]
    let ``ABA ongoing, A behind`` ()=
        let state = ABA'Ongoing (100 %/ 10, 110 %/ 10, 5 %/ 5)
        (summary state) |> inProgress |> should be True

    [<Test>]
    let ``ABA victory B`` ()=
        let state = ABA'VictoryB (100 %/ 10, 110 %/ 10, 5 %/ 10)
        (summary state) |> should equal (MatchCompleted WinTeamB)

    [<Test>]
    let ``ABA completed`` ()=
        let state = ABA'Completed (100 %/ 10, 110 %/ 10, 20 %/ 10)
        (summary state) |> should equal BetweenInnings

    [<Test>]
    let ``ABA match drawn`` ()=
        let state = ABA'MatchDrawn (sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings)
        (summary state) |> should equal (MatchCompleted Draw)

    [<Test>]
    let ``ABB ongoing, B ahead`` ()=
        let state = ABB'Ongoing (100 %/ 10, 30 %/ 10, 110 %/ 5)
        (summary state) |> inProgress |> should be True

    [<Test>]
    let ``ABB ongoing, B level`` ()=
        let state = ABB'Ongoing (100 %/ 10, 30 %/ 10, 70 %/ 5)
        (summary state) |> inProgress |> should be True

    [<Test>]
    let ``ABB ongoing, B behind`` ()=
        let state = ABB'Ongoing (100 %/ 10, 30 %/ 10, 30 %/ 5)
        (summary state) |> inProgress |> should be True

    [<Test>]
    let ``ABB victory A`` ()=
        let state = ABB'VictoryA (100 %/ 10, 30 %/ 10, 30 %/ 10)
        (summary state) |> should equal (MatchCompleted WinTeamA)

    [<Test>]
    let ``ABB completed, B ahead`` ()=
        let state = ABB'Completed (100 %/ 10, 30 %/ 10, 110 %/ 10)
        (summary state) |> should equal BetweenInnings

    [<Test>]
    let ``ABB completed, B level`` ()=
        let state = ABB'Completed (100 %/ 10, 30 %/ 10, 70 %/ 10)
        (summary state) |> should equal BetweenInnings

    [<Test>]
    let ``ABB match drawn`` ()=
        let state = ABB'MatchDrawn (sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings)
        (summary state) |> should equal (MatchCompleted Draw)

    [<Test>]
    let ``ABAB ongoing, B level`` ()=
        let state = ABAB'Ongoing(100 %/ 10, 100 %/ 10, 70 %/ 10, 70 %/ 0)
        (summary state) |> inProgress |> should be True

    [<Test>]
    let ``ABAB ongoing, B behind`` ()=
        let state = ABAB'Ongoing(100 %/ 10, 100 %/ 10, 70 %/ 10, 50 %/ 9)
        (summary state) |> inProgress |> should be True

    [<Test>]
    let ``ABAB victory A`` ()=
        let state = ABAB'VictoryA (100 %/ 10, 100 %/ 10, 70 %/ 10, 68 %/ 10)
        (summary state) |> should equal (MatchCompleted WinTeamA)

    [<Test>]
    let ``ABAB victory B`` ()=
        let state = ABAB'VictoryB (100 %/ 10, 100 %/ 10, 70 %/ 10, 71 %/ 9)
        (summary state) |> should equal (MatchCompleted WinTeamB)

    [<Test>]
    let ``ABAB match drawn`` ()=
        let state = ABAB'MatchDrawn (sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings)
        (summary state) |> should equal (MatchCompleted Draw)

    [<Test>]
    let ``ABAB match tied`` ()=
        let state = ABAB'MatchTied (100 %/ 10, 100 %/ 10, 70 %/ 10, 70 %/ 10)
        (summary state) |> should equal (MatchCompleted Tie)

    [<Test>]
    let ``ABBA ongoing, A level`` ()=
        let state = ABBA'Ongoing (100 %/ 10, 70 %/ 10, 100 %/ 10, 70 %/ 8)
        (summary state) |> inProgress |> should be True

    [<Test>]
    let ``ABBA ongoing, A behind`` ()=
        let state = ABBA'Ongoing (100 %/ 10, 70 %/ 10, 100 %/ 10, 60 %/ 8)
        (summary state) |> inProgress |> should be True

    [<Test>]
    let ``ABBA victory A`` ()=
        let state = ABBA'VictoryA (100 %/ 10, 70 %/ 10, 70 %/ 10, 101 %/ 8)
        (summary state) |> should equal (MatchCompleted WinTeamA)

    [<Test>]
    let ``ABBA victory B`` ()=
        let state = ABBA'VictoryB (100 %/ 10, 70 %/ 10, 70 %/ 10, 30 %/ 7)
        (summary state) |> should equal (MatchCompleted WinTeamB)

    [<Test>]
    let ``ABBA match drawn`` ()=
        let state = ABBA'MatchDrawn (sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings, sampleEmptyInnings)
        (summary state) |> should equal (MatchCompleted Draw)

    [<Test>]
    let ``ABBA match tied`` ()=
        let state = ABAB'MatchTied (100 %/ 10, 70 %/ 10, 70 %/ 10, 100 %/ 10)
        (summary state) |> should equal (MatchCompleted Tie)