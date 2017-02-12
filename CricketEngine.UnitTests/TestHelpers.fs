namespace Cricket.CricketEngine.UnitTests

open Cricket.CricketEngine

module SampleData =

    let sampleIndividualInnings =
        {
            Score = 50;
            BallsFaced = 80;
            HowOut = None;
            Fours = 5;
            Sixes = 1;
        }

    let sampleBowlingAnalysis =
        {
            Balls = 10;
            Maidens = 1;
            RunsConceded = 7;
            Wickets = 2;
        }

    let sampleBatsman1 = { Name = "testBatsman 1" }
    let sampleBatsman2 = { Name = "testBatsman 2" }

    let sampleInnings =
        {
            Individuals = [ (sampleBatsman1, IndividualInnings.create); (sampleBatsman2, IndividualInnings.create) ];
            IsDeclared = false;
            BatsmanAtEnd1 = Some sampleBatsman1;
            BatsmanAtEnd2 = Some sampleBatsman2;
            EndFacingNext = End1;
            OversCompleted = 0;
            BallsThisOver = [];
        }

    let sampleBowler =
        { Name = "testBowler" }

    let sampleFielder =
        { Name = "testFielder" }

    let sampleIndividualData = sampleIndividualInnings, sampleBowler, sampleFielder
    let sampleInningsData = sampleInnings, sampleBowler, sampleFielder
    let sampleBowlingData = sampleBowlingAnalysis, sampleBowler, sampleFielder

module TestHelpers =

    type MatchStateCase =
        | NotStartedCase
        | AbandonedCase
        | A'OngoingCase
        | A'CompletedCase
        | A'MatchDrawnCase
        | AB'OngoingCase
        | AB'CompletedNoFollowOnCase
        | AB'CompletedPossibleFollowOnCase
        | AB'MatchDrawnCase
        | ABA'OngoingCase
        | ABA'VictoryBCase
        | ABA'CompletedCase
        | ABA'MatchDrawnCase
        | ABB'OngoingCase
        | ABB'VictoryACase
        | ABB'CompletedCase
        | ABB'MatchDrawnCase
        | ABAB'OngoingCase
        | ABAB'VictoryACase
        | ABAB'VictoryBCase
        | ABAB'MatchDrawnCase
        | ABAB'MatchTiedCase
        | ABBA'OngoingCase
        | ABBA'VictoryACase
        | ABBA'VictoryBCase
        | ABBA'MatchDrawnCase
        | ABBA'MatchTiedCase

    let matchStateCase state =
        match state with
        | NotStarted _ -> NotStartedCase
        | Abandoned _ -> AbandonedCase
        | A'Ongoing _ -> A'OngoingCase
        | A'Completed _ -> A'CompletedCase
        | A'MatchDrawn _ -> A'MatchDrawnCase
        | AB'Ongoing _ -> AB'OngoingCase
        | AB'CompletedNoFollowOn _ -> AB'CompletedNoFollowOnCase
        | AB'CompletedPossibleFollowOn _ -> AB'CompletedPossibleFollowOnCase
        | AB'MatchDrawn _ -> AB'MatchDrawnCase
        | ABA'Ongoing _ -> ABA'OngoingCase
        | ABA'VictoryB _ -> ABA'VictoryBCase
        | ABA'Completed _ -> ABA'CompletedCase
        | ABA'MatchDrawn _ -> ABA'MatchDrawnCase
        | ABB'Ongoing _ -> ABB'OngoingCase
        | ABB'VictoryA _ -> ABB'VictoryACase
        | ABB'Completed _ -> ABB'CompletedCase
        | ABB'MatchDrawn _ -> ABB'MatchDrawnCase
        | ABAB'Ongoing _ -> ABAB'OngoingCase
        | ABAB'VictoryA _ -> ABAB'VictoryACase
        | ABAB'VictoryB _ -> ABAB'VictoryBCase
        | ABAB'MatchDrawn _ -> ABAB'MatchDrawnCase
        | ABAB'MatchTied _ -> ABAB'MatchTiedCase
        | ABBA'Ongoing _ -> ABBA'OngoingCase
        | ABBA'VictoryA _ -> ABBA'VictoryACase
        | ABBA'VictoryB _ -> ABBA'VictoryBCase
        | ABBA'MatchDrawn _ -> ABBA'MatchDrawnCase
        | ABBA'MatchTied _ -> ABBA'MatchTiedCase

    let sampleMatchRules = { FollowOnMargin = 200; }

    let rec createInnings score wickets =
        match wickets with
        | 0 -> { SampleData.sampleInnings with Individuals = [ (SampleData.sampleBatsman1, { IndividualInnings.create with Score = score}); (SampleData.sampleBatsman2, IndividualInnings.create) ] }
        | n -> { SampleData.sampleInnings with Individuals = (SampleData.sampleBatsman1, { IndividualInnings.create with HowOut = Some HowOut.RunOut }) :: (createInnings score (n - 1)).Individuals }

    let (%/) runs wickets = createInnings runs wickets
    let (%/%) runs wickets = { (createInnings runs wickets) with IsDeclared = true }

    let sampleOngoingInnings = 456 %/ 7
    let sampleCompletedInnings = 789 %/ 10
    let sampleEmptyInnings = 0 %/ 0

    let sampleUpdaterOngoing = UpdateInnings (fun _ -> sampleOngoingInnings)
    let sampleUpdaterCompleted = UpdateInnings (fun _ -> sampleCompletedInnings)
    let updater = MatchState.update sampleMatchRules

    let dotBalls n =
        [ for _ in 1 .. n -> DotBall ]