﻿namespace Cricket.CricketEngine.UnitTests

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

    let sampleBatsman1 = Name "testBatsman 1"
    let sampleBatsman2 = Name "testBatsman 2"

    let sampleInnings =
        {
            Individuals = [ (sampleBatsman1, NewIndividualInnings); (sampleBatsman2, NewIndividualInnings) ];
            IsDeclared = false;
            IndexOfBatsmanAtEnd1 = Some 0;
            IndexOfBatsmanAtEnd2 = Some 1;
            EndFacingNext = End1;
            OversCompleted = 0;
            BallsSoFarThisOver = 0;
        }

    let sampleBowler =
        Name "testBowler"

    let sampleFielder =
        Name "testFielder"

    let sampleIndividualData = sampleIndividualInnings, sampleBowler, sampleFielder
    let sampleInningsData = sampleInnings, sampleBowler, sampleFielder

module TestHelpers =

    type MatchStateCase =
        | NotStartedCase
        | AbandonedCase
        | A_OngoingCase
        | A_CompletedCase
        | A_MatchDrawnCase
        | AB_OngoingCase
        | AB_CompletedNoFollowOnCase
        | AB_CompletedPossibleFollowOnCase
        | AB_MatchDrawnCase
        | ABA_OngoingCase
        | ABA_VictoryBCase
        | ABA_CompletedCase
        | ABA_MatchDrawnCase
        | ABB_OngoingCase
        | ABB_VictoryACase
        | ABB_CompletedCase
        | ABB_MatchDrawnCase
        | ABAB_OngoingCase
        | ABAB_VictoryACase
        | ABAB_VictoryBCase
        | ABAB_MatchDrawnCase
        | ABAB_MatchTiedCase
        | ABBA_OngoingCase
        | ABBA_VictoryACase
        | ABBA_VictoryBCase
        | ABBA_MatchDrawnCase
        | ABBA_MatchTiedCase

    let matchStateCase state =
        match state with
        | NotStarted _ -> NotStartedCase
        | Abandoned _ -> AbandonedCase
        | A_Ongoing _ -> A_OngoingCase
        | A_Completed _ -> A_CompletedCase
        | A_MatchDrawn _ -> A_MatchDrawnCase
        | AB_Ongoing _ -> AB_OngoingCase
        | AB_CompletedNoFollowOn _ -> AB_CompletedNoFollowOnCase
        | AB_CompletedPossibleFollowOn _ -> AB_CompletedPossibleFollowOnCase
        | AB_MatchDrawn _ -> AB_MatchDrawnCase
        | ABA_Ongoing _ -> ABA_OngoingCase
        | ABA_VictoryB _ -> ABA_VictoryBCase
        | ABA_Completed _ -> ABA_CompletedCase
        | ABA_MatchDrawn _ -> ABA_MatchDrawnCase
        | ABB_Ongoing _ -> ABB_OngoingCase
        | ABB_VictoryA _ -> ABB_VictoryACase
        | ABB_Completed _ -> ABB_CompletedCase
        | ABB_MatchDrawn _ -> ABB_MatchDrawnCase
        | ABAB_Ongoing _ -> ABAB_OngoingCase
        | ABAB_VictoryA _ -> ABAB_VictoryACase
        | ABAB_VictoryB _ -> ABAB_VictoryBCase
        | ABAB_MatchDrawn _ -> ABAB_MatchDrawnCase
        | ABAB_MatchTied _ -> ABAB_MatchTiedCase
        | ABBA_Ongoing _ -> ABBA_OngoingCase
        | ABBA_VictoryA _ -> ABBA_VictoryACase
        | ABBA_VictoryB _ -> ABBA_VictoryBCase
        | ABBA_MatchDrawn _ -> ABBA_MatchDrawnCase
        | ABBA_MatchTied _ -> ABBA_MatchTiedCase

    let sampleMatchRules = { FollowOnMargin = 200; }

    let rec createInnings score wickets =
        match wickets with
        | 0 -> { SampleData.sampleInnings with Individuals = [ (SampleData.sampleBatsman1, {NewIndividualInnings with Score = score}); (SampleData.sampleBatsman2, NewIndividualInnings) ] }
        | n -> { SampleData.sampleInnings with Individuals = (SampleData.sampleBatsman1, { NewIndividualInnings with HowOut = Some HowOut.RunOut }) :: (createInnings score (n - 1)).Individuals }

    let (%/) runs wickets = createInnings runs wickets
    let (%/%) runs wickets = { (createInnings runs wickets) with IsDeclared = true }

    let sampleOngoingInnings = 456 %/ 7
    let sampleCompletedInnings = 789 %/ 10
    let sampleEmptyInnings = 0 %/ 0

    let sampleUpdaterOngoing = UpdateInnings (fun _ -> InningsOngoing sampleOngoingInnings) sampleMatchRules
    let sampleUpdaterCompleted = UpdateInnings (fun _ -> InningsCompleted sampleCompletedInnings) sampleMatchRules