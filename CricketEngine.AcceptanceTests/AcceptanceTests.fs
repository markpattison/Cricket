namespace Cricket.CricketEngine.AcceptanceTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine

module ``Acceptance tests`` =

    let sampleMatchRules = { FollowOnMargin = 200; }

    let batsman1 = { Name = "batsman 1" }
    let batsman2 = { Name = "batsman 2" }
    let batsman3 = { Name = "batsman 3" }

    let dot = UpdateCurrentInnings (UpdateInningsWithBall DotBall)
    let score1 = UpdateCurrentInnings (UpdateInningsWithBall (ScoreRuns 1))

    [<Test>]
    let ``start first innings`` ()=
        let match' =
            NewMatch sampleMatchRules "Team A" "Team B"
            |> UpdateMatchState StartMatch
            |> UpdateCurrentInnings (SendInNewBatsman batsman1)
            |> UpdateCurrentInnings (SendInNewBatsman batsman2)

        SummaryStatus match' |> should equal "Team A are 0 for 0 in their first innings"

    [<Test>]
    let ``one Boycott over`` ()=
        let match' =
            NewMatch sampleMatchRules "Team A" "Team B"
            |> UpdateMatchState StartMatch
            |> UpdateCurrentInnings (SendInNewBatsman batsman1)
            |> UpdateCurrentInnings (SendInNewBatsman batsman2)
            |> dot
            |> dot
            |> dot
            |> dot
            |> dot
            |> score1

        let expected =
            {
                TeamA = "Team A";
                TeamB = "Team B";
                Rules = sampleMatchRules;
                State = A_Ongoing
                    {
                        Individuals =
                            [
                                batsman1, 
                                {
                                    Score = 1;
                                    HowOut = None;
                                    BallsFaced = 6;
                                    Fours = 0;
                                    Sixes = 0;
                                };
                                batsman2, 
                                {
                                    Score = 0;
                                    HowOut = None;
                                    BallsFaced = 0;
                                    Fours = 0;
                                    Sixes = 0;
                                };
                            ];
                        IsDeclared = false;
                        IndexOfBatsmanAtEnd1 = Some 1;
                        IndexOfBatsmanAtEnd2 = Some 0;
                        EndFacingNext = End2;
                        OversCompleted = 1;
                        BallsSoFarThisOver = 0;
                    }
            }

        match' |> should equal expected

    [<Test>]
    let ``non-striker run out`` ()=
        let match' =
            NewMatch sampleMatchRules "Team A" "Team B"
            |> UpdateMatchState StartMatch
            |> UpdateCurrentInnings (SendInNewBatsman batsman1)
            |> UpdateCurrentInnings (SendInNewBatsman batsman2)
            |> dot
            |> score1
            |> score1
            |> UpdateCurrentInnings (UpdateInningsWithBall (RunOutNonStriker (1, false)))
            |> UpdateCurrentInnings (SendInNewBatsman batsman3)

        let expected =
            {
                TeamA = "Team A";
                TeamB = "Team B";
                Rules = sampleMatchRules;
                State = A_Ongoing
                    {
                        Individuals =
                            [
                                batsman1, 
                                {
                                    Score = 2;
                                    HowOut = None;
                                    BallsFaced = 3;
                                    Fours = 0;
                                    Sixes = 0;
                                };
                                batsman2, 
                                {
                                    Score = 1;
                                    HowOut = Some RunOut;
                                    BallsFaced = 1;
                                    Fours = 0;
                                    Sixes = 0;
                                };
                                batsman3, 
                                {
                                    Score = 0;
                                    HowOut = None;
                                    BallsFaced = 0;
                                    Fours = 0;
                                    Sixes = 0;
                                };
                            ];
                        IsDeclared = false;
                        IndexOfBatsmanAtEnd1 = Some 2;
                        IndexOfBatsmanAtEnd2 = Some 0;
                        EndFacingNext = End1;
                        OversCompleted = 0;
                        BallsSoFarThisOver = 4;
                    }
            }

        match' |> should equal expected